// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#include "../Window.Cocoa.Internal.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

void InfiniFrameWindow::AddCustomSchemeName(const AutoStringConst scheme)
{
    if (scheme == nullptr)
        return;
    if (m_impl->_customSchemeNames.size() >= InfiniFrameInitParams::MaxCustomSchemeNames)
        return;
    for (const auto& existing : m_impl->_customSchemeNames) {
        if (strcasecmp(existing.c_str(), scheme) == 0)
            return;
    }
    m_impl->_customSchemeNames.emplace_back(scheme);
}
