#include "Interop/ExportApi.h"

using namespace InfiniFrame::Native::Interop;

extern "C" {
    /**
     * @brief Show open file dialog
     * @param inst InfiniFrame instance
     * @param title Dialog title
     * @param defaultPath Default path
     * @param multiSelect Allow multiple selection
     * @param filters File filters
     * @param filterCount Number of filters
     * @param resultCount Output: number of selected files
     * @return Array of selected file paths
     */
    INFINIFRAME_NATIVE_EXPORT AutoString* InfiniFrame_ShowOpenFile(
        InfiniFrameWindow* inst,
        const AutoString title,
        const AutoString defaultPath,
        const bool multiSelect,
        AutoString* filters,
        const int filterCount,
        int* resultCount
        ) {
        return RunWindowReturnExport(
            inst,
            static_cast<AutoString*>(nullptr),
            [=](InfiniFrameWindow& window) {
                return window.GetDialog()->ShowOpenFile(title, defaultPath, multiSelect, filters, filterCount, resultCount);
            },
            resultCount
            );
    }

    /**
     * @brief Show open folder dialog
     * @param inst InfiniFrame instance
     * @param title Dialog title
     * @param defaultPath Default path
     * @param multiSelect Allow multiple selection
     * @param resultCount Output: number of selected folders
     * @return Array of selected folder paths
     */
    INFINIFRAME_NATIVE_EXPORT AutoString* InfiniFrame_ShowOpenFolder(
        InfiniFrameWindow* inst,
        const AutoString title,
        const AutoString defaultPath,
        const bool multiSelect,
        int* resultCount
        ) {
        return RunWindowReturnExport(
            inst,
            static_cast<AutoString*>(nullptr),
            [=](InfiniFrameWindow& window) {
                return window.GetDialog()->ShowOpenFolder(title, defaultPath, multiSelect, resultCount);
            },
            resultCount
            );
    }

    /**
     * @brief Show save file dialog
     * @param inst InfiniFrame instance
     * @param title Dialog title
     * @param defaultPath Default path
     * @param filters File filters
     * @param filterCount Number of filters
     * @param defaultFileName Default file name
     * @return Selected file path
     */
    INFINIFRAME_NATIVE_EXPORT AutoString InfiniFrame_ShowSaveFile(
        InfiniFrameWindow* inst,
        const AutoString title,
        const AutoString defaultPath,
        AutoString* filters,
        const int filterCount,
        const AutoString defaultFileName
        ) {
        return RunWindowReturnExport(
            inst,
            static_cast<AutoString>(nullptr),
            [=](InfiniFrameWindow& window) {
                return window.GetDialog()->ShowSaveFile(title, defaultPath, filters, filterCount, defaultFileName);
            }
            );
    }

    /**
     * @brief Show message dialog
     * @param inst InfiniFrame instance
     * @param title Dialog title
     * @param text Message text
     * @param buttons Button configuration
     * @param icon Icon type
     * @return User response
     */
    INFINIFRAME_NATIVE_EXPORT DialogResult InfiniFrame_ShowMessage(
        InfiniFrameWindow* inst,
        const AutoString title,
        const AutoString text,
        const DialogButtons buttons,
        const DialogIcon icon
        ) {
        return RunWindowReturnExport(inst, DialogResult::Cancel, [=](InfiniFrameWindow& window) {
            return window.GetDialog()->ShowMessage(title, text, buttons, icon);
        });
    }
}
