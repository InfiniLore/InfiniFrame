#pragma once

#ifndef INFINIFRAME_CORE_EXPORTS_H
#define INFINIFRAME_CORE_EXPORTS_H

#include "InfiniFrame.h"
#include "../Utils/ExportGuards.h"

#ifdef __linux__
#include <glib.h>
#endif

#ifdef _WIN32
#define EXPORTED __declspec(dllexport)
#else
#define EXPORTED
#endif

using infiniframe::exports::EnsureNotNull;
using infiniframe::exports::GetLastErrorMessageCopy;
using infiniframe::exports::ResetOut;
using infiniframe::exports::ResetOut2;
using infiniframe::exports::RunExportStatus;
using infiniframe::exports::RunReturnExport;
using infiniframe::exports::RunWindowExportStatus;
using infiniframe::exports::RunWindowReturnExport;

#endif // INFINIFRAME_CORE_EXPORTS_H
