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
            g_object_set_data(G_OBJECT(menuItem), "infiniframe-command-id", GUINT_TO_POINTER(commandId));

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

    GtkWidget* FindMenuItemWidget(GtkWidget* menuBar, const std::unordered_map<std::string, guint>& idToCommand, const char* menuItemId) {
        auto it = idToCommand.find(menuItemId);
        if (it == idToCommand.end())
            return nullptr;

        guint targetId = it->second;

        GList* topItems = gtk_container_get_children(GTK_CONTAINER(menuBar));
        for (GList* t = topItems; t != nullptr; t = t->next) {
            GtkWidget* topItem = GTK_WIDGET(t->data);

            // Check if this top-level item itself is the target (leaf menu item, not a submenu).
            gpointer topCmdData = g_object_get_data(G_OBJECT(topItem), "infiniframe-command-id");
            if (topCmdData != nullptr && GPOINTER_TO_UINT(topCmdData) == targetId) {
                g_list_free(topItems);
                return topItem;
            }

            GtkWidget* subMenu = gtk_menu_item_get_submenu(GTK_MENU_ITEM(topItem));
            if (subMenu == nullptr) continue;

            GList* subItems = gtk_container_get_children(GTK_CONTAINER(subMenu));
            for (GList* s = subItems; s != nullptr; s = s->next) {
                GtkWidget* subItem = GTK_WIDGET(s->data);

                gpointer subCmdData = g_object_get_data(G_OBJECT(subItem), "infiniframe-command-id");
                if (subCmdData != nullptr && GPOINTER_TO_UINT(subCmdData) == targetId) {
                    g_list_free(subItems);
                    g_list_free(topItems);
                    return subItem;
                }

                GtkWidget* nestedMenu = gtk_menu_item_get_submenu(GTK_MENU_ITEM(subItem));
                if (nestedMenu != nullptr) {
                    GList* nestedItems = gtk_container_get_children(GTK_CONTAINER(nestedMenu));
                    for (GList* n = nestedItems; n != nullptr; n = n->next) {
                        GtkWidget* nestedItem = GTK_WIDGET(n->data);

                        gpointer nestedCmdData = g_object_get_data(G_OBJECT(nestedItem), "infiniframe-command-id");
                        if (nestedCmdData != nullptr && GPOINTER_TO_UINT(nestedCmdData) == targetId) {
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

        // If the window already has children (i.e., Show() was already called),
        // attach the menu bar to the existing widget tree.
        // If not, the menu bar will be attached when Show() runs (via the existing
        // ApplyInitMenuBar call in WindowCore.Gtk.cpp before Show()).
        GtkWidget* existingChild = gtk_bin_get_child(GTK_BIN(impl->_window));
        if (existingChild != nullptr) {
            // Window already has children — create a GtkBox, reparent the existing child,
            // and add the menu bar at the top.
            GtkWidget* box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);
            gtk_box_pack_start(GTK_BOX(box), menuBar, FALSE, FALSE, 0);
            gtk_container_remove(GTK_CONTAINER(impl->_window), existingChild);
            gtk_box_pack_start(GTK_BOX(box), existingChild, TRUE, TRUE, 0);
            gtk_container_add(GTK_CONTAINER(impl->_window), box);
            gtk_widget_show_all(impl->_window);
        }
        // If existingChild is nullptr, the menu bar is stored and will be attached
        // later when Show() is called (the webview will be added to the window, and
        // then this function is called again via SetMenuBarJson).
    } catch (const simdjson::simdjson_error&) {
    }
}

void InfiniFrameWindow::SetMenuBarJson(const char* menuBarJson) {
    ApplyInitMenuBar(menuBarJson);

    // If the window already has children (webview exists), attach the menu bar now.
    // Otherwise, it will be attached when Show() runs and this is called again.
    auto* impl = static_cast<Impl*>(ImplBase());
    if (impl->_menuBar != nullptr && impl->_webview != nullptr) {
        GtkWidget* existingChild = gtk_bin_get_child(GTK_BIN(impl->_window));
        if (existingChild != nullptr) {
            GtkWidget* box = gtk_box_new(GTK_ORIENTATION_VERTICAL, 0);
            gtk_box_pack_start(GTK_BOX(box), impl->_menuBar, FALSE, FALSE, 0);
            gtk_container_remove(GTK_CONTAINER(impl->_window), existingChild);
            gtk_box_pack_start(GTK_BOX(box), existingChild, TRUE, TRUE, 0);
            gtk_container_add(GTK_CONTAINER(impl->_window), box);
            gtk_widget_show_all(impl->_window);
        }
    }
}

void InfiniFrameWindow::SetMenuItemEnabledById(const char* menuItemId, const bool enabled) {
    auto* impl = static_cast<Impl*>(ImplBase());

    if (impl->_menuBar == nullptr)
        return;

    GtkWidget* widget = FindMenuItemWidget(impl->_menuBar, impl->_menuItemIdToCommandId, menuItemId);
    if (widget != nullptr) {
        gtk_widget_set_sensitive(widget, enabled ? TRUE : FALSE);
    }
}

void InfiniFrameWindow::SetMenuItemVisibleById(const char* menuItemId, const bool visible) {
    auto* impl = static_cast<Impl*>(ImplBase());

    if (impl->_menuBar == nullptr)
        return;

    GtkWidget* widget = FindMenuItemWidget(impl->_menuBar, impl->_menuItemIdToCommandId, menuItemId);
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
