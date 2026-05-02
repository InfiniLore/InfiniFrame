#pragma once

#ifdef __linux__

#include <webkit2/webkit2.h>

void HandleWebMessage(
    WebKitUserContentManager* contentManager,
    WebKitJavascriptResult* jsResult,
    gpointer userData
    );

#endif
