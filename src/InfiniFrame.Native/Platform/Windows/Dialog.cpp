#include "Core/InfiniFrame.h"

#include <cwchar>
#include <iostream>
#include <shobjidl.h>
#include <shlwapi.h>
#include <objbase.h>
#include <vector>

class Dll
{
public:
	explicit Dll(std::string const& name);
	~Dll();

	template<typename T> class Proc
	{
	public:
		Proc(Dll const& lib, std::string const& sym)
			: _mProc(static_cast<T*>(reinterpret_cast<void*>(GetProcAddress(lib._handle, sym.c_str()))))
		{}

		explicit operator bool() const { return _mProc != nullptr; }
		explicit operator T* () const { return _mProc; }

	private:
		T* _mProc;
	};

private:
	HMODULE _handle;
};

inline Dll::Dll(std::string const& name)
	: _handle(LoadLibraryA(name.c_str()))
{}

inline Dll::~Dll()
{
	if (_handle)
		FreeLibrary(_handle);
}

class NewStyleContext
{
public:
	NewStyleContext();
	~NewStyleContext();

private:
	static HANDLE Create();
	ULONG_PTR _cookie = 0;
};

inline NewStyleContext::NewStyleContext()
{
	static HANDLE hctx = Create();

	if (hctx != INVALID_HANDLE_VALUE)
		ActivateActCtx(hctx, &_cookie);
}

inline NewStyleContext::~NewStyleContext()
{
	DeactivateActCtx(0, _cookie);
}

inline HANDLE NewStyleContext::Create()
{
	Dll comdlg32("comdlg32.dll");

	const UINT len = GetSystemDirectoryA(nullptr, 0);
	std::string sysDir(len, '\0');
	GetSystemDirectoryA(const_cast<LPSTR>(sysDir.data()), len);

	const ACTCTXA actCtx =
	{
		sizeof(actCtx),
		ACTCTX_FLAG_RESOURCE_NAME_VALID | ACTCTX_FLAG_ASSEMBLY_DIRECTORY_VALID,
		"shell32.dll", 0, 0, sysDir.c_str(), reinterpret_cast<LPCSTR>(124), nullptr, nullptr,
	};

	return CreateActCtxA(&actCtx);
}

InfiniFrameDialog::InfiniFrameDialog(InfiniFrameWindow* window)
{
	_window = window;
	CoInitializeEx(nullptr, COINIT_APARTMENTTHREADED);
}

InfiniFrameDialog::~InfiniFrameDialog()
{
	CoUninitialize();
}

template<typename T>
T* Create(HRESULT* hResult, AutoStringConst title, const AutoStringConst defaultPath)
{
	static_assert(std::is_base_of<IFileDialog, T>::value, "T must inherit from IFileDialog");
	T* pfd = nullptr;
	const CLSID clsid = typeid(T) == typeid(IFileOpenDialog) ? CLSID_FileOpenDialog : typeid(T) == typeid(IFileSaveDialog) ? CLSID_FileSaveDialog : CLSID_FileOpenDialog;
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

void AddFilters(IFileDialog* pfd, wchar_t** filters, const int filterCount, InfiniFrameWindow* wndInstance, std::vector<std::wstring>& filterStorage)
{
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

AutoString* GetResults(IFileOpenDialog* pfd, HRESULT* hr, int* resultCount)
{
	IShellItemArray* psiResults = nullptr;
	*hr = pfd->GetResults(&psiResults);
	if (SUCCEEDED(*hr)) {
		DWORD count = 0;
		psiResults->GetCount(&count);
		if (count > 0) {
			*resultCount = static_cast<int>(count);
			auto** result = new wchar_t* [count];
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

AutoString* InfiniFrameDialog::ShowOpenFile(AutoString title, AutoString defaultPath, const bool multiSelect, AutoString* filters, const int filterCount, int* resultCount)
{
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
		}
		else {
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

AutoString* InfiniFrameDialog::ShowOpenFolder(AutoString title, AutoString defaultPath, const bool multiSelect, int* resultCount)
{
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
		}
		else {
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

AutoString InfiniFrameDialog::ShowSaveFile(AutoString title, AutoString defaultPath, AutoString* filters, const int filterCount, AutoString defaultFileName)
{
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

DialogResult InfiniFrameDialog::ShowMessage(AutoString title, AutoString text, const DialogButtons buttons, const DialogIcon icon)
{
	std::wstring wideTitle = _window->ToUTF16String(title);
	std::wstring wideText = _window->ToUTF16String(text);
	NewStyleContext ctx;

	UINT flags = {};

	switch (icon) {
		case DialogIcon::Info:	   flags |= MB_ICONINFORMATION;	break;
		case DialogIcon::Warning:  flags |= MB_ICONWARNING;	    break;
		case DialogIcon::Error:	   flags |= MB_ICONERROR;	    break;
		case DialogIcon::Question: flags |= MB_ICONQUESTION;    break;
	}

	switch (buttons) {
		case DialogButtons::Ok:               flags |= MB_OK;               break;
		case DialogButtons::OkCancel:         flags |= MB_OKCANCEL;         break;
		case DialogButtons::YesNo:			  flags |= MB_YESNO;			break;
		case DialogButtons::YesNoCancel:      flags |= MB_YESNOCANCEL;	    break;
		case DialogButtons::RetryCancel:	  flags |= MB_RETRYCANCEL;	    break;
		case DialogButtons::AbortRetryIgnore: flags |= MB_ABORTRETRYIGNORE; break;
	}

	const auto result = MessageBoxW(_window->getHwnd(), wideText.c_str(), wideTitle.c_str(), flags);

	switch (result) {
		case IDCANCEL: return DialogResult::Cancel;
		case IDOK:     return DialogResult::Ok;
		case IDYES:    return DialogResult::Yes;
		case IDNO:     return DialogResult::No;
		case IDABORT:  return DialogResult::Abort;
		case IDRETRY:  return DialogResult::Retry;
		case IDIGNORE: return DialogResult::Ignore;
		default:	   return DialogResult::Cancel;
	}
}
