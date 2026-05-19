#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <webkit2/webkit2.h>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace gtk_webkit {
    void HandleWebMessage(
        WebKitUserContentManager* contentManager, WebKitJavascriptResult* jsResult, gpointer userData
    );

    void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, gpointer userData);
}
