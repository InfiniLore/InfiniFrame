#include <cstddef>
#include <type_traits>

#include "Api/Abi/Basic.h"
#include "Api/Abi/Callbacks.h"
#include "Api/Abi/CustomSchemeResponse.h"
#include "Api/Abi/DialogButtons.h"
#include "Api/Abi/DialogIcon.h"
#include "Api/Abi/DialogResult.h"
#include "Api/Abi/InfiniFrameApplicationInitParams.h"
#include "Api/Abi/InfiniFrameWindowInitParams.h"
#include "Api/Abi/Monitor.h"

static_assert(std::is_standard_layout_v<InfiniFrameApplicationInitParams>);
static_assert(std::is_standard_layout_v<InfiniFrameWindowInitParams>);
static_assert(std::is_standard_layout_v<CustomSchemeResponse>);
static_assert(offsetof(InfiniFrameApplicationInitParams, StructSize) ==
              sizeof(const char*) * 4);
static_assert(offsetof(CustomSchemeResponse, StructSize) == 0);
