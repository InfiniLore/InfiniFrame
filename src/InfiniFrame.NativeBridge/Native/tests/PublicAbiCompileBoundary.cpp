#include <cstddef>
#include <type_traits>

#include "Runtime/Internal/Interop/Basic.h"
#include "Runtime/Internal/Interop/Callbacks.h"
#include "Runtime/Internal/Interop/CustomSchemeResponse.h"
#include "Runtime/Internal/Interop/DialogButtons.h"
#include "Runtime/Internal/Interop/DialogIcon.h"
#include "Runtime/Internal/Interop/DialogResult.h"
#include "Runtime/Internal/Interop/InfiniFrameApplicationInitParams.h"
#include "Runtime/Internal/Interop/InfiniFrameWindowInitParams.h"
#include "Runtime/Internal/Interop/Monitor.h"

static_assert(std::is_standard_layout_v<InfiniFrameApplicationInitParams>);
static_assert(std::is_standard_layout_v<InfiniFrameWindowInitParams>);
static_assert(std::is_standard_layout_v<CustomSchemeResponse>);
static_assert(offsetof(InfiniFrameApplicationInitParams, StructSize) ==
              sizeof(const char*) * 4);
static_assert(offsetof(CustomSchemeResponse, StructSize) == 0);
