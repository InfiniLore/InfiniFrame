#pragma once
/**
 * @file ExportApi.h
 * @brief Shared declarations for InfiniFrame C ABI export translation units.
 */

#ifndef INFINIFRAME_INTEROP_EXPORTAPI_H
#define INFINIFRAME_INTEROP_EXPORTAPI_H

#include "Core/InfiniFrame.h"
#include "Interop/NativeResult.h"
#include "Interop/NativeString.h"

#ifdef _WIN32
#define INFINIFRAME_NATIVE_EXPORT __declspec(dllexport)
#else
#define INFINIFRAME_NATIVE_EXPORT
#endif

#endif // INFINIFRAME_INTEROP_EXPORTAPI_H
