// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include "../Window.Cocoa.Internal.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::AddCustomSchemeName(const const char* scheme)
{
    if (scheme == nullptr)
        return;
    if (m_impl->common._customSchemeNames.size() >= InfiniFrameWindowInitParams::MaxCustomSchemeNames)
        return;
    for (const auto& existing : m_impl->common._customSchemeNames) {
        if (strcasecmp(existing.c_str(), scheme) == 0)
            return;
    }
    m_impl->common._customSchemeNames.emplace_back(scheme);
}
