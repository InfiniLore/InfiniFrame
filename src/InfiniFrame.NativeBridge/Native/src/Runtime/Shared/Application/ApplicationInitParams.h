#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

/**
 * @brief Initialization parameters for InfiniFrameApplication.
 *
 * Field order defines the ABI layout shared with the managed (.NET) side via LayoutKind.Sequential.
 * When adding or removing fields, append at the end (before StructSize) and bump StructSize.
 */
struct ApplicationInitParams {
    int StructSize;

    // ── Process identity (Win32) ──────────────────────────────────────────
    const char* WindowsAppUserModelId;
    const char* NotificationRegistrationId;

    // ── WebView2 runtime path override (Win32) ────────────────────────────
    const char* WebView2RuntimePath;

    // ── ABI version (must remain last) ────────────────────────────────────
    int Reserved;
};
