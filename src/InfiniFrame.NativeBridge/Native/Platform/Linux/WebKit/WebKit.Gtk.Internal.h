#pragma once

#include <webkit2/webkit2.h>

namespace gtk_webkit {
    void HandleWebMessage(
        WebKitUserContentManager* contentManager, WebKitJavascriptResult* jsResult, gpointer userData
    );

    void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, gpointer userData);
}
