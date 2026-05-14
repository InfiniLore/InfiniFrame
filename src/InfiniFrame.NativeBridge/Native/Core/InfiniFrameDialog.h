#pragma once
/**
 * @file InfiniFrameDialog.h
 * @brief Dialog handlers for file/folder operations and messages
 */

#ifndef INFINIFRAME_CORE_DIALOG_H
#define INFINIFRAME_CORE_DIALOG_H

#include "../Types/Basic.h"
#include "../Types/Dialog.h"

#ifdef __APPLE__
#include <Cocoa/Cocoa.h>
#endif

class InfiniFrameWindow; // forward declaration

/**
 * @brief Dialog handler for file/folder operations and message boxes
 */
class InfiniFrameDialog {
    public:
#ifdef _WIN32
        /**
         * @brief Construct dialog handler with parent window (Windows)
         * @param window Parent InfiniFrame window
         */
        InfiniFrameDialog(InfiniFrameWindow* window);
#else
        /**
         * @brief Construct dialog handler (Linux/macOS)
         */
        InfiniFrameDialog();
#endif

        /**
         * @brief Destroy dialog handler
         */
        ~InfiniFrameDialog();

        /**
         * @brief Show open file dialog
         * @param title Dialog title
         * @param defaultPath Default path
         * @param multiSelect Allow multiple selection
         * @param filters File filters (e.g., "*.txt;*.doc")
         * @param filterCount Number of filters
         * @param resultCount Output: number of selected files
         * @return Array of selected file paths
         */
        AutoString* ShowOpenFile(
            AutoString title,
            AutoString defaultPath,
            bool multiSelect,
            AutoString* filters,
            int filterCount,
            int* resultCount
            );

        /**
         * @brief Show open folder dialog
         * @param title Dialog title
         * @param defaultPath Default path
         * @param multiSelect Allow multiple selection
         * @param resultCount Output: number of selected folders
         * @return Array of selected folder paths
         */
        AutoString* ShowOpenFolder(AutoString title, AutoString defaultPath, bool multiSelect, int* resultCount);

        /**
         * @brief Show save file dialog
         * @param title Dialog title
         * @param defaultPath Default path
         * @param filters File filters
         * @param filterCount Number of filters
         * @param defaultFileName Default file name
         * @return Selected file path
         */
        AutoString ShowSaveFile(
            AutoString title,
            AutoString defaultPath,
            AutoString* filters,
            int filterCount,
            AutoString defaultFileName = nullptr
            );

        /**
         * @brief Show message dialog
         * @param title Dialog title
         * @param text Message text
         * @param buttons Button configuration
         * @param icon Icon type
         * @return User's response
         */
        DialogResult ShowMessage(AutoString title, AutoString text, DialogButtons buttons, DialogIcon icon);

    protected:
#ifdef __APPLE__
        NSImage* _errorIcon;
        NSImage* _infoIcon;
        NSImage* _questionIcon;
        NSImage* _warningIcon;
#elif _WIN32
        InfiniFrameWindow* _window;
#endif
};

#endif // INFINIFRAME_CORE_DIALOG_H
