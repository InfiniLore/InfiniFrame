// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <string>
#include <unordered_map>

#include <Cocoa/Cocoa.h>
#include <simdjson.h>

#include "Runtime/Platform/Mac/Window.Cocoa.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// MenuActionHandler – target-action helper
// ---------------------------------------------------------------------------------------------------------------------
@interface MenuActionHandler : NSObject
@property (nonatomic, assign) InfiniFrameWindow* window;
@property (nonatomic, assign) std::unordered_map<NSInteger, std::string>* tagToItemId;
- (void)menuItemClicked:(NSMenuItem*)sender;
@end

@implementation MenuActionHandler

- (void)menuItemClicked:(NSMenuItem*)sender {
    if (_window == nullptr || _tagToItemId == nullptr)
        return;

    NSInteger tag = [sender tag];
    auto it = _tagToItemId->find(tag);
    if (it == _tagToItemId->end())
        return;

    std::string message = std::string("menu:") + it->second;
    _window->SendWebMessage(message.c_str());
}

@end
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    void BuildMenuFromJson(
        NSMenu* parentMenu,
        const simdjson::dom::array& items,
        std::unordered_map<std::string, NSInteger>& idToTag,
        std::unordered_map<NSInteger, std::string>& tagToId,
        NSInteger& nextTag,
        MenuActionHandler* handler
    ) {
        for (const auto& item : items) {
            simdjson::dom::object obj;
            if (item.get(obj) != simdjson::SUCCESS) continue;

            std::string id;
            if (obj["id"].get_string().get(id) != simdjson::SUCCESS)
                continue;

            int64_t type = 0;
            obj["type"].get_int64().get(type);

            bool isEnabled = true;
            obj["isEnabled"].get_bool().get(isEnabled);

            bool isVisible = true;
            obj["isVisible"].get_bool().get(isVisible);

            if (!isVisible)
                continue;

            if (type == 1) {
                [parentMenu addItem:[NSMenuItem separatorItem]];
                continue;
            }

            std::string label;
            obj["label"].get_string().get(label);

            NSInteger tag = nextTag++;
            idToTag[id] = tag;
            tagToId[tag] = id;

            NSMenuItem* menuItem = [[NSMenuItem alloc]
                initWithTitle:[NSString stringWithUTF8String:label.c_str()]
                action:@selector(menuItemClicked:)
                keyEquivalent:@""];
            [menuItem setTag:tag];
            [menuItem setTarget:handler];
            [menuItem setEnabled:isEnabled ? YES : NO];

            if (type == 2) {
                NSMenu* subMenu = [[NSMenu alloc] initWithTitle:[NSString stringWithUTF8String:label.c_str()]];
                simdjson::dom::array children;
                if (obj["children"].get_array().get(children) == simdjson::SUCCESS) {
                    BuildMenuFromJson(subMenu, children, idToTag, tagToId, nextTag, handler);
                }
                [menuItem setSubmenu:subMenu];
                [subMenu release];
            }

            [parentMenu addItem:menuItem];
            [menuItem release];
        }
    }
}
// ---------------------------------------------------------------------------------------------------------------------
// InfiniFrameWindow menu methods
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::ApplyInitMenuBar(const char* menuBarJson) {
    if (menuBarJson == nullptr || menuBarJson[0] == '\0')
        return;

    if (m_impl->_menuBar != nil) {
        [NSApp setMainMenu:nil];
        [m_impl->_menuBar release];
        m_impl->_menuBar = nil;
        m_impl->_menuItemIdToTag.clear();
        m_impl->_menuTagToItemId.clear();
        m_impl->_nextMenuTag = 1;
    }

    if (m_impl->_menuActionHandler == nil) {
        m_impl->_menuActionHandler = [[MenuActionHandler alloc] init];
        [m_impl->_menuActionHandler setWindow:this];
        [m_impl->_menuActionHandler setTagToItemId:&m_impl->_menuTagToItemId];
    }

    try {
        simdjson::dom::parser parser;
        std::string jsonInput(menuBarJson);
        simdjson::padded_string padded(jsonInput);
        simdjson::dom::element root = parser.parse(padded);

        simdjson::dom::array items;
        if (root["items"].get_array().get(items) != simdjson::SUCCESS)
            return;

        NSMenu* menuBar = [[NSMenu alloc] initWithTitle:@"MainMenu"];
        BuildMenuFromJson(
            menuBar, items,
            m_impl->_menuItemIdToTag,
            m_impl->_menuTagToItemId,
            m_impl->_nextMenuTag,
            m_impl->_menuActionHandler
        );

        m_impl->_menuBar = menuBar;
        m_impl->_menuBarJson = [[NSString stringWithUTF8String:menuBarJson] retain];

        [NSApp setMainMenu:menuBar];
    } catch (const simdjson::simdjson_error&) {
    }
}

void InfiniFrameWindow::SetMenuBarJson(const char* menuBarJson) {
    ApplyInitMenuBar(menuBarJson);
}

void InfiniFrameWindow::SetMenuItemEnabledById(const char* menuItemId, bool enabled) {
    auto it = m_impl->_menuItemIdToTag.find(menuItemId);
    if (it == m_impl->_menuItemIdToTag.end())
        return;

    NSInteger tag = it->second;
    NSMenu* menuBar = [NSApp mainMenu];
    if (menuBar == nil)
        return;

    NSArray* items = [menuBar itemArray];
    for (NSMenuItem* topItem in items) {
        NSMenu* subMenu = [topItem submenu];
        if (subMenu == nil) continue;

        NSMenuItem* found = [subMenu itemWithTag:tag];
        if (found != nil) {
            [found setEnabled:enabled ? YES : NO];
            return;
        }

        NSArray* subItems = [subMenu itemArray];
        for (NSMenuItem* subItem in subItems) {
            NSMenu* nestedMenu = [subItem submenu];
            if (nestedMenu == nil) continue;

            NSMenuItem* nestedFound = [nestedMenu itemWithTag:tag];
            if (nestedFound != nil) {
                [nestedFound setEnabled:enabled ? YES : NO];
                return;
            }
        }
    }
}

void InfiniFrameWindow::SetMenuItemVisibleById(const char* menuItemId, bool visible) {
    auto it = m_impl->_menuItemIdToTag.find(menuItemId);
    if (it == m_impl->_menuItemIdToTag.end())
        return;

    NSInteger tag = it->second;
    NSMenu* menuBar = [NSApp mainMenu];
    if (menuBar == nil)
        return;

    NSArray* items = [menuBar itemArray];
    for (NSMenuItem* topItem in items) {
        NSMenu* subMenu = [topItem submenu];
        if (subMenu == nil) continue;

        NSMenuItem* found = [subMenu itemWithTag:tag];
        if (found != nil) {
            [found setHidden:visible ? NO : YES];
            return;
        }

        NSArray* subItems = [subMenu itemArray];
        for (NSMenuItem* subItem in subItems) {
            NSMenu* nestedMenu = [subItem submenu];
            if (nestedMenu == nil) continue;

            NSMenuItem* nestedFound = [nestedMenu itemWithTag:tag];
            if (nestedFound != nil) {
                [nestedFound setHidden:visible ? NO : YES];
                return;
            }
        }
    }
}

void InfiniFrameWindow::ClickMenuItemById(const char* menuItemId) {
    if (m_impl->_webMessageReceivedCallback == nullptr)
        return;

    std::string message = std::string("menu:") + menuItemId;
    SendWebMessage(message.c_str());
}
