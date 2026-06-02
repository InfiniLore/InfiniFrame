#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <exception>
#include <utility>

#include "Runtime/Shared/Window/InfiniFrame.h"
#include "Runtime/Shared/Utilities/InteropStatus.h"
#include "ExportErrorState.h"
#include "ExportValidation.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::exports {
    template <typename Fn> InteropStatus RunExportStatus(Fn&& fn) noexcept {
        try {
            SetSuccess();
            std::forward<Fn>(fn)();
            if (g_lastStatus != InteropStatus::Success) {
                return g_lastStatus;
            }
            SetSuccess();
            return InteropStatus::Success;
        } catch (const std::exception& ex) {
            return TranslateException(ex);
        } catch (...) {
            SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return InteropStatus::OperationFailed;
        }
    }

    template <typename Fn> InteropStatus RunWindowExportStatus(InfiniFrameWindow* instance, Fn&& fn) noexcept {
        return RunExportStatus([&] {
            if (!EnsureNotNull(instance, "instance")) {
                return;
            }

            std::forward<Fn>(fn)(instance);
        });
    }

    template <typename T, typename Fn>
    T RunWindowReturnExport(InfiniFrameWindow* instance, T fallback, Fn&& fn) noexcept {
        try {
            if (!EnsureNotNull(instance, "instance")) {
                return fallback;
            }

            T value = std::forward<Fn>(fn)(instance);
            SetSuccess();
            return value;
        } catch (const std::exception& ex) {
            TranslateException(ex);
            return fallback;
        } catch (...) {
            SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return fallback;
        }
    }

    template <typename T, typename Fn> T RunReturnExport(T fallback, Fn&& fn) noexcept {
        try {
            T value = std::forward<Fn>(fn)();
            SetSuccess();
            return value;
        } catch (const std::exception& ex) {
            TranslateException(ex);
            return fallback;
        } catch (...) {
            SetFailure(InteropStatus::OperationFailed, "Unknown native exception.");
            return fallback;
        }
    }
}