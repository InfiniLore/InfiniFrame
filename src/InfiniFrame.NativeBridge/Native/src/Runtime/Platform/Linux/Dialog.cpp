// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <gtk/gtk.h>

#include "Runtime/Platform/Linux/Core/UiThread.Gtk.h"
#include "Runtime/Shared/Window/InfiniFrameDialog.h"
#include "Runtime/Shared/Window/InfiniFrameWindow.h"
#include "Runtime/Shared/Operations/DialogOperation.h"
#include "Runtime/Shared/Utilities/StringArrayCopy.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
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
 * @param dialog Target GtkFileChooserDialog widget
 * @param filters UTF-8 filter strings (array of length filterCount)
 * @param filterCount Number of filters
 */
void AddFilters(GtkWidget* dialog, AutoString* filters, const int filterCount) {
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
        gtk_file_chooser_add_filter(GTK_FILE_CHOOSER(dialog), filter);
        g_free(filterCopy); // Free the duplicated string
    }
}

/**
 * @brief Shared implementation for all GTK file-chooser dialogs.
 *
 * Constructs a GtkFileChooserDialog configured for the requested type, runs it
 * modally, and returns the selected path(s) as a heap-allocated array
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
AutoString* ShowDialog(
    const DialogType type,
    const AutoString title,
    const AutoString defaultPath,
    const bool multiSelect,
    AutoString* filters,
    const int filterCount,
    int* resultCount,
    const AutoString defaultFileName = nullptr
) {
    GtkFileChooserAction action = GTK_FILE_CHOOSER_ACTION_OPEN;
    const char* buttonText = "_Open";
    switch (type) {
        case OpenFile:
            action = GTK_FILE_CHOOSER_ACTION_OPEN;
            buttonText = "_Open";
            break;
        case OpenFolder:
            action = GTK_FILE_CHOOSER_ACTION_SELECT_FOLDER;
            buttonText = "_Select";
            break;
        case SaveFile:
            action = GTK_FILE_CHOOSER_ACTION_SAVE;
            buttonText = "_Save";
            break;
    }

    GtkWidget* dialog = gtk_file_chooser_dialog_new(
        title, nullptr, action, "_Cancel", GTK_RESPONSE_CANCEL, buttonText, GTK_RESPONSE_ACCEPT, nullptr
    );

    if (defaultPath != nullptr) {
        gtk_file_chooser_set_current_folder(GTK_FILE_CHOOSER(dialog), defaultPath);
    }
    if (type == OpenFile || type == OpenFolder) {
        gtk_file_chooser_set_select_multiple(GTK_FILE_CHOOSER(dialog), multiSelect);
    }
    if (type == SaveFile) {
        gtk_file_chooser_set_do_overwrite_confirmation(GTK_FILE_CHOOSER(dialog), TRUE);
        if (defaultFileName != nullptr)
            gtk_file_chooser_set_current_name(GTK_FILE_CHOOSER(dialog), defaultFileName);
    }
    if (type == OpenFile || type == SaveFile) {
        AddFilters(dialog, filters, filterCount);
    }

    gint res = gtk_dialog_run(GTK_DIALOG(dialog));

    if (res != GTK_RESPONSE_ACCEPT) {
        if (type == OpenFile || type == OpenFolder)
            *resultCount = 0;

        gtk_widget_destroy(dialog);
        return nullptr;
    }

    if (type == OpenFile || type == OpenFolder) {
        GSList* pathList = gtk_file_chooser_get_filenames(GTK_FILE_CHOOSER(dialog));
        int count = g_slist_length(pathList);
        auto* results = AllocateStringArray(count);
        for (int i = 0; i < count; i++) {
            results[i] = g_strdup(static_cast<char*>(g_slist_nth_data(pathList, i)));
        }
        g_slist_free_full(pathList, g_free);
        *resultCount = count;
        gtk_widget_destroy(dialog);
        return results;
    } else {
        char* result = gtk_file_chooser_get_filename(GTK_FILE_CHOOSER(dialog));
        gtk_widget_destroy(dialog);
        auto* arr = AllocateStringArray(1);
        arr[0] = result;
        return arr;
    }
}

InfiniFrameDialog::InfiniFrameDialog() {}

InfiniFrameDialog::~InfiniFrameDialog() {}

AutoString* InfiniFrameDialog::ShowOpenFile(
    const AutoString title,
    const AutoString defaultPath,
    const bool multiSelect,
    AutoString* filters,
    const int filterCount,
    int* resultCount
) {
    return ShowDialog(OpenFile, title, defaultPath, multiSelect, filters, filterCount, resultCount);
}

AutoString* InfiniFrameDialog::ShowOpenFolder(
    const AutoString title, const AutoString defaultPath, const bool multiSelect, int* resultCount
) {
    return ShowDialog(OpenFolder, title, defaultPath, multiSelect, nullptr, 0, resultCount);
}

AutoString InfiniFrameDialog::ShowSaveFile(
    const AutoString title,
    const AutoString defaultPath,
    AutoString* filters,
    const int filterCount,
    const AutoString defaultFileName
) {
    char** result = ShowDialog(SaveFile, title, defaultPath, false, filters, filterCount, nullptr, defaultFileName);
    if (result != nullptr) {
        char* value = result[0];
        delete[] result;
        return value;
    }
    return nullptr;
}

DialogResult InfiniFrameDialog::ShowMessage(
    const AutoString title, const AutoString text, const DialogButtons buttons, const DialogIcon icon
) {
    GtkWidget* dialog;
    GtkMessageType type;

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
        default:
            type = GTK_MESSAGE_OTHER;
            break;
    }

    dialog = gtk_message_dialog_new(nullptr, GTK_DIALOG_MODAL, type, GTK_BUTTONS_NONE, "%s", title);
    gtk_message_dialog_set_markup(GTK_MESSAGE_DIALOG(dialog), text);

    switch (buttons) {
        case DialogButtons::Ok:
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Ok", static_cast<gint>(DialogResult::Ok));
            break;
        case DialogButtons::OkCancel:
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Ok", static_cast<gint>(DialogResult::Ok));
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Cancel", static_cast<gint>(DialogResult::Cancel));
            break;
        case DialogButtons::YesNo:
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Yes", static_cast<gint>(DialogResult::Yes));
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_No", static_cast<gint>(DialogResult::No));
            break;
        case DialogButtons::YesNoCancel:
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Yes", static_cast<gint>(DialogResult::Yes));
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_No", static_cast<gint>(DialogResult::No));
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Cancel", static_cast<gint>(DialogResult::Cancel));
            break;
        case DialogButtons::RetryCancel:
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Retry", static_cast<gint>(DialogResult::Retry));
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Cancel", static_cast<gint>(DialogResult::Cancel));
            break;
        case DialogButtons::AbortRetryIgnore:
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Abort", static_cast<gint>(DialogResult::Abort));
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Retry", static_cast<gint>(DialogResult::Retry));
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Ignore", static_cast<gint>(DialogResult::Ignore));
            break;
        default:
            gtk_dialog_add_button(GTK_DIALOG(dialog), "_Ok", static_cast<gint>(DialogResult::Ok));
            break;
    }

    gint result = gtk_dialog_run(GTK_DIALOG(dialog));
    gtk_widget_destroy(dialog);

    switch (result) {
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

namespace {
    void ScheduleDialogCancellation(GtkWidget* dialog, const gint response) {
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
    }

    GtkWidget* CreateAsyncMessageDialog(
        InfiniFrameWindow* owner, AutoString title, AutoString text,
        const DialogButtons buttons, const DialogIcon icon
    ) {
        GtkMessageType type = GTK_MESSAGE_OTHER;
        switch (icon) {
            case DialogIcon::Info: type = GTK_MESSAGE_INFO; break;
            case DialogIcon::Warning: type = GTK_MESSAGE_WARNING; break;
            case DialogIcon::Error: type = GTK_MESSAGE_ERROR; break;
            case DialogIcon::Question: type = GTK_MESSAGE_QUESTION; break;
        }
        GtkWidget* dialog = gtk_message_dialog_new(
            GTK_WINDOW(owner->getGtkWindow()), GTK_DIALOG_MODAL, type, GTK_BUTTONS_NONE, "%s", title
        );
        gtk_message_dialog_set_markup(GTK_MESSAGE_DIALOG(dialog), text);
        const auto add = [dialog](const char* label, const DialogResult result) {
            gtk_dialog_add_button(GTK_DIALOG(dialog), label, static_cast<gint>(result));
        };
        switch (buttons) {
            case DialogButtons::Ok: add("_Ok", DialogResult::Ok); break;
            case DialogButtons::OkCancel:
                add("_Ok", DialogResult::Ok); add("_Cancel", DialogResult::Cancel); break;
            case DialogButtons::YesNo:
                add("_Yes", DialogResult::Yes); add("_No", DialogResult::No); break;
            case DialogButtons::YesNoCancel:
                add("_Yes", DialogResult::Yes); add("_No", DialogResult::No);
                add("_Cancel", DialogResult::Cancel); break;
            case DialogButtons::RetryCancel:
                add("_Retry", DialogResult::Retry); add("_Cancel", DialogResult::Cancel); break;
            case DialogButtons::AbortRetryIgnore:
                add("_Abort", DialogResult::Abort); add("_Retry", DialogResult::Retry);
                add("_Ignore", DialogResult::Ignore); break;
        }
        gtk_window_set_destroy_with_parent(GTK_WINDOW(dialog), TRUE);
        return dialog;
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

        AutoString* values = nullptr;
        int count = 0;
        if (response == GTK_RESPONSE_ACCEPT) {
            GSList* paths = gtk_file_chooser_get_filenames(GTK_FILE_CHOOSER(state->dialog));
            count = g_slist_length(paths);
            values = AllocateStringArray(count);
            int index = 0;
            for (GSList* item = paths; item != nullptr; item = item->next)
                values[index++] = g_strdup(static_cast<char*>(item->data));
            g_slist_free_full(paths, g_free);
        }

        g_signal_handlers_disconnect_by_data(state->dialog, state);
        if (!destroyed)
            gtk_widget_destroy(state->dialog);
        state->operation->CompleteFile(response == GTK_RESPONSE_ACCEPT ? 0 : 2, count, values);
        FreeStringArray(values, count);
        delete state;
    }

    void OnAsyncFileResponse(GtkDialog*, const gint response, gpointer userData) {
        CompleteAsyncFileDialog(static_cast<AsyncFileDialogState*>(userData), response, false);
    }

    void OnAsyncFileDestroyed(GtkWidget*, gpointer userData) {
        CompleteAsyncFileDialog(static_cast<AsyncFileDialogState*>(userData), GTK_RESPONSE_CANCEL, true);
    }

    struct AsyncMessageDialogState {
        GtkWidget* dialog;
        std::shared_ptr<DialogOperation> operation;
        bool completed = false;
    };

    void CompleteAsyncMessageDialog(
        AsyncMessageDialogState* state, const DialogResult result, const bool destroyed
    ) {
        if (state == nullptr || state->completed)
            return;
        state->completed = true;
        g_signal_handlers_disconnect_by_data(state->dialog, state);
        if (!destroyed)
            gtk_widget_destroy(state->dialog);
        state->operation->CompleteMessage(result);
        delete state;
    }

    void OnAsyncMessageResponse(GtkDialog*, const gint response, gpointer userData) {
        CompleteAsyncMessageDialog(
            static_cast<AsyncMessageDialogState*>(userData),
            static_cast<DialogResult>(response), false
        );
    }

    void OnAsyncMessageDestroyed(GtkWidget*, gpointer userData) {
        CompleteAsyncMessageDialog(
            static_cast<AsyncMessageDialogState*>(userData), DialogResult::Cancel, true
        );
    }

    GtkWidget* CreateAsyncFileDialog(
        InfiniFrameWindow* owner,
        const DialogType type,
        AutoString title,
        AutoString defaultPath,
        const bool multiSelect,
        AutoString* filters,
        const int filterCount,
        AutoString defaultFileName
    ) {
        const GtkFileChooserAction action = type == OpenFile ? GTK_FILE_CHOOSER_ACTION_OPEN
            : type == OpenFolder ? GTK_FILE_CHOOSER_ACTION_SELECT_FOLDER
                                 : GTK_FILE_CHOOSER_ACTION_SAVE;
        const char* accept = type == SaveFile ? "_Save" : type == OpenFolder ? "_Select" : "_Open";
        GtkWidget* dialog = gtk_file_chooser_dialog_new(
            title, GTK_WINDOW(owner->getGtkWindow()), action,
            "_Cancel", GTK_RESPONSE_CANCEL, accept, GTK_RESPONSE_ACCEPT, nullptr
        );
        gtk_window_set_destroy_with_parent(GTK_WINDOW(dialog), TRUE);
        if (defaultPath != nullptr && defaultPath[0] != '\0')
            gtk_file_chooser_set_current_folder(GTK_FILE_CHOOSER(dialog), defaultPath);
        if (type != SaveFile)
            gtk_file_chooser_set_select_multiple(GTK_FILE_CHOOSER(dialog), multiSelect);
        else {
            gtk_file_chooser_set_do_overwrite_confirmation(GTK_FILE_CHOOSER(dialog), TRUE);
            if (defaultFileName != nullptr && defaultFileName[0] != '\0')
                gtk_file_chooser_set_current_name(GTK_FILE_CHOOSER(dialog), defaultFileName);
        }
        if (type != OpenFolder)
            AddFilters(dialog, filters, filterCount);
        return dialog;
    }

    void BeginAsyncFileDialog(
        InfiniFrameWindow* owner,
        const DialogType type,
        const uint64_t operationId,
        AutoString title,
        AutoString defaultPath,
        const bool multiSelect,
        AutoString* filters,
        const int filterCount,
        AutoString defaultFileName,
        const FileDialogCompletedCallback completion,
        void* completionContext
    ) {
        GtkWidget* dialog = CreateAsyncFileDialog(
            owner, type, title, defaultPath, multiSelect, filters, filterCount, defaultFileName
        );
        const char* name = type == OpenFile ? "ShowOpenFile" : type == OpenFolder ? "ShowOpenFolder" : "ShowSaveFile";
        auto operation = owner->RegisterFileDialogOperation(operationId, name, completion, completionContext);
        auto* state = new AsyncFileDialogState{dialog, operation};
        g_signal_connect(dialog, "response", G_CALLBACK(OnAsyncFileResponse), state);
        g_signal_connect(dialog, "destroy", G_CALLBACK(OnAsyncFileDestroyed), state);
        operation->SetCancelAction([dialog] {
            ScheduleDialogCancellation(dialog, GTK_RESPONSE_CANCEL);
        });
        gtk_widget_show(dialog);
    }
}

void InfiniFrameWindow::BeginShowMessage(
    const uint64_t id, AutoString title, AutoString text,
    const DialogButtons buttons, const DialogIcon icon,
    const OperationCompletedCallback completion, void* context
) {
    auto operation = RegisterMessageDialogOperation(id, completion, context);
    GtkWidget* dialog = CreateAsyncMessageDialog(this, title, text, buttons, icon);
    auto* state = new AsyncMessageDialogState{dialog, operation};
    g_signal_connect(dialog, "response", G_CALLBACK(OnAsyncMessageResponse), state);
    g_signal_connect(dialog, "destroy", G_CALLBACK(OnAsyncMessageDestroyed), state);
    operation->SetCancelAction([dialog] {
        ScheduleDialogCancellation(dialog, static_cast<gint>(DialogResult::Cancel));
    });
    gtk_widget_show(dialog);
}

void InfiniFrameWindow::BeginShowOpenFile(
    const uint64_t id, AutoString title, AutoString path, const bool multiSelect,
    AutoString* filters, const int filterCount, const FileDialogCompletedCallback completion, void* context
) {
    BeginAsyncFileDialog(this, OpenFile, id, title, path, multiSelect, filters, filterCount, nullptr, completion, context);
}

void InfiniFrameWindow::BeginShowOpenFolder(
    const uint64_t id, AutoString title, AutoString path, const bool multiSelect,
    const FileDialogCompletedCallback completion, void* context
) {
    BeginAsyncFileDialog(this, OpenFolder, id, title, path, multiSelect, nullptr, 0, nullptr, completion, context);
}

void InfiniFrameWindow::BeginShowSaveFile(
    const uint64_t id, AutoString title, AutoString path, AutoString* filters, const int filterCount,
    AutoString defaultFileName, const FileDialogCompletedCallback completion, void* context
) {
    BeginAsyncFileDialog(this, SaveFile, id, title, path, false, filters, filterCount, defaultFileName, completion, context);
}
