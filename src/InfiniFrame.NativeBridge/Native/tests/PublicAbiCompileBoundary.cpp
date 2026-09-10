#include <cstddef>
#include <type_traits>

#include "Runtime/Internal/Interop/Types/Basic.h"
#include "Runtime/Internal/Interop/Types/Callbacks.h"
#include "Runtime/Internal/Interop/Types/CustomSchemeResponse.h"
#include "Runtime/Internal/Interop/Types/DialogButtons.h"
#include "Runtime/Internal/Interop/Types/DialogIcon.h"
#include "Runtime/Internal/Interop/Types/DialogResult.h"
#include "Runtime/Internal/Interop/Types/InfiniFrameApplicationInitParams.h"
#include "Runtime/Internal/Interop/Types/InfiniFrameWindowInitParams.h"
#include "Runtime/Internal/Interop/Types/Monitor.h"

static_assert(std::is_standard_layout_v<InfiniFrameApplicationInitParams>);
static_assert(std::is_standard_layout_v<InfiniFrameWindowInitParams>);
static_assert(std::is_standard_layout_v<CustomSchemeResponse>);
static_assert(sizeof(bool) == 1);
static_assert(offsetof(InfiniFrameApplicationInitParams, WebView2RuntimePath) == 0);
static_assert(offsetof(InfiniFrameApplicationInitParams, NotificationRegistrationId) == sizeof(const char*));
static_assert(offsetof(InfiniFrameApplicationInitParams, AppUserModelId) == sizeof(const char*) * 2);
static_assert(offsetof(InfiniFrameApplicationInitParams, DefaultNotificationIcon) == sizeof(const char*) * 3);
static_assert(offsetof(InfiniFrameApplicationInitParams, StructSize) == sizeof(const char*) * 4);
static_assert(offsetof(InfiniFrameApplicationInitParams, StructSize) + sizeof(int) <= sizeof(InfiniFrameApplicationInitParams));
static_assert(offsetof(InfiniFrameWindowInitParams, StartString) == 0);
static_assert(offsetof(InfiniFrameWindowInitParams, StartUrl) > offsetof(InfiniFrameWindowInitParams, StartString));
static_assert(offsetof(InfiniFrameWindowInitParams, CustomSchemeNames) > offsetof(InfiniFrameWindowInitParams, DebugEventHandler));
static_assert(offsetof(InfiniFrameWindowInitParams, DragDropEnabled) > offsetof(InfiniFrameWindowInitParams, DragDropHandler));
static_assert(offsetof(InfiniFrameWindowInitParams, BackgroundColorR) > offsetof(InfiniFrameWindowInitParams, NotificationsEnabled));
static_assert(offsetof(InfiniFrameWindowInitParams, MenuBarJson) > offsetof(InfiniFrameWindowInitParams, BackgroundColorA));
static_assert(offsetof(InfiniFrameWindowInitParams, StructSize) > offsetof(InfiniFrameWindowInitParams, MenuBarJson));
static_assert(offsetof(InfiniFrameWindowInitParams, StructSize) + sizeof(int) <= sizeof(InfiniFrameWindowInitParams));
static_assert(offsetof(CustomSchemeResponse, StructSize) == 0);
