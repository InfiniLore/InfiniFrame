#pragma once
/**
 * @file InfiniFrame.h
 * @brief Main header file for InfiniFrame native interop
 *
 * This file provides unified access to all InfiniFrame types and classes.
 * It is the primary include file for C API consumers.
 */

#ifndef INFINIFRAME_H
#define INFINIFRAME_H

// ============================================================================
// Core Types
// ============================================================================

#include "../Types/Basic.h"
#include "../Types/Dialog.h"
#include "../Types/Callbacks.h"
#include "InfiniFrameInitParams.h"

// ============================================================================
// Core Classes
// ============================================================================

#include "InfiniFrameWindow.h"
#include "InfiniFrameDialog.h"

// ============================================================================
// Utilities
// ============================================================================

#include "../Utils/Common.h"
#include "../Utils/Event.h"

#endif // INFINIFRAME_H
