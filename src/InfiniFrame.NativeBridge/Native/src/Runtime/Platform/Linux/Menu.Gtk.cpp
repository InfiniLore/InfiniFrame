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
namespace {
    struct MenuActivateData {
        InfiniFrameWindow* window;
        std::string itemId;
    };

    void DestroyMenuActivateData(InfiniFrameWindow::Impl* impl) {
        for (void* ptr : impl->_menuActivateDataList) {
            delete static_cast<MenuActivateData*>(ptr);
        }
        impl->_menuActivateDataList.clear();
    }

    void onMenuActivate(GtkMenuItem* /*menuItem*/, const gpointer userData) {
        auto* data = static_cast<MenuActivateData*>(userData);
        if (data == nullptr || data->window == nullptr) return;

        std::string message = std::string("menu:") + data->itemId;
        data->window->SendWebMessage(message.c_str());
    }

    void BuildMenuFromJson(
        GtkWidget* parentMenu,
        const simdjson::dom::array& items,
        std::unordered_map<std::string, guint>& idToCommand,
        std::unordered_map<guint, std::string>& commandToId,
        guint& nextId,
        std::vector<void*>& activateDataList
    ) {
        for (const auto& item : items) {
            simdjson::dom::object obj;
            if (item.get(obj) != simdjson::SUCCESS) continue;

            std::string id;
            if (obj["id"].get_string().get(id) != simdjson::SUCCESS)
                continue;

            int64_t type = 0;
            (void)obj["type"].get_int64().get(type);

            bool isEnabled = true;
            (void)obj["isEnabled"].get_bool().get(isEnabled);

            bool isVisible = true;
            (void)obj["isVisible"].get_bool().get(isVisible);

            if (!isVisible)
                continue;

            if (type == 1) {
                GtkWidget* separator = gtk_separator_menu_item_new();
                gtk_menu_shell_append(GTK_MENU_SHELL(parentMenu), separator);
                continue;
            }

            std::string label;
            (void)obj["label"].get_string().get(label);

            guint commandId = nextId++;
            idToCommand[id] = commandId;
            commandToId[commandId] = id;

            GtkWidget* menuItem = gtk_menu_item_new_with_label(label.c_str());

            // Store the item ID as GObject data for later lookup in FindMenuItemWidget.
            g_object_set_data_full(G_OBJECT(menuItem), "infiniframe-item-id", strdup(id.c_str()), g_free);

            if (type == 2) {
                GtkWidget* subMenu = gtk_menu_new();
                gtk_menu_item_set_submenu(GTK_MENU_ITEM(menuItem), subMenu);
                gtk_menu_shell_append(GTK_MENU_SHELL(parentMenu), menuItem);

                simdjson::dom::array children;
                if (obj["children"].get_array().get(children) == simdjson::SUCCESS) {
                    BuildMenuFromJson(subMenu, children, idToCommand, commandToId, nextId, activateDataList);
                }
            } else {
                gtk_menu_shell_append(GTK_MENU_SHELL(parentMenu), menuItem);
            }

            if (!isEnabled) {
                gtk_widget_set_sensitive(menuItem, FALSE);
            }

            auto* data = new MenuActivateData{nullptr, id};
            activateDataList.push_back(data);
            g_signal_connect(menuItem, "activate", G_CALLBACK(onMenuActivate), data);
        }
    }

    GtkWidget* FindMenuItemWidget(GtkWidget* menuBar, const char* menuItemId) {
        GList* topItems = gtk_container_get_children(GTK_CONTAINER(menuBar));
        for (GList* t = topItems; t != nullptr; t = t->next) {
            GtkWidget* topItem = GTK_WIDGET(t->data);

            GtkWidget* subMenu = gtk_menu_item_get_submenu(GTK_MENU_ITEM(topItem));
            if (subMenu == nullptr) continue;

            GList* subItems = gtk_container_get_children(GTK_CONTAINER(subMenu));
            for (GList* s = subItems; s != nullptr; s = s->next) {
                GtkWidget* subItem = GTK_WIDGET(s->data);

                const char* itemId = static_cast<const char*>(g_object_get_data(G_OBJECT(subItem), "infiniframe-item-id"));
                if (itemId != nullptr && strcmp(itemId, menuItemId) == 0) {
                    g_list_free(subItems);
                    g_list_free(topItems);
                    return subItem;
                }

                GtkWidget* nestedMenu = gtk_menu_item_get_submenu(GTK_MENU_ITEM(subItem));
                if (nestedMenu != nullptr) {
                    GList* nestedItems = gtk_container_get_children(GTK_CONTAINER(nestedMenu));
                    for (GList* n = nestedItems; n != nullptr; n = n->next) {
                        GtkWidget* nestedItem = GTK_WIDGET(n->data);
                        const char* nestedId = static_cast<const char*>(g_object_get_data(G_OBJECT(nestedItem), "infiniframe-item-id"));
                        if (nestedId != nullptr && strcmp(nestedId, menuItemId) == 0) {
                            g_list_free(nestedItems);
                            g_list_free(subItems);
                            g_list_free(topItems);
                            return nestedItem;
                        }
                    }
                    g_list_free(nestedItems);
                }
            }
            g_list_free(subItems);
        }
        g_list_free(topItems);
        return nullptr;
    }
}

void InfiniFrameWindow::ApplyInitMenuBar(const char* menuBarJson) {
    if (menuBarJson == nullptr || menuBarJson[0] == '\0')
        return;

    auto* impl = static_cast<Impl*>(ImplBase());

    if (impl->_menuBar != nullptr) {
        DestroyMenuActivateData(impl);
        gtk_widget_destroy(impl->_menuBar);
        impl->_menuBar = nullptr;
        impl->_menuItemIdToCommandId.clear();
        impl->_menuCommandIdToItemId.clear();
        impl->_nextMenuCommandId = 1;
    }

    try {
        simdjson::dom::parser parser;
        std::string jsonInput(menuBarJson);
        simdjson::padded_string padded(jsonInput);
        simdjson::dom::element root = parser.parse(padded);

        simdjson::dom::array items;
        if (root["items"].get_array().get(items) != simdjson::SUCCESS)
            return;

        GtkWidget* menuBar = gtk_menu_bar_new();

        BuildMenuFromJson(
            menuBar, items,
            impl->_menuItemIdToCommandId,
            impl->_menuCommandIdToItemId,
            impl->_nextMenuCommandId,
            impl->_menuActivateDataList
        );

        impl->_menuBar = menuBar;
        impl->_menuBarJson = menuBarJson;

        // The menu bar is stored and will be attached when Show() is called.
        // If Show() has already been called, we need to restructure the widget tree.
        // For now, the menu bar is applied during Show() via AttachMenuBar().
    } catch (const simdjson::simdjson_error&) {
    }
}

void InfiniFrameWindow::AttachMenuBar() {
    auto* impl = static_cast<Impl*>(ImplBase());

    if (impl->_menuBar == nullptr)
        return;

    // GtkWindow is a GtkBin — it can only hold ONE child.
    // Strategy: remove the existing child (webview), create a GtkBox,
    // pack menu bar + webview into it, then add the box to the window.
    GtkWidget* existingChild = gtk_bin_get_child(GTK_BIN(impl->_window));

    GtkWidget* box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);

    // Add menu bar at the top.
    gtk_box_pack_start(GTK_BOX(box), impl->_menuBar, FALSE, FALSE, 0);

    // Reparent the existing child (webview) into the box.
    if (existingChild != nullptr) {
        gtk_container_remove(GTK_CONTAINER(impl->_window), existingChild);
        gtk_box_pack_start(GTK_BOX(box), existingChild, TRUE, TRUE, 0);
    }

    // Add the box to the window.
    gtk_container_add(GTK_CONTAINER(impl->_window), box);
    gtk_widget_show_all(impl->_window);
}

void InfiniFrameWindow::SetMenuBarJson(const char* menuBarJson) {
    ApplyInitMenuBar(menuBarJson);

    // If the window is already shown, restructure the widget tree.
    auto* impl = static_cast<Impl*>(ImplBase());
    if (impl->_webview != nullptr) {
        AttachMenuBar();
    }
}

void InfiniFrameWindow::SetMenuItemEnabledById(const char* menuItemId, const bool enabled) {
    auto* impl = static_cast<Impl*>(ImplBase());

    if (impl->_menuBar == nullptr)
        return;

    GtkWidget* widget = FindMenuItemWidget(impl->_menuBar, menuItemId);
    if (widget != nullptr) {
        gtk_widget_set_sensitive(widget, enabled ? TRUE : FALSE);
    }
}

void InfiniFrameWindow::SetMenuItemVisibleById(const char* menuItemId, const bool visible) {
    auto* impl = static_cast<Impl*>(ImplBase());

    if (impl->_menuBar == nullptr)
        return;

    GtkWidget* widget = FindMenuItemWidget(impl->_menuBar, menuItemId);
    if (widget != nullptr) {
        if (visible) {
            gtk_widget_show(widget);
        } else {
            gtk_widget_hide(widget);
        }
    }
}

void InfiniFrameWindow::ClickMenuItemById(const char* menuItemId) {
    if (m_impl->_webMessageReceivedCallback == nullptr)
        return;

    std::string message = std::string("menu:") + menuItemId;
    SendWebMessage(message.c_str());
}
