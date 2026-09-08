// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::AddCustomSchemeName(const char* scheme) {
    if (scheme == nullptr)
        return;
    if (m_impl->common._customSchemeNames.size() >= InfiniFrameWindowInitParams::MaxCustomSchemeNames)
        return;
    for (const auto& existing : m_impl->common._customSchemeNames) {
        if (g_ascii_strcasecmp(existing.c_str(), scheme) == 0)
            return;
    }
    m_impl->common._customSchemeNames.emplace_back(scheme);
}
