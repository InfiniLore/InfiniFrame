#pragma once

#ifndef INFINIFRAME_EXPORTS_SHARED_H
#define INFINIFRAME_EXPORTS_SHARED_H

#include "../Core/InfiniFrame.h"
#include "ExportGuards.h"

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

#endif // INFINIFRAME_EXPORTS_SHARED_H
