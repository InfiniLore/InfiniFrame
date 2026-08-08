// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <unordered_map>

#include <windows.h>
#ifdef _MSC_VER
#pragma warning(push)
#pragma warning(disable: 4100 4244)
#endif
#include <simdjson.h>
#ifdef _MSC_VER
#pragma warning(pop)
#endif

#include "Runtime/Platform/Windows/Window.Win32.Context.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    void DestroyMenuRecursive(HMENU menu) {
        if (menu == nullptr) return;
        int count = GetMenuItemCount(menu);
        for (int i = 0; i < count; i++) {
            HMENU sub = GetSubMenu(menu, i);
            if (sub != nullptr)
                DestroyMenuRecursive(sub);
        }
        DestroyMenu(menu);
    }

    void BuildMenuFromJson(
        HMENU parentMenu,
        const simdjson::dom::array& items,
        std::unordered_map<std::string, UINT>& idToCommand,
        std::unordered_map<UINT, std::string>& commandToId,
        UINT& nextId
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
                AppendMenuW(parentMenu, MF_SEPARATOR, 0, nullptr);
                continue;
            }

            std::string label;
            (void)obj["label"].get_string().get(label);
            std::wstring wideLabel = Utf8ToWide(label.c_str());

            UINT commandId = nextId++;
            idToCommand[id] = commandId;
            commandToId[commandId] = id;

            if (type == 2) {
                HMENU subMenu = CreatePopupMenu();
                simdjson::dom::array children;
                if (obj["children"].get_array().get(children) == simdjson::SUCCESS) {
                    BuildMenuFromJson(subMenu, children, idToCommand, commandToId, nextId);
                }
                UINT flags = MF_POPUP | MF_STRING;
                if (!isEnabled) flags |= MF_GRAYED;
                AppendMenuW(parentMenu, flags, reinterpret_cast<UINT_PTR>(subMenu), wideLabel.c_str());
            } else {
                UINT flags = MF_STRING;
                if (!isEnabled) flags |= MF_GRAYED;
                AppendMenuW(parentMenu, flags, commandId, wideLabel.c_str());
            }
        }
    }

    bool FindMenuItem(
        InfiniFrameWindow::Impl* impl,
        const char* menuItemId,
        HMENU& outParent,
        UINT& outPosition,
        UINT& outCommandId
    ) {
        auto it = impl->_menuItemIdToCommandId.find(menuItemId);
        if (it == impl->_menuItemIdToCommandId.end())
            return false;

        outCommandId = it->second;

        HMENU menuBar = GetMenu(impl->_hWnd);
        if (menuBar == nullptr)
            return false;

        int topCount = GetMenuItemCount(menuBar);
        for (int t = 0; t < topCount; t++) {
            HMENU sub = GetSubMenu(menuBar, t);
            if (sub == nullptr) continue;

            int subCount = GetMenuItemCount(sub);
            for (int s = 0; s < subCount; s++) {
                if (GetMenuItemID(sub, s) == outCommandId) {
                    outParent = sub;
                    outPosition = s;
                    return true;
                }

                HMENU nested = GetSubMenu(sub, s);
                if (nested == nullptr) continue;

                int nestedCount = GetMenuItemCount(nested);
                for (int n = 0; n < nestedCount; n++) {
                    if (GetMenuItemID(nested, n) == outCommandId) {
                        outParent = nested;
                        outPosition = n;
                        return true;
                    }
                }
            }
        }

        return false;
    }
}

void InfiniFrameWindow::ApplyInitMenuBar(const char* menuBarJson) {
    if (menuBarJson == nullptr || menuBarJson[0] == '\0')
        return;

    if (m_impl->_menuBar != nullptr) {
        DestroyMenuRecursive(m_impl->_menuBar);
        m_impl->_menuBar = nullptr;
        m_impl->_menuItemIdToCommandId.clear();
        m_impl->_menuCommandIdToItemId.clear();
        m_impl->_nextMenuCommandId = 1;
    }

    try {
        simdjson::dom::parser parser;
        std::string jsonInput(menuBarJson);
        simdjson::padded_string padded(jsonInput);
        simdjson::dom::element root = parser.parse(padded);

        simdjson::dom::array items;
        if (root["items"].get_array().get(items) != simdjson::SUCCESS)
            return;

        HMENU menuBar = CreateMenu();
        BuildMenuFromJson(
            menuBar, items,
            m_impl->_menuItemIdToCommandId,
            m_impl->_menuCommandIdToItemId,
            m_impl->_nextMenuCommandId
        );

        m_impl->_menuBar = menuBar;
        m_impl->_menuBarJson = menuBarJson;

        SetMenu(m_impl->_hWnd, menuBar);
        DrawMenuBar(m_impl->_hWnd);
    } catch (const simdjson::simdjson_error&) {
    }
}

void InfiniFrameWindow::SetMenuBarJson(const char* menuBarJson) {
    ApplyInitMenuBar(menuBarJson);
}

void InfiniFrameWindow::SetMenuItemEnabledById(const char* menuItemId, bool enabled) {
    HMENU parent = nullptr;
    UINT position = 0;
    UINT commandId = 0;
    if (!FindMenuItem(m_impl.get(), menuItemId, parent, position, commandId))
        return;

    UINT state = MF_BYCOMMAND | (enabled ? MF_ENABLED : MF_GRAYED);
    EnableMenuItem(parent, commandId, state);
    DrawMenuBar(m_impl->_hWnd);
}

void InfiniFrameWindow::SetMenuItemVisibleById(const char* menuItemId, bool visible) {
    auto it = m_impl->_menuItemIdToCommandId.find(menuItemId);
    if (it == m_impl->_menuItemIdToCommandId.end())
        return;

    if (visible) {
        std::string json = m_impl->_menuBarJson;
        ApplyInitMenuBar(json.c_str());
    } else {
        HMENU parent = nullptr;
        UINT position = 0;
        UINT commandId = 0;
        if (FindMenuItem(m_impl.get(), menuItemId, parent, position, commandId)) {
            RemoveMenu(parent, commandId, MF_BYCOMMAND);
            DrawMenuBar(m_impl->_hWnd);
        }
    }
}

void InfiniFrameWindow::ClickMenuItemById(const char* menuItemId) {
    if (m_impl->_webMessageReceivedCallback == nullptr)
        return;

    std::string message = std::string("menu:") + menuItemId;
    SendWebMessage(message.c_str());
}

void InfiniFrameWindow::HandleMenuCommand(WPARAM wParam) {
    UINT commandId = LOWORD(wParam);

    auto it = m_impl->_menuCommandIdToItemId.find(commandId);
    if (it == m_impl->_menuCommandIdToItemId.end())
        return;

    if (m_impl->_webMessageReceivedCallback == nullptr)
        return;

    std::string message = std::string("menu:") + it->second;
    SendWebMessage(message.c_str());
}
