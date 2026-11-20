#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <webkit/webkit.h>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace gtk_webkit {
    void HandleWebMessage(
        WebKitUserContentManager* contentManager,
        JSCValue* value,
        gpointer userData
        );

    void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, gpointer userData);
}