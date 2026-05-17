#pragma once

#include <expected>

#include "ErrorCode.h"

template <typename T> using Result = std::expected<T, ErrorCode>;
