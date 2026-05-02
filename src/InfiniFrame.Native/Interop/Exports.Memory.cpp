#include "Interop/ExportApi.h"

using namespace InfiniFrame::Native::Interop;

extern "C" {
    /**
     * @brief Get and clear the latest native error message for the current thread.
     * @return Error message string, or null when no message is available.
     */
    INFINIFRAME_NATIVE_EXPORT AutoString InfiniFrame_GetLastErrorMessage() {
        return RunReturnExport(static_cast<AutoString>(nullptr), [] {
            const NativeString& message = GetExportErrorMessage();
            if (message.empty())
                return static_cast<AutoString>(nullptr);

            return AllocateStringCopy(message);
        });
    }

    /**
     * @brief Free string allocated by native code
     * @param value String to free
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_FreeString(AutoString value) {
        if (value == nullptr) {
            return SetExportSuccess();
        }

        return RunExportStatus([&] {
            FreeNativeString(value);
        });
    }

    /**
     * @brief Free string array allocated by native code
     * @param values String array to free
     * @param count Number of strings in array
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_FreeStringArray(AutoString* values, const int count) {
        if (values == nullptr) {
            return SetExportSuccess();
        }

        if (count < 0)
            return SetExportInvalidArgument();

        return RunExportStatus([&] {
            FreeNativeStringArray(values, count);
        });
    }
}
