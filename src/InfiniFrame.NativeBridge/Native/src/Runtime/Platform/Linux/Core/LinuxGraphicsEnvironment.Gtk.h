#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace infiniframe::linux_gtk {
    /// Applies deterministic software-rendering defaults (CI / forced / no render device).
    /// Never overrides variables already present in the environment.
    void ConfigureGraphicsEnvironment();

    /// Logs a one-time rendering-environment diagnostics summary: GTK/WebKitGTK versions,
    /// display backend, renderer selection inputs, compositing mode, and the
    /// software/hardware rendering decision. Safe to call at any point after gtk_init().
    void LogRenderingDiagnostics();
}
