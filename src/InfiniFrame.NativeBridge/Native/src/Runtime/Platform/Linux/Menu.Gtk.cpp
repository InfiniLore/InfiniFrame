// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <unordered_map>

#include <gtk/gtk.h>
#ifdef _MSC_VER
#pragma warning(push)
#pragma warning(disable: 4100 4244)
#endif
#include <simdjson.h>
#ifdef _MSC_VER
#pragma warning(pop)
#endif

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
// GTK4 removed GtkMenu/GtkMenuItem/GtkMenuBar. Dropdowns are model-based:
// GMenu + GSimpleAction (group "menu") rendered by GtkPopoverMenu popovers. The menu
// bar itself is a horizontal GtkBox row of flat buttons - GtkMenuButton per top-level
// submenu, GtkButton per top-level command item - because GtkPopoverMenuBar accepts
// only submenus as toplevel elements and silently drops command items with
// "Don't know how to handle this item" (gtkpopovermenubar.c tracker_insert).
// Enabled state lives on the actions; menu-bar buttons activate the item's GAction,
// so a disabled action ignores activation (GSimpleAction semantics). Visibility
// changes rebuild the row.
namespace {
    struct MenuActivateData {
        InfiniFrameWindow* window;
        std::string itemId;
        GSimpleAction* action = nullptr; // non-owning; menu-bar buttons activate this
    };

    void DestroyMenuActivateData(InfiniFrameWindow::Impl* impl) {
        for (void* ptr : impl->_menuActivateDataList) {
            delete static_cast<MenuActivateData*>(ptr);
        }
        impl->_menuActivateDataList.clear();
    }

    void onMenuActivate(GSimpleAction* /*action*/, GVariant* /*parameter*/, const gpointer userData) {
        auto* data = static_cast<MenuActivateData*>(userData);
        if (data == nullptr || data->window == nullptr)
            return;

        std::string message = std::string("menu:") + data->itemId;
        data->window->SendWebMessage(message.c_str());
    }

    void onMenuButtonClicked(GtkButton* /*button*/, const gpointer userData) {
        auto* data = static_cast<MenuActivateData*>(userData);
        if (data == nullptr || data->action == nullptr)
            return;
        // Route the click through the item's GAction so the enabled state stays
        // authoritative: activating a disabled GSimpleAction is a no-op.
        g_action_activate(G_ACTION(data->action), nullptr);
    }

    // Registers command id, activation data, and the "item-N" GAction for one menu
    // item and returns the action name used by popover menu items and buttons.
    std::string RegisterMenuAction(
        const std::string& id,
        const bool isEnabled,
        InfiniFrameWindow* window,
        InfiniFrameWindow::Impl* impl,
        GSimpleActionGroup* actions
        ) {
        const guint commandId = impl->_nextMenuCommandId++;
        impl->_menuItemIdToCommandId[id] = commandId;
        impl->_menuCommandIdToItemId[commandId] = id;

        const std::string actionSuffix = "item-" + std::to_string(commandId);
        GSimpleAction* action = g_simple_action_new(actionSuffix.c_str(), nullptr);
        g_simple_action_set_enabled(action, isEnabled ? TRUE : FALSE);

        auto* data = new MenuActivateData{window, id, action};
        impl->_menuActivateDataList.push_back(data);
        g_signal_connect(action, "activate", G_CALLBACK(onMenuActivate), data);
        g_action_map_add_action(G_ACTION_MAP(actions), G_ACTION(action));
        g_object_unref(action);
        return actionSuffix;
    }

    // Applies runtime enabled/visible overrides to an item's JSON defaults.
    void ResolveItemState(
        InfiniFrameWindow::Impl* impl,
        const std::string& id,
        bool& isEnabled,
        bool& isVisible
        ) {
        if (const auto enabledIt = impl->_menuEnabledOverrides.find(id);
            enabledIt != impl->_menuEnabledOverrides.end()) {
            isEnabled = enabledIt->second;
        }
        if (const auto visibleIt = impl->_menuVisibleOverrides.find(id);
            visibleIt != impl->_menuVisibleOverrides.end()) {
            isVisible = visibleIt->second;
        }
    }

    bool GetBoolOr(const simdjson::dom::object& obj, const char* key, const bool fallback) {
        bool value = fallback;
        if (obj[key].get_bool().get(value) != simdjson::SUCCESS)
            return fallback;
        return value;
    }

    void BuildMenuModel(
        const simdjson::dom::array& items,
        InfiniFrameWindow* window,
        InfiniFrameWindow::Impl* impl,
        GSimpleActionGroup* actions,
        GMenu* target
        ) {
        // Consecutive visible items form one GMenu section; separators close the current
        // section so the popover renders a divider between groups (and submenus likewise).
        GMenu* section = g_menu_new();

        for (const auto& item : items) {
            simdjson::dom::object obj;
            if (item.get(obj) != simdjson::SUCCESS)
                continue;

            std::string id;
            if (obj["id"].get_string().get(id) != simdjson::SUCCESS)
                continue;

            int64_t type = 0;
            const simdjson::error_code typeError = obj["type"].get_int64().get(type);
            if (typeError != simdjson::SUCCESS)
                type = 0;

            bool isEnabled = GetBoolOr(obj, "isEnabled", true);
            bool isVisible = GetBoolOr(obj, "isVisible", true);

            // Runtime overrides applied by SetMenuItemEnabledById/SetMenuItemVisibleById.
            ResolveItemState(impl, id, isEnabled, isVisible);

            if (!isVisible)
                continue;

            if (type == 1) {
                if (g_menu_model_get_n_items(G_MENU_MODEL(section)) > 0) {
                    g_menu_append_section(target, nullptr, G_MENU_MODEL(section));
                    g_object_unref(section);
                    section = g_menu_new();
                }
                continue;
            }

            std::string label;
            const simdjson::error_code labelError = obj["label"].get_string().get(label);
            if (labelError != simdjson::SUCCESS)
                label.clear();

            const std::string actionSuffix = RegisterMenuAction(id, isEnabled, window, impl, actions);

            if (type == 2) {
                GMenu* subMenu = g_menu_new();
                simdjson::dom::array children;
                if (obj["children"].get_array().get(children) == simdjson::SUCCESS) {
                    BuildMenuModel(children, window, impl, actions, subMenu);
                }
                GMenuItem* subItem = g_menu_item_new_submenu(label.c_str(), G_MENU_MODEL(subMenu));
                g_menu_append_item(section, subItem);
                g_object_unref(subItem);
                g_object_unref(subMenu);
            } else {
                GMenuItem* menuItem = g_menu_item_new(label.c_str(), ("menu." + actionSuffix).c_str());
                g_menu_append_item(section, menuItem);
                g_object_unref(menuItem);
            }
        }

        if (g_menu_model_get_n_items(G_MENU_MODEL(section)) > 0)
            g_menu_append_section(target, nullptr, G_MENU_MODEL(section));
        g_object_unref(section);
    }

    GtkWidget* AppendMenuBarCommandButton(
        GtkWidget* row,
        const std::string& label,
        MenuActivateData* data,
        const bool isEnabled
        ) {
        GtkWidget* button = gtk_button_new_with_label(label.c_str());
        gtk_widget_add_css_class(button, "flat");
        gtk_widget_set_sensitive(button, isEnabled ? TRUE : FALSE);
        g_signal_connect(button, "clicked", G_CALLBACK(onMenuButtonClicked), data);
        gtk_box_append(GTK_BOX(row), button);
        return button;
    }

    // Builds the top-level menu-bar row: a flat GtkMenuButton per top-level submenu
    // (popover from the item's GMenu) and a flat GtkButton per top-level command item.
    void BuildMenuBarRow(
        const simdjson::dom::array& items,
        InfiniFrameWindow* window,
        InfiniFrameWindow::Impl* impl,
        GSimpleActionGroup* actions,
        GtkWidget* row
        ) {
        for (const auto& item : items) {
            simdjson::dom::object obj;
            if (item.get(obj) != simdjson::SUCCESS)
                continue;

            std::string id;
            if (obj["id"].get_string().get(id) != simdjson::SUCCESS)
                continue;

            int64_t type = 0;
            if (obj["type"].get_int64().get(type) != simdjson::SUCCESS)
                type = 0;

            bool isEnabled = GetBoolOr(obj, "isEnabled", true);
            bool isVisible = GetBoolOr(obj, "isVisible", true);
            ResolveItemState(impl, id, isEnabled, isVisible);
            if (!isVisible)
                continue;
            if (type == 1)
                continue; // separators have no meaning in the top-level row

            std::string label;
            if (obj["label"].get_string().get(label) != simdjson::SUCCESS)
                label.clear();

            RegisterMenuAction(id, isEnabled, window, impl, actions);
            // RegisterMenuAction appended this item's activation data last; reuse it
            // to route button clicks through the item's GAction.
            auto* data = static_cast<MenuActivateData*>(impl->_menuActivateDataList.back());

            if (type == 2) {
                GMenu* subMenu = g_menu_new();
                simdjson::dom::array children;
                if (obj["children"].get_array().get(children) == simdjson::SUCCESS)
                    BuildMenuModel(children, window, impl, actions, subMenu);

                if (g_menu_model_get_n_items(G_MENU_MODEL(subMenu)) > 0) {
                    GtkWidget* button = gtk_menu_button_new();
                    gtk_menu_button_set_label(GTK_MENU_BUTTON(button), label.c_str());
                    gtk_widget_add_css_class(button, "flat");
                    // The popover keeps its own model reference; ours is released below.
                    GtkWidget* popover = gtk_popover_menu_new_from_model_full(
                        G_MENU_MODEL(subMenu), GTK_POPOVER_MENU_NESTED);
                    gtk_menu_button_set_popover(GTK_MENU_BUTTON(button), popover);
                    gtk_widget_set_sensitive(button, isEnabled ? TRUE : FALSE);
                    gtk_box_append(GTK_BOX(row), button);
                    impl->_menuWidgetById[id] = button;
                } else {
                    // No visible children: an empty dropdown is useless - render the
                    // command action as a plain button instead.
                    impl->_menuWidgetById[id] = AppendMenuBarCommandButton(row, label, data, isEnabled);
                }
                g_object_unref(subMenu);
            } else {
                impl->_menuWidgetById[id] = AppendMenuBarCommandButton(row, label, data, isEnabled);
            }
        }
    }

    void RebuildMenuModel(InfiniFrameWindow::Impl* impl, InfiniFrameWindow* window) {
        // Remove the previous row's widgets first so no button click handler can reach
        // activation data that is about to be deleted.
        if (impl->_menuBar != nullptr) {
            GtkWidget* rowChild;
            while ((rowChild = gtk_widget_get_first_child(impl->_menuBar)) != nullptr)
                gtk_box_remove(GTK_BOX(impl->_menuBar), rowChild);
        }
        impl->_menuWidgetById.clear();

        DestroyMenuActivateData(impl);
        impl->_menuItemIdToCommandId.clear();
        impl->_menuCommandIdToItemId.clear();
        impl->_nextMenuCommandId = 1;

        if (impl->_menuActions != nullptr) {
            gtk_widget_insert_action_group(GTK_WIDGET(impl->_window), "menu", nullptr);
            g_object_unref(impl->_menuActions);
            impl->_menuActions = nullptr;
        }

        try {
            simdjson::dom::parser parser;
            std::string jsonInput(impl->_menuBarJson);
            simdjson::padded_string padded(jsonInput);
            simdjson::dom::element root = parser.parse(padded);

            simdjson::dom::array items;
            if (root["items"].get_array().get(items) != simdjson::SUCCESS)
                return;

            impl->_menuActions = g_simple_action_group_new();

            if (impl->_menuBar == nullptr)
                impl->_menuBar = gtk_box_new(GTK_ORIENTATION_HORIZONTAL, 0);

            BuildMenuBarRow(items, window, impl, impl->_menuActions, impl->_menuBar);

            gtk_widget_insert_action_group(
                GTK_WIDGET(impl->_window), "menu", G_ACTION_GROUP(impl->_menuActions));
        } catch (const simdjson::simdjson_error&) {}
    }
}

void InfiniFrameWindow::Impl::ReleaseMenuResources() {
    // Release menu-owned GObjects exactly once at window destruction (migration requirement):
    // activation data, the action group (and its widget insertion), and the id maps.
    // The menu-bar row widget and its buttons (_menuBar/_menuBox) are released by the
    // GTK widget hierarchy when the window is destroyed.
    DestroyMenuActivateData(this);
    _menuItemIdToCommandId.clear();
    _menuCommandIdToItemId.clear();
    _menuEnabledOverrides.clear();
    _menuVisibleOverrides.clear();
    _menuWidgetById.clear();
    if (_window != nullptr)
        gtk_widget_insert_action_group(GTK_WIDGET(_window), "menu", nullptr);
    if (_menuActions != nullptr) {
        g_object_unref(_menuActions);
        _menuActions = nullptr;
    }
    _menuBar = nullptr;
    _menuBox = nullptr;
}

void InfiniFrameWindow::Impl::RelayoutWindow() {
    if (_window == nullptr)
        return;

    GtkWidget* webview = _webview;

    // Idempotent fast path: the layout is already correct. Re-running the reparenting
    // dance would unmap/remap the webview and re-set the same window child for no reason.
    if (_menuBar != nullptr && _menuBox != nullptr &&
        gtk_window_get_child(GTK_WINDOW(_window)) == _menuBox &&
        gtk_widget_get_parent(_menuBar) == _menuBox &&
        (webview == nullptr || gtk_widget_get_parent(webview) == _menuBox)) {
        return;
    }
    if (_menuBar == nullptr && webview != nullptr && gtk_window_get_child(GTK_WINDOW(_window)) == webview)
        return;

    // Detach the webview from its previous container (its construction reference keeps
    // it alive across reparenting).
    GtkWidget* webviewParent = webview != nullptr ? gtk_widget_get_parent(webview) : nullptr;
    if (webviewParent == _window) {
        g_object_ref(webview);
        gtk_window_set_child(GTK_WINDOW(_window), nullptr);
        g_object_unref(webview);
    } else if (webviewParent != nullptr && GTK_IS_BOX(webviewParent)) {
        g_object_ref(webview);
        gtk_box_remove(GTK_BOX(webviewParent), webview);
        g_object_unref(webview);
    }

    if (_menuBar == nullptr) {
        gtk_window_set_child(GTK_WINDOW(_window), webview);
        return;
    }

    if (_menuBox == nullptr)
        _menuBox = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);

    GtkWidget* menuParent = gtk_widget_get_parent(_menuBar);
    if (menuParent != _menuBox) {
        if (menuParent != nullptr && GTK_IS_BOX(menuParent)) {
            g_object_ref(_menuBar);
            gtk_box_remove(GTK_BOX(menuParent), _menuBar);
            g_object_unref(_menuBar);
        }
        gtk_box_prepend(GTK_BOX(_menuBox), _menuBar);
    }
    if (webview != nullptr)
        gtk_box_append(GTK_BOX(_menuBox), webview);

    gtk_window_set_child(GTK_WINDOW(_window), _menuBox);
}

void InfiniFrameWindow::ApplyInitMenuBar(const char* menuBarJson) {
    if (menuBarJson == nullptr || menuBarJson[0] == '\0')
        return;

    auto* impl = m_impl.get();

    // A new menu definition replaces all state, including runtime overrides.
    DestroyMenuActivateData(impl);
    impl->_menuItemIdToCommandId.clear();
    impl->_menuCommandIdToItemId.clear();
    impl->_menuEnabledOverrides.clear();
    impl->_menuVisibleOverrides.clear();
    impl->_nextMenuCommandId = 1;
    impl->_menuBarJson = menuBarJson;

    RebuildMenuModel(impl, this);
    impl->RelayoutWindow();
    gtk_widget_set_visible(GTK_WIDGET(impl->_window), TRUE);
}

void InfiniFrameWindow::SetMenuBarJson(const char* menuBarJson) {
    ApplyInitMenuBar(menuBarJson);
}

void InfiniFrameWindow::SetMenuItemEnabledById(const char* menuItemId, const bool enabled) {
    auto* impl = m_impl.get();

    if (impl->_menuBar == nullptr)
        return;

    impl->_menuEnabledOverrides[menuItemId] = enabled;

    // Enabled state is an action property in the GTK4 menu model; no rebuild needed.
    const auto it = impl->_menuItemIdToCommandId.find(menuItemId);
    if (it != impl->_menuItemIdToCommandId.end() && impl->_menuActions != nullptr) {
        const std::string actionName = "item-" + std::to_string(it->second);
        GAction* action = g_action_map_lookup_action(G_ACTION_MAP(impl->_menuActions), actionName.c_str());
        if (action != nullptr)
            g_simple_action_set_enabled(G_SIMPLE_ACTION(action), enabled ? TRUE : FALSE);
    }

    // Menu-bar buttons are not action-aware widgets; sync their sensitivity directly
    // (clicks are still gated by the action for correctness).
    const auto widgetIt = impl->_menuWidgetById.find(menuItemId);
    if (widgetIt != impl->_menuWidgetById.end() && widgetIt->second != nullptr)
        gtk_widget_set_sensitive(widgetIt->second, enabled ? TRUE : FALSE);
}

void InfiniFrameWindow::SetMenuItemVisibleById(const char* menuItemId, const bool visible) {
    auto* impl = m_impl.get();

    if (impl->_menuBar == nullptr)
        return;

    impl->_menuVisibleOverrides[menuItemId] = visible;

    // GTK4 menu models have no per-item visibility; rebuild the model with the override.
    // The widget layout is unchanged, so no relayout is required.
    RebuildMenuModel(impl, this);
}

void InfiniFrameWindow::ClickMenuItemById(const char* menuItemId) {
    if (m_impl->common._webMessageReceivedCallback == nullptr)
        return;

    std::string message = std::string("menu:") + menuItemId;
    SendWebMessage(message.c_str());
}
