#include "Window.Win32.Context.h"

InfiniFrameWindow* LookupWindowInstance(const HWND hwnd) {
    return reinterpret_cast<InfiniFrameWindow*>(GetWindowLongPtr(hwnd, GWLP_USERDATA));
}

HWND ResolveParentWindowHandle(InfiniFrameWindow* parent) {
    if (parent == nullptr)
        return nullptr;

    HWND parentHwnd = parent->getHwnd();
    if (parentHwnd == nullptr || !IsWindow(parentHwnd))
        return nullptr;

    return parentHwnd;
}
