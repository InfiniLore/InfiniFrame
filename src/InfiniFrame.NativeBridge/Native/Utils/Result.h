#pragma once

#ifndef INFINIFRAME_UTILS_RESULT_H
#define INFINIFRAME_UTILS_RESULT_H

#include <expected>

#include "ErrorCode.h"

template <typename T>
using Result = std::expected<T, ErrorCode>;

#endif // INFINIFRAME_UTILS_RESULT_H
