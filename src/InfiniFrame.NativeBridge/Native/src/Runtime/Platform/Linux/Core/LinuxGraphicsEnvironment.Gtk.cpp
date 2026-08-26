// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <dirent.h>
#include <mutex>

#include <glib.h>

#include "Runtime/Platform/Linux/Core/LinuxGraphicsEnvironment.Gtk.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    bool HasRenderDevice() {
        DIR* directory = opendir("/dev/dri");
        if (directory == nullptr)
            return false;

        bool hasRenderDevice = false;
        while (dirent* entry = readdir(directory)) {
            if (g_str_has_prefix(entry->d_name, "renderD")) {
                hasRenderDevice = true;
                break;
            }
        }

        closedir(directory);
        return hasRenderDevice;
    }

    bool IsTruthy(const char* value) {
        return value != nullptr && value[0] != '\0' && g_strcmp0(value, "0") != 0 &&
            g_ascii_strcasecmp(value, "false") != 0;
    }

    bool IsDisabled(const char* value) {
        return value != nullptr && (g_strcmp0(value, "0") == 0 || g_ascii_strcasecmp(value, "false") == 0);
    }

    void SetDefaultEnvironmentVariable(const char* name, const char* value) {
        if (g_getenv(name) == nullptr) {
            g_setenv(name, value, FALSE);
        }
    }
}

namespace infiniframe::linux_gtk {
    void ConfigureGraphicsEnvironment() {
        static std::once_flag configureOnce;
        std::call_once(
            configureOnce, [] {
                const char* forceSoftware = g_getenv("INFINIFRAME_LINUX_FORCE_SOFTWARE_RENDERING");
                const bool shouldUseSoftwareRendering =
                    IsTruthy(forceSoftware) ||
                    (forceSoftware == nullptr && (IsTruthy(g_getenv("CI")) || !HasRenderDevice()));

                if (IsDisabled(forceSoftware) || !shouldUseSoftwareRendering)
                    return;

                SetDefaultEnvironmentVariable("LIBGL_ALWAYS_SOFTWARE", "1");
                SetDefaultEnvironmentVariable("GALLIUM_DRIVER", "llvmpipe");
                SetDefaultEnvironmentVariable("MESA_LOADER_DRIVER_OVERRIDE", "llvmpipe");
                SetDefaultEnvironmentVariable("MESA_GL_VERSION_OVERRIDE", "3.3");
                SetDefaultEnvironmentVariable("WEBKIT_DISABLE_COMPOSITING_MODE", "1");
                SetDefaultEnvironmentVariable("WEBKIT_DISABLE_DMABUF_RENDERER", "1");
            });
    }
}