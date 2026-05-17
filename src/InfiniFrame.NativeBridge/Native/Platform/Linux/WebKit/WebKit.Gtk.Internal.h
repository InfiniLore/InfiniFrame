#pragma once

#ifndef INFINIFRAME_PLATFORM_LINUX_WEBKIT_GTK_INTERNAL_H
#define INFINIFRAME_PLATFORM_LINUX_WEBKIT_GTK_INTERNAL_H

#include <webkit2/webkit2.h>

namespace gtk_webkit {
void HandleWebMessage(WebKitUserContentManager* contentManager, WebKitJavascriptResult* jsResult, gpointer userData);

void HandleCustomSchemeRequest(WebKitURISchemeRequest* request, gpointer userData);
} // namespace gtk_webkit

#endif // INFINIFRAME_PLATFORM_LINUX_WEBKIT_GTK_INTERNAL_H
