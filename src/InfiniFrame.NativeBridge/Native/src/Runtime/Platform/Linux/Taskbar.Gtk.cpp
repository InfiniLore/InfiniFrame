// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef __linux__
#include <gio/gio.h>
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

// D-Bus paths and interfaces for taskbar integration
static const char* STATUS_NOTIFIER_ITEM_BUS_NAME = "org.kde.StatusNotifierItem";
static const char* STATUS_NOTIFIER_ITEM_PATH = "/StatusNotifierItem";
static const char* STATUS_NOTIFIER_ITEM_IFACE = "org.kde.StatusNotifierItem";
static const char* LAUNCHER_ENTRY_BUS_NAME = "com.canonical.Unity.LauncherEntry";
static const char* LAUNCHER_ENTRY_PATH = "/com/canonical/Unity/LauncherEntry";
static const char* LAUNCHER_ENTRY_IFACE = "com.canonical.Unity.LauncherEntry";

// Cached D-Bus connections and proxy objects
static GDBusConnection* s_sessionBus = nullptr;
static GDBusProxy* s_statusNotifierProxy = nullptr;
static GDBusProxy* s_launcherEntryProxy = nullptr;
static bool s_dbusInitialized = false;
static bool s_hasStatusNotifier = false;
static bool s_hasLauncherEntry = false;

static void EnsureDBusInitialized() {
    if (s_dbusInitialized) return;
    s_dbusInitialized = true;

    GError* error = nullptr;
    s_sessionBus = g_bus_get_sync(G_BUS_TYPE_SESSION, nullptr, &error);
    if (!s_sessionBus || error) {
        if (error) g_error_free(error);
        return;
    }

    // Try StatusNotifierItem (KDE, Unity, some others)
    s_statusNotifierProxy = g_dbus_proxy_new_sync(
        s_sessionBus,
        G_DBUS_PROXY_FLAGS_NONE,
        nullptr,
        STATUS_NOTIFIER_ITEM_BUS_NAME,
        STATUS_NOTIFIER_ITEM_PATH,
        STATUS_NOTIFIER_ITEM_IFACE,
        nullptr,
        &error
    );
    if (s_statusNotifierProxy && !error) {
        s_hasStatusNotifier = true;
    }
    if (error) g_error_free(error);
    error = nullptr;

    // Try Unity LauncherEntry
    s_launcherEntryProxy = g_dbus_proxy_new_sync(
        s_sessionBus,
        G_DBUS_PROXY_FLAGS_NONE,
        nullptr,
        LAUNCHER_ENTRY_BUS_NAME,
        LAUNCHER_ENTRY_PATH,
        LAUNCHER_ENTRY_IFACE,
        nullptr,
        &error
    );
    if (s_launcherEntryProxy && !error) {
        s_hasLauncherEntry = true;
    }
    if (error) g_error_free(error);
}

static double ProgressToFraction(int state, uint64_t current, uint64_t total) {
    if (state == 0) return -1.0;  // -1 means "no progress" for LauncherEntry
    if (state == 1) return 0.0;   // Indeterminate
    if (total == 0) return 0.0;
    return static_cast<double>(current) / static_cast<double>(total);
}

void InfiniFrameWindow::SetTaskbarProgress(int state, uint64_t current, uint64_t total) {
    EnsureDBusInitialized();

    double progress = ProgressToFraction(state, current, total);

    // Unity LauncherEntry
    if (s_hasLauncherEntry && s_launcherEntryProxy) {
        GVariantBuilder builder;
        g_variant_builder_init(&builder, G_VARIANT_TYPE("a{sv}"));

        GVariant* progressVariant = g_variant_new_double(progress);
        g_variant_builder_add(&builder, "{sv}", "quicklist-count", g_variant_new_int32(0));

        if (progress >= 0.0) {
            g_variant_builder_add(&builder, "{sv}", "progress-visible", g_variant_new_boolean(true));
            g_variant_builder_add(&builder, "{sv}", "progress", progressVariant);
        } else {
            g_variant_builder_add(&builder, "{sv}", "progress-visible", g_variant_new_boolean(false));
        }

        GVariant* parameters = g_variant_builder_end(&builder);
        GError* error = nullptr;
        g_dbus_connection_call_sync(
            g_dbus_proxy_get_connection(s_launcherEntryProxy),
            LAUNCHER_ENTRY_BUS_NAME,
            LAUNCHER_ENTRY_PATH,
            "org.freedesktop.DBus.Properties",
            "Set",
            g_variant_new("(ssv)", LAUNCHER_ENTRY_IFACE, "UnityCount", g_variant_new_int32(0)),
            nullptr,
            G_DBUS_CALL_FLAGS_NONE,
            -1,
            nullptr,
            &error
        );
        // Also set the progress properties
        g_dbus_connection_call_sync(
            g_dbus_proxy_get_connection(s_launcherEntryProxy),
            LAUNCHER_ENTRY_BUS_NAME,
            LAUNCHER_ENTRY_PATH,
            "org.freedesktop.DBus.Properties",
            "Set",
            g_variant_new("(ssv)", LAUNCHER_ENTRY_IFACE, "UnityProgress", progressVariant),
            nullptr,
            G_DBUS_CALL_FLAGS_NONE,
            -1,
            nullptr,
            &error
        );
        if (error) g_error_free(error);
    }

    // StatusNotifierItem
    if (s_hasStatusNotifier && s_statusNotifierProxy) {
        const char* status = "NeedsAttention";
        if (state == 0) status = "Passive";
        else if (state == 2) status = "NeedsAttention";

        GError* error = nullptr;
        g_dbus_connection_call_sync(
            g_dbus_proxy_get_connection(s_statusNotifierProxy),
            STATUS_NOTIFIER_ITEM_BUS_NAME,
            STATUS_NOTIFIER_ITEM_PATH,
            "org.freedesktop.DBus.Properties",
            "Set",
            g_variant_new("(ssv)", STATUS_NOTIFIER_ITEM_IFACE, "Status", g_variant_new_string(status)),
            nullptr,
            G_DBUS_CALL_FLAGS_NONE,
            -1,
            nullptr,
            &error
        );
        if (error) g_error_free(error);
    }
}

void InfiniFrameWindow::ClearTaskbarProgress() {
    SetTaskbarProgress(0, 0, 0);
}

void InfiniFrameWindow::SetTaskbarFlash(int mode, uint32_t count) {
    EnsureDBusInitialized();

    if (!s_hasStatusNotifier || !s_statusNotifierProxy) return;

    const char* status = "Passive";
    switch (mode) {
        case 0: status = "Passive"; break;
        case 1: // All
        case 2: // Timer
        case 3: // TimerAll
            status = "NeedsAttention";
            break;
        default: status = "Passive"; break;
    }

    GError* error = nullptr;
    g_dbus_connection_call_sync(
        g_dbus_proxy_get_connection(s_statusNotifierProxy),
        STATUS_NOTIFIER_ITEM_BUS_NAME,
        STATUS_NOTIFIER_ITEM_PATH,
        "org.freedesktop.DBus.Properties",
        "Set",
        g_variant_new("(ssv)", STATUS_NOTIFIER_ITEM_IFACE, "Status", g_variant_new_string(status)),
        nullptr,
        G_DBUS_CALL_FLAGS_NONE,
        -1,
        nullptr,
        &error
    );
    if (error) g_error_free(error);
}

void InfiniFrameWindow::StopTaskbarFlash() {
    EnsureDBusInitialized();

    if (!s_hasStatusNotifier || !s_statusNotifierProxy) return;

    GError* error = nullptr;
    g_dbus_connection_call_sync(
        g_dbus_proxy_get_connection(s_statusNotifierProxy),
        STATUS_NOTIFIER_ITEM_BUS_NAME,
        STATUS_NOTIFIER_ITEM_PATH,
        "org.freedesktop.DBus.Properties",
        "Set",
        g_variant_new("(ssv)", STATUS_NOTIFIER_ITEM_IFACE, "Status", g_variant_new_string("Passive")),
        nullptr,
        G_DBUS_CALL_FLAGS_NONE,
        -1,
        nullptr,
        &error
    );
    if (error) g_error_free(error);
}

void InfiniFrameWindow::GetTaskbarProgressSupported(bool* supported) const {
    EnsureDBusInitialized();
    if (supported) *supported = s_hasStatusNotifier || s_hasLauncherEntry;
}

#endif
