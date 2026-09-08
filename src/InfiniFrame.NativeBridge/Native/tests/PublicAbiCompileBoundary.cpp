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
static_assert(offsetof(InfiniFrameApplicationInitParams, StructSize) ==
              sizeof(const char*) * 4);
static_assert(offsetof(CustomSchemeResponse, StructSize) == 0);
