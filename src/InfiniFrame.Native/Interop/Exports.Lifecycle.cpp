#include "Interop/ExportApi.h"

#include <memory>

using namespace InfiniFrame::Native::Interop;

extern "C" {
    /**
     * @brief Create new InfiniFrame window instance
     * @param initParams Initialization parameters
     * @return Raw pointer - ownership transferred to caller (.NET)
     */
    INFINIFRAME_NATIVE_EXPORT InfiniFrameWindow* InfiniFrame_ctor(InfiniFrameInitParams* initParams) {
        if (initParams == nullptr) {
            SetExportInvalidArgument();
            return nullptr;
        }

        return RunReturnExport(static_cast<InfiniFrameWindow*>(nullptr), [&] {
            auto instance = std::make_unique<InfiniFrameWindow>(initParams);
            return instance.release();
        });
    }

    /**
     * @brief Destroy InfiniFrame window instance
     * @param instance Raw pointer from InfiniFrame_ctor
     */
    INFINIFRAME_NATIVE_EXPORT NativeStatusCode InfiniFrame_dtor(InfiniFrameWindow* instance) {
        if (instance == nullptr) {
            return SetExportSuccess();
        }

        return RunExportStatus([&] {
            std::unique_ptr<InfiniFrameWindow> guard{instance};
        });
    }
}
