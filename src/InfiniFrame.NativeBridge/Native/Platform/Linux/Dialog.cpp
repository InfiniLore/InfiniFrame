#ifdef __linux__
/**
 * @file Dialog.cpp (Linux)
 * @brief Linux implementation of InfiniFrameDialog using GTK3 file-chooser and message dialogs
 */

#include "Core/InfiniFrameDialog.h"
#include <gtk/gtk.h>

/** @brief Distinguishes which GtkFileChooserAction to configure in ShowDialog */
enum DialogType {
    OpenFile, /// GTK_FILE_CHOOSER_ACTION_OPEN — select one or more files
    OpenFolder, /// GTK_FILE_CHOOSER_ACTION_SELECT_FOLDER — select one or more directories
    SaveFile /// GTK_FILE_CHOOSER_ACTION_SAVE — choose a save destination
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
        title, nullptr, action,
        "_Cancel", GTK_RESPONSE_CANCEL,
        buttonText, GTK_RESPONSE_ACCEPT,
        nullptr
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
        char** results = new char*[count];
        for (int i = 0; i < count; i++) {
            results[i] = g_strdup(static_cast<char*>(g_slist_nth_data(pathList, i)));
        }
        g_slist_free_full(pathList, g_free);
        *resultCount = count;
        gtk_widget_destroy(dialog);
        return results;
    }
    else {
        char* result = gtk_file_chooser_get_filename(GTK_FILE_CHOOSER(dialog));
        gtk_widget_destroy(dialog);
        return new char*[1]{result};
    }
}

InfiniFrameDialog::InfiniFrameDialog() {
}

InfiniFrameDialog::~InfiniFrameDialog() {
}

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
    const AutoString title,
    const AutoString defaultPath,
    const bool multiSelect,
    int* resultCount
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
    const AutoString title,
    const AutoString text,
    const DialogButtons buttons,
    const DialogIcon icon
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

    dialog = gtk_message_dialog_new(
        nullptr,
        GTK_DIALOG_MODAL,
        type,
        GTK_BUTTONS_NONE,
        "%s",
        title
        );
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
#endif
