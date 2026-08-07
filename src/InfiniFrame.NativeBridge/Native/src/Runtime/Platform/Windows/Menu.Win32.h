#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
class InfiniFrameWindow;

void ApplyInitMenuBar(InfiniFrameWindow* window, const char* menuBarJson);
void SetMenuBarJson(InfiniFrameWindow* window, const char* menuBarJson);
void SetMenuItemEnabledById(InfiniFrameWindow* window, const char* menuItemId, bool enabled);
void SetMenuItemVisibleById(InfiniFrameWindow* window, const char* menuItemId, bool visible);
void ClickMenuItemById(InfiniFrameWindow* window, const char* menuItemId);
void HandleMenuCommand(InfiniFrameWindow* window, WPARAM wParam);
