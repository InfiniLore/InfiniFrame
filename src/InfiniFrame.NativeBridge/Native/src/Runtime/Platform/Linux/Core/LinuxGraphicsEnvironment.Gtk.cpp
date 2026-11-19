// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <dirent.h>
#include <mutex>

#include <glib.h>
#include <gtk/gtk.h>
#include <webkit/webkit.h>

#include "Runtime/Platform/Linux/Core/LinuxGraphicsEnvironment.Gtk.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    // Decision recorded by ConfigureGraphicsEnvironment for the rendering diagnostics.
    struct RenderingDecision {
        bool recorded = false;
        bool softwareRendering = false;
        const char* reason = "unknown";
    };

    RenderingDecision g_renderingDecision = {};

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

    const char* NullToDash(const char* value) {
        return value != nullptr ? value : "(unset)";
    }
}

namespace infiniframe::linux_gtk {
    void ConfigureGraphicsEnvironment() {
        static std::once_flag configureOnce;
        std::call_once(
            configureOnce, [] {
                const char* forceSoftware = g_getenv("INFINIFRAME_LINUX_FORCE_SOFTWARE_RENDERING");
                const bool hasRenderDevice = HasRenderDevice();
                const bool runningInCi = IsTruthy(g_getenv("CI"));
                const bool shouldUseSoftwareRendering =
                    IsTruthy(forceSoftware) ||
                    (forceSoftware == nullptr && (runningInCi || !hasRenderDevice));

                g_renderingDecision.recorded = true;
                if (IsDisabled(forceSoftware)) {
                    g_renderingDecision.softwareRendering = false;
                    g_renderingDecision.reason = "software rendering explicitly disabled";
                } else if (!shouldUseSoftwareRendering) {
                    g_renderingDecision.softwareRendering = false;
                    g_renderingDecision.reason = "hardware rendering (render device present, not CI, not forced)";
                } else {
                    g_renderingDecision.softwareRendering = true;
                    g_renderingDecision.reason = IsTruthy(forceSoftware)
                        ? "forced by INFINIFRAME_LINUX_FORCE_SOFTWARE_RENDERING"
                        : runningInCi
                        ? "CI environment detected"
                        : "no /dev/dri render device";
                }

                if (IsDisabled(forceSoftware) || !shouldUseSoftwareRendering)
                    return;

                // Deterministic software-rendering defaults. SetDefaultEnvironmentVariable never
                // overrides values already present in the environment, so explicit user/CI settings
                // always win and nothing here leaks into production sessions that configure their
                // own rendering.
                SetDefaultEnvironmentVariable("LIBGL_ALWAYS_SOFTWARE", "1");
                SetDefaultEnvironmentVariable("GALLIUM_DRIVER", "llvmpipe");
                SetDefaultEnvironmentVariable("MESA_LOADER_DRIVER_OVERRIDE", "llvmpipe");
                SetDefaultEnvironmentVariable("MESA_GL_VERSION_OVERRIDE", "3.3");
                SetDefaultEnvironmentVariable("WEBKIT_DISABLE_COMPOSITING_MODE", "1");
                SetDefaultEnvironmentVariable("WEBKIT_DISABLE_DMABUF_RENDERER", "1");
            });
    }

    void LogRenderingDiagnostics() {
        static std::once_flag diagnosticsOnce;
        std::call_once(
            diagnosticsOnce, [] {
                ConfigureGraphicsEnvironment();

                const char* displayBackend = "(none)";
                GdkDisplay* display = gdk_display_get_default();
                if (display != nullptr)
                    displayBackend = G_OBJECT_TYPE_NAME(display);

                g_message(
                    "[InfiniFrame/Linux] rendering diagnostics: gtk=%d.%d.%d webkit=%d.%d.%d display=%s",
                    gtk_get_major_version(), gtk_get_minor_version(), gtk_get_micro_version(),
                    webkit_get_major_version(), webkit_get_minor_version(), webkit_get_micro_version(),
                    displayBackend
                    );

                g_message(
                    "[InfiniFrame/Linux] rendering state: software=%s reason=\"%s\" renderDevice=%s "
                    "compositing=%s dmabufRenderer=%s",
                    g_renderingDecision.softwareRendering ? "yes" : "no",
                    g_renderingDecision.reason,
                    HasRenderDevice() ? "yes" : "no",
                    IsTruthy(g_getenv("WEBKIT_DISABLE_COMPOSITING_MODE")) ? "disabled" : "enabled",
                    IsTruthy(g_getenv("WEBKIT_DISABLE_DMABUF_RENDERER")) ? "disabled" : "enabled"
                    );

                g_message(
                    "[InfiniFrame/Linux] renderer env: LIBGL_ALWAYS_SOFTWARE=%s GALLIUM_DRIVER=%s "
                    "MESA_LOADER_DRIVER_OVERRIDE=%s MESA_GL_VERSION_OVERRIDE=%s",
                    NullToDash(g_getenv("LIBGL_ALWAYS_SOFTWARE")),
                    NullToDash(g_getenv("GALLIUM_DRIVER")),
                    NullToDash(g_getenv("MESA_LOADER_DRIVER_OVERRIDE")),
                    NullToDash(g_getenv("MESA_GL_VERSION_OVERRIDE"))
                    );
            });
    }
}