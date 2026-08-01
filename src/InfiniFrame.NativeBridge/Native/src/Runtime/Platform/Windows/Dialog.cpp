// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Shared/Window/InfiniFrame.h"
#include "Runtime/Shared/Operations/DialogOperation.h"
#include "Runtime/Shared/Utilities/StringArrayCopy.h"

#include <iostream>
#include <shobjidl.h>
#include <shlwapi.h>
#include <objbase.h>
#include <vector>
#include <thread>
#include <atomic>
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @brief RAII wrapper that loads a DLL on construction and frees it on destruction.
 * Used to ensure comdlg32 is available before activating the common-controls activation context
 */
class Dll {
    public:
    /** @brief Load the named DLL; handle is null if loading fails */
    explicit Dll(const std::string& name);
    /** @brief Unload the DLL if it was loaded successfully */
    ~Dll();

    /**
         * @brief Type-safe wrapper around a single exported function retrieved via GetProcAddress
         * @tparam T Function signature (e.g. BOOL(HWND, LPCWSTR))
         */
    template <typename T> class Proc {
        public:
        /**
                 * @brief Resolve a symbol from a loaded DLL
                 * @param lib DLL to search
                 * @param sym Exported symbol name
                 */
        Proc(const Dll& lib, const std::string& sym)
            : _mProc(static_cast<T*>(reinterpret_cast<void*>(GetProcAddress(lib._handle, sym.c_str())))) {}

        /** @brief Returns true if the symbol was resolved successfully */
        explicit operator bool() const {
            return _mProc != nullptr;
        }

        /** @brief Returns the raw function pointer */
        explicit operator T*() const {
            return _mProc;
        }

        private:
        T* _mProc;
    };

    private:
    HMODULE _handle;
};

inline Dll::Dll(const std::string& name)
    : _handle(LoadLibraryA(name.c_str())) {}

inline Dll::~Dll() {
    if (_handle)
        FreeLibrary(_handle);
}

/**
 * @brief RAII guard that activates the Common Controls v6 activation context for the duration of its lifetime.
 *
 * Required so that IFileDialog uses the modern Aero-style common controls instead of the
 * legacy Windows 95 look. The activation context is created once (statically) from shell32.dll's
 * embedded manifest resource (ID 124)
 */
class NewStyleContext {
    public:
    /** @brief Activate the Common Controls v6 context */
    NewStyleContext();
    /** @brief Deactivate the context */
    ~NewStyleContext();

    private:
    /** @brief Create the activation context from shell32.dll's manifest; called once */
    static HANDLE Create();

    struct ActivationContextHolder {
        HANDLE handle = INVALID_HANDLE_VALUE;

        ~ActivationContextHolder() {
            if (handle != INVALID_HANDLE_VALUE)
                ReleaseActCtx(handle);
        }
    };

    ULONG_PTR _cookie = 0; /// Activation cookie returned by ActivateActCtx; used to deactivate
};

inline NewStyleContext::NewStyleContext() {
    static ActivationContextHolder actCtx{Create()};

    if (actCtx.handle != INVALID_HANDLE_VALUE)
        ActivateActCtx(actCtx.handle, &_cookie);
}

inline NewStyleContext::~NewStyleContext() {
    if (_cookie != 0)
        DeactivateActCtx(0, _cookie);
}

inline HANDLE NewStyleContext::Create() {
    Dll comdlg32("comdlg32.dll");

    const UINT len = GetSystemDirectoryA(nullptr, 0);
    std::string sysDir(len, '\0');
    GetSystemDirectoryA(const_cast<LPSTR>(sysDir.data()), len);

    const ACTCTXA actCtx = {
        sizeof(actCtx),
        ACTCTX_FLAG_RESOURCE_NAME_VALID | ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID,
        "shell32.dll",
        0,
        0,
        sysDir.c_str(),
        reinterpret_cast<LPCSTR>(124),
        nullptr,
        nullptr,
    };

    return CreateActCtxA(&actCtx);
}

InfiniFrameDialog::InfiniFrameDialog(InfiniFrameWindow* window) {
    _window = window;
}

InfiniFrameDialog::~InfiniFrameDialog() = default;

/**
 * @brief Create and configure an IFileDialog (open or save) with a title and optional default folder.
 * @tparam T Either IFileOpenDialog or IFileSaveDialog; the correct CLSID is deduced automatically
 * @param hResult Output: HRESULT from CoCreateInstance / SetFolder
 * @param title UTF-16 dialog title
 * @param defaultPath UTF-16 path to pre-select as the starting folder; may be null
 * @return Pointer to the created dialog; caller owns the COM reference. Returns null on failure.
 */
template <typename T> T* Create(HRESULT* hResult, AutoStringConst title, const AutoStringConst defaultPath) {
    static_assert(std::is_base_of<IFileDialog, T>::value, "T must inherit from IFileDialog");
    T* pfd = nullptr;
    const CLSID clsid = typeid(T) == typeid(IFileOpenDialog) ? CLSID_FileOpenDialog
        : typeid(T) == typeid(IFileSaveDialog)               ? CLSID_FileSaveDialog
                                                             : CLSID_FileOpenDialog;
    HRESULT hr = CoCreateInstance(clsid, nullptr, CLSCTX_INPROC_SERVER, IID_PPV_ARGS(&pfd));
    if (SUCCEEDED(hr)) {
        pfd->SetTitle(title);

        if (defaultPath) {
            IShellItem* psiDefault = nullptr;
            hr = SHCreateItemFromParsingName(defaultPath, nullptr, IID_PPV_ARGS(&psiDefault));
            if (SUCCEEDED(hr)) {
                pfd->SetFolder(psiDefault);
                psiDefault->Release();
            }
        }

        *hResult = hr;
        return pfd;
    }
    return nullptr;
}

/**
 * @brief Attach file-type filters to an IFileDialog.
 *
 * Each filter string must be in the format "Display Name|*.ext1;*.ext2".
 * The function tokenises the string in-place so filterStorage keeps the
 * underlying wstring buffers alive until the dialog is shown.
 *
 * @param pfd Target dialog
 * @param filters UTF-8 filter strings (array of length filterCount)
 * @param filterCount Number of filters
 * @param wndInstance Window used for UTF-8 → UTF-16 conversion
 * @param filterStorage Backing storage for converted wide strings; must outlive the dialog
 */
void AddFilters(
    IFileDialog* pfd,
    wchar_t** filters,
    const int filterCount,
    InfiniFrameWindow* wndInstance,
    std::vector<std::wstring>& filterStorage
) {
    std::vector<COMDLG_FILTERSPEC> specs;
    for (int i = 0; i < filterCount; i++) {
        filterStorage.push_back(wndInstance->ToUTF16String(filters[i]));
        std::wstring& filterText = filterStorage.back();
        wchar_t* context = nullptr;
        wchar_t* filterName = wcstok_s(filterText.data(), L"|", &context);
        wchar_t* filterPattern = wcstok_s(nullptr, L"|", &context);
        if (filterName == nullptr)
            continue;
        if (filterPattern == nullptr)
            filterPattern = filterName;
        COMDLG_FILTERSPEC spec;
        spec.pszName = filterName;
        spec.pszSpec = filterPattern;
        specs.push_back(spec);
    }
    pfd->SetFileTypes(static_cast<UINT>(specs.size()), specs.data());
}

/**
 * @brief Retrieve all selected paths from an IFileOpenDialog after it has been shown.
 *
 * Allocates a heap array of wchar_t* strings (one per selected item).
 * Releases pfd unconditionally before returning
 *
 * @param pfd The dialog to query; released on return
 * @param hr Output: HRESULT from GetResults / GetItemAt
 * @param resultCount Output: number of paths in the returned array
 * @return Heap-allocated array of UTF-16 paths, or null if nothing was selected
 */
AutoString* GetResults(IFileOpenDialog* pfd, HRESULT* hr, int* resultCount) {
    *resultCount = 0;

    IShellItemArray* psiResults = nullptr;
    *hr = pfd->GetResults(&psiResults);
    if (SUCCEEDED(*hr)) {
        DWORD count = 0;
        psiResults->GetCount(&count);
        if (count > 0) {
            *resultCount = static_cast<int>(count);
            auto* result = AllocateStringArray(count);
            for (DWORD i = 0; i < count; ++i) {
                IShellItem* psiItem = nullptr;
                *hr = psiResults->GetItemAt(i, &psiItem);
                if (SUCCEEDED(*hr)) {
                    PWSTR pszName = nullptr;
                    *hr = psiItem->GetDisplayName(SIGDN_FILESYSPATH, &pszName);
                    if (SUCCEEDED(*hr)) {
                        const auto len = wcslen(pszName);
                        result[i] = new wchar_t[len + 1];
                        wcscpy_s(result[i], len + 1, pszName);
                        CoTaskMemFree(pszName);
                    }
                    psiItem->Release();
                }
            }
            psiResults->Release();
            pfd->Release();
            return result;
        }
        psiResults->Release();
    }
    pfd->Release();

    return nullptr;
}

AutoString* InfiniFrameDialog::ShowOpenFile(
    AutoString title,
    AutoString defaultPath,
    const bool multiSelect,
    AutoString* filters,
    const int filterCount,
    int* resultCount
) {
    HRESULT hr;
    std::wstring wideTitle = _window->ToUTF16String(title);
    std::wstring wideDefaultPath = _window->ToUTF16String(defaultPath);

    auto* pfd = Create<IFileOpenDialog>(&hr, wideTitle.c_str(), wideDefaultPath.c_str());

    if (SUCCEEDED(hr)) {
        std::vector<std::wstring> filterStorage;
        AddFilters(pfd, filters, filterCount, _window, filterStorage);

        DWORD dwOptions;
        pfd->GetOptions(&dwOptions);
        dwOptions |= FOS_FILEMUSTEXIST | FOS_NOCHANGEDIR;
        if (multiSelect) {
            dwOptions |= FOS_ALLOWMULTISELECT;
        } else {
            dwOptions &= ~FOS_ALLOWMULTISELECT;
        }
        pfd->SetOptions(dwOptions);

        hr = pfd->Show(_window->getHwnd());
        if (SUCCEEDED(hr)) {
            return GetResults(pfd, &hr, resultCount);
        }
        pfd->Release();
    }
    return nullptr;
}

AutoString* InfiniFrameDialog::ShowOpenFolder(
    AutoString title, AutoString defaultPath, const bool multiSelect, int* resultCount
) {
    HRESULT hr;
    std::wstring wideTitle = _window->ToUTF16String(title);
    std::wstring wideDefaultPath = _window->ToUTF16String(defaultPath);

    auto* pfd = Create<IFileOpenDialog>(&hr, wideTitle.c_str(), wideDefaultPath.c_str());

    if (SUCCEEDED(hr)) {
        DWORD dwOptions;
        pfd->GetOptions(&dwOptions);
        dwOptions |= FOS_PICKFOLDERS | FOS_NOCHANGEDIR;
        if (multiSelect) {
            dwOptions |= FOS_ALLOWMULTISELECT;
        } else {
            dwOptions &= ~FOS_ALLOWMULTISELECT;
        }
        pfd->SetOptions(dwOptions);

        hr = pfd->Show(_window->getHwnd());
        if (SUCCEEDED(hr)) {
            return GetResults(pfd, &hr, resultCount);
        }
        pfd->Release();
    }
    return nullptr;
}

AutoString InfiniFrameDialog::ShowSaveFile(
    AutoString title, AutoString defaultPath, AutoString* filters, const int filterCount, AutoString defaultFileName
) {
    HRESULT hr;
    std::wstring wideTitle = _window->ToUTF16String(title);
    std::wstring wideDefaultPath = _window->ToUTF16String(defaultPath);
    std::wstring wideDefaultFileName = _window->ToUTF16String(defaultFileName);
    auto* pfd = Create<IFileSaveDialog>(&hr, wideTitle.c_str(), wideDefaultPath.c_str());
    if (SUCCEEDED(hr)) {
        if (!wideDefaultFileName.empty()) {
            pfd->SetFileName(wideDefaultFileName.c_str());
        }

        std::vector<std::wstring> filterStorage;
        AddFilters(pfd, filters, filterCount, _window, filterStorage);

        DWORD dwOptions;
        pfd->GetOptions(&dwOptions);
        dwOptions |= FOS_NOCHANGEDIR;
        pfd->SetOptions(dwOptions);

        hr = pfd->Show(_window->getHwnd());
        if (SUCCEEDED(hr)) {
            IShellItem* psiResult = nullptr;
            hr = pfd->GetResult(&psiResult);
            if (SUCCEEDED(hr)) {
                wchar_t* result = nullptr;
                PWSTR pszName = nullptr;
                hr = psiResult->GetDisplayName(SIGDN_FILESYSPATH, &pszName);
                if (SUCCEEDED(hr)) {
                    const auto len = wcslen(pszName);
                    result = new wchar_t[len + 1];
                    wcscpy_s(result, len + 1, pszName);
                    CoTaskMemFree(pszName);
                }
                psiResult->Release();
                pfd->Release();
                return result;
            }
        }
        pfd->Release();
    }
    return nullptr;
}

DialogResult InfiniFrameDialog::ShowMessage(
    AutoString title, AutoString text, const DialogButtons buttons, const DialogIcon icon
) {
    std::wstring wideTitle = _window->ToUTF16String(title);
    std::wstring wideText = _window->ToUTF16String(text);
    NewStyleContext ctx;

    UINT flags = {};

    switch (icon) {
        case DialogIcon::Info:
            flags |= MB_ICONINFORMATION;
            break;
        case DialogIcon::Warning:
            flags |= MB_ICONWARNING;
            break;
        case DialogIcon::Error:
            flags |= MB_ICONERROR;
            break;
        case DialogIcon::Question:
            flags |= MB_ICONQUESTION;
            break;
    }

    switch (buttons) {
        case DialogButtons::Ok:
            flags |= MB_OK;
            break;
        case DialogButtons::OkCancel:
            flags |= MB_OKCANCEL;
            break;
        case DialogButtons::YesNo:
            flags |= MB_YESNO;
            break;
        case DialogButtons::YesNoCancel:
            flags |= MB_YESNOCANCEL;
            break;
        case DialogButtons::RetryCancel:
            flags |= MB_RETRYCANCEL;
            break;
        case DialogButtons::AbortRetryIgnore:
            flags |= MB_ABORTRETRYIGNORE;
            break;
    }

    const auto result = MessageBoxW(_window->getHwnd(), wideText.c_str(), wideTitle.c_str(), flags);

    switch (result) {
        case IDCANCEL:
            return DialogResult::Cancel;
        case IDOK:
            return DialogResult::Ok;
        case IDYES:
            return DialogResult::Yes;
        case IDNO:
            return DialogResult::No;
        case IDABORT:
            return DialogResult::Abort;
        case IDRETRY:
            return DialogResult::Retry;
        case IDIGNORE:
            return DialogResult::Ignore;
        default:
            return DialogResult::Cancel;
    }
}

namespace {
    struct DialogThreadCancellation {
        std::atomic<DWORD> threadId = 0;
        std::atomic<bool> requested = false;
        HWND owner = nullptr;

        void Request() {
            requested.store(true, std::memory_order_release);
            const DWORD id = threadId.load(std::memory_order_acquire);
            if (id == 0) return;
            EnumThreadWindows(id, [](HWND hwnd, LPARAM value) -> BOOL {
                auto* state = reinterpret_cast<DialogThreadCancellation*>(value);
                if (IsWindowVisible(hwnd) && GetWindow(hwnd, GW_OWNER) == state->owner)
                    PostMessageW(hwnd, WM_CLOSE, 0, 0);
                return TRUE;
            }, reinterpret_cast<LPARAM>(this));
        }
    };

    thread_local DialogThreadCancellation* activeDialogCancellation = nullptr;

    LRESULT CALLBACK dialog_cancellation_hook(const int code, const WPARAM wParam, const LPARAM lParam) {
        if (code == HCBT_ACTIVATE && activeDialogCancellation != nullptr
            && activeDialogCancellation->requested.load(std::memory_order_acquire)) {
            PostMessageW(reinterpret_cast<HWND>(wParam), WM_CLOSE, 0, 0);
        }
        return CallNextHookEx(nullptr, code, wParam, lParam);
    }

    struct ScopedDialogCancellationHook {
        std::shared_ptr<DialogThreadCancellation> state;
        HHOOK hook = nullptr;

        explicit ScopedDialogCancellationHook(std::shared_ptr<DialogThreadCancellation> value)
            : state(std::move(value)) {
            state->threadId.store(GetCurrentThreadId(), std::memory_order_release);
            activeDialogCancellation = state.get();
            hook = SetWindowsHookExW(WH_CBT, dialog_cancellation_hook, nullptr, GetCurrentThreadId());
        }

        ~ScopedDialogCancellationHook() {
            if (hook != nullptr)
                UnhookWindowsHookEx(hook);
            activeDialogCancellation = nullptr;
            state->threadId.store(0, std::memory_order_release);
        }
    };
}

void InfiniFrameWindow::BeginShowOpenFile(
    const uint64_t operationId,
    AutoString title,
    AutoString defaultPath,
    const bool multiSelect,
    AutoString* filters,
    const int filterCount,
    const FileDialogCompletedCallback completion,
    void* completionContext
) {
    auto operation = RegisterFileDialogOperation(operationId, "ShowOpenFile", completion, completionContext);
    auto cancellation = std::make_shared<DialogThreadCancellation>();
    cancellation->owner = getHwnd();
    operation->SetCancelAction([cancellation] { cancellation->Request(); });
    NativeString titleCopy(title);
    NativeString pathCopy(defaultPath);
    std::vector<NativeString> filterCopies;
    for (int i = 0; i < filterCount; ++i)
        filterCopies.emplace_back(filters[i]);
    InfiniFrameDialog* dialog = GetDialog();

    std::thread([operationId, titleCopy = std::move(titleCopy), pathCopy = std::move(pathCopy),
                 filterCopies = std::move(filterCopies), multiSelect, operation, cancellation, dialog]() mutable {
        CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
        ScopedDialogCancellationHook cancellationHook(cancellation);
        std::vector<AutoString> filterPointers;
        for (auto& filter : filterCopies)
            filterPointers.push_back(filter.data());
        int count = 0;
        AutoString* values = cancellation->requested.load(std::memory_order_acquire) ? nullptr
            : dialog->ShowOpenFile(
                titleCopy.data(), pathCopy.data(), multiSelect, filterPointers.data(),
                static_cast<int>(filterPointers.size()), &count
            );
        operation->CompleteFile(values == nullptr ? 2 : 0, count, values);
        FreeStringArray(values, count);
        CoUninitialize();
    }).detach();
}

void InfiniFrameWindow::BeginShowOpenFolder(
    const uint64_t operationId,
    AutoString title,
    AutoString defaultPath,
    const bool multiSelect,
    const FileDialogCompletedCallback completion,
    void* completionContext
) {
    auto operation = RegisterFileDialogOperation(operationId, "ShowOpenFolder", completion, completionContext);
    auto cancellation = std::make_shared<DialogThreadCancellation>();
    cancellation->owner = getHwnd();
    operation->SetCancelAction([cancellation] { cancellation->Request(); });
    NativeString titleCopy(title);
    NativeString pathCopy(defaultPath);
    InfiniFrameDialog* dialog = GetDialog();
    std::thread([operationId, titleCopy = std::move(titleCopy), pathCopy = std::move(pathCopy),
                 multiSelect, operation, cancellation, dialog]() mutable {
        CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
        ScopedDialogCancellationHook cancellationHook(cancellation);
        int count = 0;
        AutoString* values = cancellation->requested.load(std::memory_order_acquire) ? nullptr
            : dialog->ShowOpenFolder(titleCopy.data(), pathCopy.data(), multiSelect, &count);
        operation->CompleteFile(values == nullptr ? 2 : 0, count, values);
        FreeStringArray(values, count);
        CoUninitialize();
    }).detach();
}

void InfiniFrameWindow::BeginShowSaveFile(
    const uint64_t operationId,
    AutoString title,
    AutoString defaultPath,
    AutoString* filters,
    const int filterCount,
    AutoString defaultFileName,
    const FileDialogCompletedCallback completion,
    void* completionContext
) {
    auto operation = RegisterFileDialogOperation(operationId, "ShowSaveFile", completion, completionContext);
    auto cancellation = std::make_shared<DialogThreadCancellation>();
    cancellation->owner = getHwnd();
    operation->SetCancelAction([cancellation] { cancellation->Request(); });
    NativeString titleCopy(title);
    NativeString pathCopy(defaultPath);
    NativeString fileNameCopy(defaultFileName);
    std::vector<NativeString> filterCopies;
    for (int i = 0; i < filterCount; ++i)
        filterCopies.emplace_back(filters[i]);
    InfiniFrameDialog* dialog = GetDialog();
    std::thread([operationId, titleCopy = std::move(titleCopy), pathCopy = std::move(pathCopy),
                 fileNameCopy = std::move(fileNameCopy), filterCopies = std::move(filterCopies),
                 operation, cancellation, dialog]() mutable {
        CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
        ScopedDialogCancellationHook cancellationHook(cancellation);
        std::vector<AutoString> filterPointers;
        for (auto& filter : filterCopies)
            filterPointers.push_back(filter.data());
        AutoString value = cancellation->requested.load(std::memory_order_acquire) ? nullptr
            : dialog->ShowSaveFile(
                titleCopy.data(), pathCopy.data(), filterPointers.data(),
                static_cast<int>(filterPointers.size()), fileNameCopy.data()
            );
        AutoString* values = nullptr;
        int count = 0;
        if (value != nullptr) {
            values = AllocateStringArray(1);
            values[0] = value;
            count = 1;
        }
        operation->CompleteFile(value == nullptr ? 2 : 0, count, values);
        FreeStringArray(values, count);
        CoUninitialize();
    }).detach();
}

void InfiniFrameWindow::BeginShowMessage(
    const uint64_t operationId, AutoString title, AutoString text,
    const DialogButtons buttons, const DialogIcon icon,
    const OperationCompletedCallback completion, void* completionContext
) {
    auto operation = RegisterMessageDialogOperation(operationId, completion, completionContext);
    auto cancellation = std::make_shared<DialogThreadCancellation>();
    cancellation->owner = getHwnd();
    operation->SetCancelAction([cancellation] { cancellation->Request(); });
    NativeString titleCopy(title);
    NativeString textCopy(text);
    InfiniFrameDialog* dialog = GetDialog();
    std::thread([titleCopy = std::move(titleCopy), textCopy = std::move(textCopy), buttons, icon,
                 operation, cancellation, dialog]() mutable {
        CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
        ScopedDialogCancellationHook cancellationHook(cancellation);
        const DialogResult value = cancellation->requested.load(std::memory_order_acquire)
            ? DialogResult::Cancel : dialog->ShowMessage(titleCopy.data(), textCopy.data(), buttons, icon);
        operation->CompleteMessage(value);
        CoUninitialize();
    }).detach();
}
