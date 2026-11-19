// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <condition_variable>
#include <mutex>
#include <string>
#include <vector>

#include <gtk/gtk.h>

#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#include "Runtime/Internal/Window/InfiniFrameDialog.h"
#include "Runtime/Internal/Interop/Types/InfiniFrameWindow.h"
#include "Runtime/Internal/Operations/DialogOperation.h"
#include "Runtime/Internal/Utilities/StringArrayCopy.h"

struct InfiniFrameDialog::Impl {};

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// GTK4 removed gtk_dialog_run (nested-loop dialogs). All dialogs now run on the
// asynchronous response model: widgets emit "response"/"destroy" and operations
// complete exactly once. The synchronous InfiniFrameDialog facade waits on that same
// model (owner-context modal wait on the GTK thread, condition-variable wait elsewhere).
// Note: GtkFileChooserDialog/GtkMessageDialog are deprecated in favor of GtkFileDialog/
// GtkAlertDialog (which are portal-based); the widget dialogs are kept because their
// lifecycle semantics match the existing operation model exactly.
/** @brief Distinguishes which GtkFileChooserAction to configure in ShowDialog */
enum DialogType {
    OpenFile, /// GTK_FILE_CHOOSER_ACTION_OPEN, select one or more files
    OpenFolder, /// GTK_FILE_CHOOSER_ACTION_SELECT_FOLDER, select one or more directories
    SaveFile /// GTK_FILE_CHOOSER_ACTION_SAVE, choose a save destination
};

/**
 * @brief Attach file-type filters to a GtkFileChooser.
 *
 * Each filter string must be in the format "Display Name|*.ext1;*.ext2"
 * @param chooser Target chooser
 * @param filters UTF-8 filter strings (array of length filterCount)
 * @param filterCount Number of filters
 */
void AddFilters(GtkFileChooser* chooser, const char** filters, const int filterCount) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    for (int i = 0; i < filterCount; i++) {
        GtkFileFilter* filter = gtk_file_filter_new();

        char* filterCopy = g_strdup(filters[i]); // Copy the string
        char* saveptr = nullptr;
        const char* name = strtok_r(filterCopy, "|", &saveptr);
        gtk_file_filter_set_name(filter, name);
        char* patterns = strtok_r(nullptr, "|", &saveptr);
        if (patterns != nullptr) {
            char* patternSavePtr = nullptr;
            char* pattern = strtok_r(patterns, ";", &patternSavePtr);
            while (pattern != nullptr) {
                gtk_file_filter_add_pattern(filter, pattern);
                pattern = strtok_r(nullptr, ";", &patternSavePtr);
            }
        }
        gtk_file_chooser_add_filter(chooser, filter);
        g_free(filterCopy); // Free the duplicated string
    }
G_GNUC_END_IGNORE_DEPRECATIONS
}

/**
 * @brief Builds a GtkFileChooserDialog configured for the requested type.
 *
 * @param type Kind of dialog to show (OpenFile, OpenFolder, SaveFile)
 * @param title UTF-8 dialog title
 * @param parent Transient parent window; may be null
 * @param defaultPath UTF-8 initial directory; may be null
 * @param multiSelect Allow selecting multiple items (ignored for SaveFile)
 * @param filters File-type filters; may be null
 * @param filterCount Number of entries in filters
 * @param defaultFileName UTF-8 pre-filled filename for SaveFile; may be null
 */
GtkWidget* BuildFileDialogWidget(
    const DialogType type,
    const char* title,
    GtkWindow* parent,
    const char* defaultPath,
    const bool multiSelect,
    const char** filters,
    const int filterCount,
    const char* defaultFileName
    ) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    const GtkFileChooserAction action = type == OpenFile
        ? GTK_FILE_CHOOSER_ACTION_OPEN
        : type == OpenFolder
        ? GTK_FILE_CHOOSER_ACTION_SELECT_FOLDER
        : GTK_FILE_CHOOSER_ACTION_SAVE;
    const char* accept = type == SaveFile ? "_Save" : type == OpenFolder ? "_Select" : "_Open";
    GtkWidget* dialog = gtk_file_chooser_dialog_new(
        title, parent, action, "_Cancel", GTK_RESPONSE_CANCEL, accept, GTK_RESPONSE_ACCEPT, nullptr
        );
    if (parent != nullptr)
        gtk_window_set_destroy_with_parent(GTK_WINDOW(dialog), TRUE);

    if (defaultPath != nullptr && defaultPath[0] != '\0') {
        GFile* folder = g_file_new_for_path(defaultPath);
        gtk_file_chooser_set_current_folder(GTK_FILE_CHOOSER(dialog), folder, nullptr);
        g_object_unref(folder);
    }
    if (type != SaveFile)
        gtk_file_chooser_set_select_multiple(GTK_FILE_CHOOSER(dialog), multiSelect);
    else if (defaultFileName != nullptr && defaultFileName[0] != '\0')
        gtk_file_chooser_set_current_name(GTK_FILE_CHOOSER(dialog), defaultFileName);
    if (type != OpenFolder)
        AddFilters(GTK_FILE_CHOOSER(dialog), filters, filterCount);
G_GNUC_END_IGNORE_DEPRECATIONS
    return dialog;
}

/**
 * @brief Extracts the selected paths from a GtkFileChooserDialog.
 *
 * @param dialog Target dialog (after GTK_RESPONSE_ACCEPT)
 * @param values Output: heap-allocated UTF-8 string array
 * @param count Output: number of paths returned
 */
void ExtractSelectedPaths(GtkWidget* dialog, const char*** values, int* count) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    GListModel* files = gtk_file_chooser_get_files(GTK_FILE_CHOOSER(dialog));
G_GNUC_END_IGNORE_DEPRECATIONS
    const guint n = files != nullptr ? g_list_model_get_n_items(files) : 0;
    auto* results = AllocateStringArray(static_cast<int>(n));
    for (guint i = 0; i < n; i++) {
        GFile* file = G_FILE(g_list_model_get_item(files, i));
        gchar* path = g_file_get_path(file);
        results[i] = AllocateStringCopy(std::string(path != nullptr ? path : ""));
        g_free(path);
        g_object_unref(file);
    }
    if (files != nullptr)
        g_object_unref(files);
    *values = results;
    *count = static_cast<int>(n);
}

/**
 * @brief Synchronous modal wait over the asynchronous dialog response model.
 *
 * The managed layer marshals synchronous dialog calls onto the GTK owner thread
 * (NativeInvoke.InvokeSyncWithValidation), where waiting for completion requires
 * pumping the owner context while the dialog is open. From other threads the wait
 * blocks on a condition variable. Every dialog completes exactly once: the
 * response/destroy handlers are one-shot and disconnected on completion.
 */
namespace {
    struct SyncDialogWaiter {
        GtkWidget* dialog = nullptr;
        std::mutex mutex;
        std::condition_variable completion;
        bool done = false;
        gint response = GTK_RESPONSE_CANCEL;
    };

    void CompleteSyncDialog(SyncDialogWaiter* waiter, const gint response) {
        {
            std::lock_guard lock(waiter->mutex);
            if (waiter->done)
                return;
            waiter->done = true;
            waiter->response = response;
        }
        g_signal_handlers_disconnect_by_data(waiter->dialog, waiter);
        waiter->completion.notify_all();
    }

    void OnSyncDialogResponse(GtkDialog*, const gint response, const gpointer userData) {
        CompleteSyncDialog(static_cast<SyncDialogWaiter*>(userData), response);
    }

    void OnSyncDialogDestroyed(GtkWidget*, const gpointer userData) {
        CompleteSyncDialog(static_cast<SyncDialogWaiter*>(userData), GTK_RESPONSE_CANCEL);
    }

    gint RunDialogModal(GtkWidget* dialog) {
        SyncDialogWaiter waiter;
        waiter.dialog = dialog;
        g_signal_connect(dialog, "response", G_CALLBACK(OnSyncDialogResponse), &waiter);
        g_signal_connect(dialog, "destroy", G_CALLBACK(OnSyncDialogDestroyed), &waiter);
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
        gtk_window_present(GTK_WINDOW(dialog));
G_GNUC_END_IGNORE_DEPRECATIONS

        if (infiniframe::linux_gtk::ui_thread::IsCurrentThread()) {
            // Modal wait on the GTK owner thread: pump the owner context until the
            // asynchronous dialog completes (the gtk_dialog_run replacement).
            for (;;) {
                {
                    std::lock_guard lock(waiter.mutex);
                    if (waiter.done)
                        break;
                }
                g_main_context_iteration(g_main_context_default(), TRUE);
            }
        } else {
            std::unique_lock lock(waiter.mutex);
            waiter.completion.wait(
                lock, [&] {
                    return waiter.done;
                });
        }

        std::lock_guard lock(waiter.mutex);
        return waiter.response;
    }
}

/**
 * @brief Shared implementation for all synchronous GTK file dialogs.
 *
 * Builds the dialog and runs it modally through the asynchronous response model,
 * returning the selected path(s) as a heap-allocated array
 *
 * @param type Kind of dialog to show (OpenFile, OpenFolder, SaveFile)
 * @param title UTF-8 dialog title
 * @param defaultPath UTF-8 initial directory; may be null
 * @param multiSelect Allow selecting multiple items (ignored for SaveFile)
 * @param filters File-type filters; may be null
 * @param filterCount Number of entries in filters
 * @param resultCount Output: number of paths returned (unused for SaveFile)
 * @param defaultFileName UTF-8 pre-filled filename for SaveFile; may be null
 * @return Heap-allocated array of UTF-8 path strings, or null if cancelled
 */
const char** ShowDialog(
    const DialogType type,
    const char* title,
    const char* defaultPath,
    const bool multiSelect,
    const char** filters,
    const int filterCount,
    int* resultCount,
    const char* defaultFileName = nullptr
    ) {
    GtkWidget* dialog = BuildFileDialogWidget(
        type, title, nullptr, defaultPath, multiSelect, filters, filterCount, defaultFileName
        );

    const gint res = RunDialogModal(dialog);

    const char** results = nullptr;
    if (res == GTK_RESPONSE_ACCEPT) {
        int count = 0;
        ExtractSelectedPaths(dialog, &results, &count);
        if (resultCount != nullptr)
            *resultCount = count;
    } else if (type == OpenFile || type == OpenFolder) {
        if (resultCount != nullptr)
            *resultCount = 0;
    }

    if (!gtk_widget_in_destruction(dialog))
        gtk_window_destroy(GTK_WINDOW(dialog));
    return results;
}

InfiniFrameDialog::InfiniFrameDialog() :
    m_impl(std::make_unique<Impl>()) {}

InfiniFrameDialog::~InfiniFrameDialog() {}

const char** InfiniFrameDialog::ShowOpenFile(
    const char* title,
    const char* defaultPath,
    const bool multiSelect,
    const char** filters,
    const int filterCount,
    int* resultCount
    ) {
    return ShowDialog(OpenFile, title, defaultPath, multiSelect, filters, filterCount, resultCount);
}

const char** InfiniFrameDialog::ShowOpenFolder(
    const char* title,
    const char* defaultPath,
    const bool multiSelect,
    int* resultCount
    ) {
    return ShowDialog(OpenFolder, title, defaultPath, multiSelect, nullptr, 0, resultCount);
}

const char* InfiniFrameDialog::ShowSaveFile(
    const char* title,
    const char* defaultPath,
    const char** filters,
    const int filterCount,
    const char* defaultFileName
    ) {
    const char** result = ShowDialog(
        SaveFile, title, defaultPath, false, filters, filterCount, nullptr, defaultFileName);
    if (result != nullptr) {
        const char* value = result[0];
        delete[] result;
        return value;
    }
    return nullptr;
}

namespace {
    /**
     * @brief Builds a GtkMessageDialog with the requested buttons.
     */
    GtkWidget* BuildMessageDialogWidget(
        GtkWindow* parent,
        const char* title,
        const char* text,
        const DialogButtons buttons,
        const DialogIcon icon
        ) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
        GtkMessageType type = GTK_MESSAGE_OTHER;
        switch (icon) {
            case DialogIcon::Info:
                type = GTK_MESSAGE_INFO;
                break;
            case DialogIcon::Warning:
                type = GTK_MESSAGE_WARNING;
                break;
            case DialogIcon::Error:
                type = GTK_MESSAGE_ERROR;
                break;
            case DialogIcon::Question:
                type = GTK_MESSAGE_QUESTION;
                break;
        }
        GtkWidget* dialog = gtk_message_dialog_new(
            parent, GTK_DIALOG_MODAL, type, GTK_BUTTONS_NONE, "%s", title
            );
        gtk_message_dialog_set_markup(GTK_MESSAGE_DIALOG(dialog), text);
        const auto add = [dialog](const char* label, const DialogResult result) {
            gtk_dialog_add_button(GTK_DIALOG(dialog), label, static_cast<gint>(result));
        };
        switch (buttons) {
            case DialogButtons::Ok:
                add("_Ok", DialogResult::Ok);
                break;
            case DialogButtons::OkCancel:
                add("_Ok", DialogResult::Ok);
                add("_Cancel", DialogResult::Cancel);
                break;
            case DialogButtons::YesNo:
                add("_Yes", DialogResult::Yes);
                add("_No", DialogResult::No);
                break;
            case DialogButtons::YesNoCancel:
                add("_Yes", DialogResult::Yes);
                add("_No", DialogResult::No);
                add("_Cancel", DialogResult::Cancel);
                break;
            case DialogButtons::RetryCancel:
                add("_Retry", DialogResult::Retry);
                add("_Cancel", DialogResult::Cancel);
                break;
            case DialogButtons::AbortRetryIgnore:
                add("_Abort", DialogResult::Abort);
                add("_Retry", DialogResult::Retry);
                add("_Ignore", DialogResult::Ignore);
                break;
        }
        if (parent != nullptr)
            gtk_window_set_destroy_with_parent(GTK_WINDOW(dialog), TRUE);
G_GNUC_END_IGNORE_DEPRECATIONS
        return dialog;
    }

    DialogResult MapDialogResult(const gint response) {
        switch (response) {
            case GTK_RESPONSE_CLOSE:
                return DialogResult::Cancel;
            case static_cast<gint>(DialogResult::Ok):
                return DialogResult::Ok;
            case static_cast<gint>(DialogResult::Yes):
                return DialogResult::Yes;
            case static_cast<gint>(DialogResult::No):
                return DialogResult::No;
            case static_cast<gint>(DialogResult::Cancel):
                return DialogResult::Cancel;
            case static_cast<gint>(DialogResult::Abort):
                return DialogResult::Abort;
            case static_cast<gint>(DialogResult::Retry):
                return DialogResult::Retry;
            case static_cast<gint>(DialogResult::Ignore):
                return DialogResult::Ignore;
            default:
                return DialogResult::Cancel;
        }
    }
}

DialogResult InfiniFrameDialog::ShowMessage(
    const char* title,
    const char* text,
    const DialogButtons buttons,
    const DialogIcon icon
    ) {
    GtkWidget* dialog = BuildMessageDialogWidget(nullptr, title, text, buttons, icon);
    const gint result = RunDialogModal(dialog);
    if (!gtk_widget_in_destruction(dialog))
        gtk_window_destroy(GTK_WINDOW(dialog));
    return MapDialogResult(result);
}

namespace {
    void ScheduleDialogCancellation(GtkWidget* dialog, const gint response) {
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
        if (!GTK_IS_DIALOG(dialog))
            return;

        // gtk_dialog_response emits the response signal synchronously. Defer it by one
        // owner-context turn so cancellation never invokes a managed completion callback
        // reentrantly from inside the native dispatch operation that requested cancellation.
        auto* retainedDialog = GTK_WIDGET(g_object_ref(dialog));
        const bool scheduled = infiniframe::linux_gtk::ui_thread::InvokeAsync(
            [retainedDialog, response] {
                if (GTK_IS_DIALOG(retainedDialog) && !gtk_widget_in_destruction(retainedDialog))
                    gtk_dialog_response(GTK_DIALOG(retainedDialog), response);
                g_object_unref(retainedDialog);
            }
            );
        if (!scheduled) {
            g_object_unref(retainedDialog);
            if (!gtk_widget_in_destruction(dialog))
                gtk_dialog_response(GTK_DIALOG(dialog), response);
        }
G_GNUC_END_IGNORE_DEPRECATIONS
    }

    GtkWidget* CreateAsyncMessageDialog(
        InfiniFrameWindow* owner,
        const char* title,
        const char* text,
        const DialogButtons buttons,
        const DialogIcon icon
        ) {
        return BuildMessageDialogWidget(
            GTK_WINDOW(owner->getGtkWindow()), title, text, buttons, icon);
    }

    struct AsyncFileDialogState {
        GtkWidget* dialog;
        std::shared_ptr<DialogOperation> operation;
        bool completed = false;
    };

    void CompleteAsyncFileDialog(AsyncFileDialogState* state, const gint response, const bool destroyed) {
        if (state == nullptr || state->completed)
            return;
        state->completed = true;

        const char** values = nullptr;
        int count = 0;
        if (response == GTK_RESPONSE_ACCEPT) {
            ExtractSelectedPaths(state->dialog, &values, &count);
        }

        g_signal_handlers_disconnect_by_data(state->dialog, state);
        if (!destroyed && !gtk_widget_in_destruction(state->dialog))
            gtk_window_destroy(GTK_WINDOW(state->dialog));
        state->operation->CompleteFile(response == GTK_RESPONSE_ACCEPT ? 0 : 2, count, values);
        FreeStringArray(values, count);
        delete state;
    }

    void OnAsyncFileResponse(GtkDialog*, const gint response, const gpointer userData) {
        CompleteAsyncFileDialog(static_cast<AsyncFileDialogState*>(userData), response, false);
    }

    void OnAsyncFileDestroyed(GtkWidget*, const gpointer userData) {
        CompleteAsyncFileDialog(static_cast<AsyncFileDialogState*>(userData), GTK_RESPONSE_CANCEL, true);
    }

    struct AsyncMessageDialogState {
        GtkWidget* dialog;
        std::shared_ptr<DialogOperation> operation;
        bool completed = false;
    };

    void CompleteAsyncMessageDialog(
        AsyncMessageDialogState* state,
        const DialogResult result,
        const bool destroyed
        ) {
        if (state == nullptr || state->completed)
            return;
        state->completed = true;
        g_signal_handlers_disconnect_by_data(state->dialog, state);
        if (!destroyed && !gtk_widget_in_destruction(state->dialog))
            gtk_window_destroy(GTK_WINDOW(state->dialog));
        state->operation->CompleteMessage(result);
        delete state;
    }

    void OnAsyncMessageResponse(GtkDialog*, const gint response, const gpointer userData) {
        CompleteAsyncMessageDialog(
            static_cast<AsyncMessageDialogState*>(userData),
            static_cast<DialogResult>(response), false
            );
    }

    void OnAsyncMessageDestroyed(GtkWidget*, const gpointer userData) {
        CompleteAsyncMessageDialog(
            static_cast<AsyncMessageDialogState*>(userData), DialogResult::Cancel, true
            );
    }

    void BeginAsyncFileDialog(
        InfiniFrameWindow* owner,
        const DialogType type,
        const uint64_t operationId,
        const char* title,
        const char* defaultPath,
        const bool multiSelect,
        const char** filters,
        const int filterCount,
        const char* defaultFileName,
        const FileDialogCompletedCallback completion,
        void* completionContext
        ) {
        GtkWidget* dialog = BuildFileDialogWidget(
            type, title, GTK_WINDOW(owner->getGtkWindow()), defaultPath, multiSelect,
            filters, filterCount, defaultFileName
            );
        const char* name = type == OpenFile ? "ShowOpenFile" : type == OpenFolder ? "ShowOpenFolder" : "ShowSaveFile";
        auto operation = owner->RegisterFileDialogOperation(operationId, name, completion, completionContext);
        auto* state = new AsyncFileDialogState{dialog, operation};
        g_signal_connect(dialog, "response", G_CALLBACK(OnAsyncFileResponse), state);
        g_signal_connect(dialog, "destroy", G_CALLBACK(OnAsyncFileDestroyed), state);
        operation->SetCancelAction(
            [dialog] {
                ScheduleDialogCancellation(dialog, GTK_RESPONSE_CANCEL);
            });
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
        gtk_window_present(GTK_WINDOW(dialog));
G_GNUC_END_IGNORE_DEPRECATIONS
    }
}

void InfiniFrameWindow::BeginShowMessage(
    const uint64_t id,
    const char* title,
    const char* text,
    const DialogButtons buttons,
    const DialogIcon icon,
    const OperationCompletedCallback completion,
    void* context
    ) {
    auto operation = RegisterMessageDialogOperation(id, completion, context);
    GtkWidget* dialog = CreateAsyncMessageDialog(this, title, text, buttons, icon);
    auto* state = new AsyncMessageDialogState{dialog, operation};
    g_signal_connect(dialog, "response", G_CALLBACK(OnAsyncMessageResponse), state);
    g_signal_connect(dialog, "destroy", G_CALLBACK(OnAsyncMessageDestroyed), state);
    operation->SetCancelAction(
        [dialog] {
            ScheduleDialogCancellation(dialog, static_cast<gint>(DialogResult::Cancel));
        });
G_GNUC_BEGIN_IGNORE_DEPRECATIONS
    gtk_window_present(GTK_WINDOW(dialog));
G_GNUC_END_IGNORE_DEPRECATIONS
}

void InfiniFrameWindow::BeginShowOpenFile(
    const uint64_t id,
    const char* title,
    const char* path,
    const bool multiSelect,
    const char** filters,
    const int filterCount,
    const FileDialogCompletedCallback completion,
    void* context
    ) {
    BeginAsyncFileDialog(
        this, OpenFile, id, title, path, multiSelect, filters, filterCount, nullptr, completion, context);
}

void InfiniFrameWindow::BeginShowOpenFolder(
    const uint64_t id,
    const char* title,
    const char* path,
    const bool multiSelect,
    const FileDialogCompletedCallback completion,
    void* context
    ) {
    BeginAsyncFileDialog(this, OpenFolder, id, title, path, multiSelect, nullptr, 0, nullptr, completion, context);
}

void InfiniFrameWindow::BeginShowSaveFile(
    const uint64_t id,
    const char* title,
    const char* path,
    const char** filters,
    const int filterCount,
    const char* defaultFileName,
    const FileDialogCompletedCallback completion,
    void* context
    ) {
    BeginAsyncFileDialog(
        this, SaveFile, id, title, path, false, filters, filterCount, defaultFileName, completion, context);
}
