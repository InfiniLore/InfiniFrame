// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <gio/gio.h>
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// D-Bus paths and interfaces for taskbar integration
static auto statusNotifierItemBusName = "org.kde.StatusNotifierItem";
static auto statusNotifierItemPath = "/StatusNotifierItem";
static auto statusNotifierItemIface = "org.kde.StatusNotifierItem";
static auto launcherEntryBusName = "com.canonical.Unity.LauncherEntry";
static auto launcherEntryPath = "/com/canonical/Unity/LauncherEntry";
static auto launcherEntryIface = "com.canonical.Unity.LauncherEntry";

// Cached D-Bus connections and proxy objects
static GDBusConnection* sessionBus = nullptr;
static GDBusProxy* statusNotifierProxy = nullptr;
static GDBusProxy* launcherEntryProxy = nullptr;
static bool dbusInitialized = false;
static bool hasStatusNotifier = false;
static bool hasLauncherEntry = false;

static void EnsureDBusInitialized() {
    if (dbusInitialized) {
        return;
    }
    dbusInitialized = true;

    GError* error = nullptr;
    sessionBus = g_bus_get_sync(G_BUS_TYPE_SESSION, nullptr, &error);
    if (sessionBus == nullptr || error != nullptr) {
        if (error != nullptr) {
            g_error_free(error);
        }
        return;
    }

    // Try StatusNotifierItem (KDE, Unity, some others)
    statusNotifierProxy = g_dbus_proxy_new_sync(
        sessionBus, G_DBUS_PROXY_FLAGS_NONE, nullptr, statusNotifierItemBusName, statusNotifierItemPath,
        statusNotifierItemIface, nullptr, &error
        );
    if (statusNotifierProxy != nullptr && error == nullptr) {
        hasStatusNotifier = true;
    }
    if (error != nullptr) {
        g_error_free(error);
    }
    error = nullptr;

    // Try Unity LauncherEntry
    launcherEntryProxy = g_dbus_proxy_new_sync(
        sessionBus, G_DBUS_PROXY_FLAGS_NONE, nullptr, launcherEntryBusName, launcherEntryPath, launcherEntryIface,
        nullptr, &error
        );
    if (launcherEntryProxy != nullptr && error == nullptr) {
        hasLauncherEntry = true;
    }
    if (error) {
        g_error_free(error);
    }
}

static double ProgressToFraction(const int state, const uint64_t current, const uint64_t total) {
    if (state == 0) {
        return -1.0; // -1 means "no progress" for LauncherEntry
    }
    if (state == 1) {
        return 0.0; // Indeterminate
    }
    if (total == 0) {
        return 0.0;
    }
    return static_cast<double>(current) / static_cast<double>(total);
}

void InfiniFrameWindow::SetTaskbarProgress(const int state, const uint64_t current, const uint64_t total) {
    EnsureDBusInitialized();

    const double Progress = ProgressToFraction(state, current, total);

    // Unity LauncherEntry
    if (hasLauncherEntry && launcherEntryProxy != nullptr) {
        GVariant* progressVariant = g_variant_new_double(Progress);
        GError* error = nullptr;
        g_dbus_connection_call_sync(
            g_dbus_proxy_get_connection(launcherEntryProxy), launcherEntryBusName, launcherEntryPath,
            "org.freedesktop.DBus.Properties", "Set",
            g_variant_new("(ssv)", launcherEntryIface, "UnityCount", g_variant_new_int32(0)), nullptr,
            G_DBUS_CALL_FLAGS_NONE, -1, nullptr, &error
            );
        g_dbus_connection_call_sync(
            g_dbus_proxy_get_connection(launcherEntryProxy), launcherEntryBusName, launcherEntryPath,
            "org.freedesktop.DBus.Properties", "Set",
            g_variant_new("(ssv)", launcherEntryIface, "UnityProgress", progressVariant), nullptr,
            G_DBUS_CALL_FLAGS_NONE, -1, nullptr, &error
            );
        if (error != nullptr) {
            g_error_free(error);
        }
    }

    // StatusNotifierItem
    if (hasStatusNotifier && statusNotifierProxy != nullptr) {
        auto status = "NeedsAttention";
        if (state == 0) {
            status = "Passive";
        } else if (state == 2) {
            status = "NeedsAttention";
        }

        GError* error = nullptr;
        g_dbus_connection_call_sync(
            g_dbus_proxy_get_connection(statusNotifierProxy), statusNotifierItemBusName, statusNotifierItemPath,
            "org.freedesktop.DBus.Properties", "Set",
            g_variant_new("(ssv)", statusNotifierItemIface, "Status", g_variant_new_string(status)), nullptr,
            G_DBUS_CALL_FLAGS_NONE, -1, nullptr, &error
            );
        if (error != nullptr) {
            g_error_free(error);
        }
    }
}

void InfiniFrameWindow::ClearTaskbarProgress() {
    SetTaskbarProgress(0, 0, 0);
}

void InfiniFrameWindow::SetTaskbarFlash(const int mode, uint32_t) {
    EnsureDBusInitialized();

    if (!hasStatusNotifier || statusNotifierProxy == nullptr) {
        return;
    }

    auto status = "Passive";
    switch (mode) {
        case 0:
            status = "Passive";
            break;
        case 1: // All
        case 2: // Timer
        case 3: // TimerAll
            status = "NeedsAttention";
            break;
        default:
            status = "Passive";
            break;
    }

    GError* error = nullptr;
    g_dbus_connection_call_sync(
        g_dbus_proxy_get_connection(statusNotifierProxy), statusNotifierItemBusName, statusNotifierItemPath,
        "org.freedesktop.DBus.Properties", "Set",
        g_variant_new("(ssv)", statusNotifierItemIface, "Status", g_variant_new_string(status)), nullptr,
        G_DBUS_CALL_FLAGS_NONE, -1, nullptr, &error
        );
    if (error != nullptr) {
        g_error_free(error);
    }
}

void InfiniFrameWindow::StopTaskbarFlash() {
    EnsureDBusInitialized();

    if (!hasStatusNotifier || statusNotifierProxy == nullptr) {
        return;
    }

    GError* error = nullptr;
    g_dbus_connection_call_sync(
        g_dbus_proxy_get_connection(statusNotifierProxy), statusNotifierItemBusName, statusNotifierItemPath,
        "org.freedesktop.DBus.Properties", "Set",
        g_variant_new("(ssv)", statusNotifierItemIface, "Status", g_variant_new_string("Passive")), nullptr,
        G_DBUS_CALL_FLAGS_NONE, -1, nullptr, &error
        );
    if (error != nullptr) {
        g_error_free(error);
    }
}

void InfiniFrameWindow::GetTaskbarProgressSupported(bool* supported) const {
    EnsureDBusInitialized();
    if (supported != nullptr) {
        *supported = hasStatusNotifier || hasLauncherEntry;
    }
}