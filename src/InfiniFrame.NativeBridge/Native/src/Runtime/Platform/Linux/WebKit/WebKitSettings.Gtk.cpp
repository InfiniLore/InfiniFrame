// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#ifdef _MSC_VER
#pragma warning(push)
#pragma warning(disable: 4100 4244)
#endif
#include <simdjson.h>
#ifdef _MSC_VER
#pragma warning(pop)
#endif

#include "Runtime/Platform/Linux/Window.Gtk.Internal.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
void InfiniFrameWindow::Impl::set_webkit_settings() {
    // WebKitGTK remote inspector requires developer extras to be enabled.
    const bool enableDeveloperExtras = _devToolsEnabled || _remoteDebuggingPort > 0;

    WebKitSettings* settings = webkit_settings_new_with_settings(
        "allow_modal_dialogs", TRUE, "allow_top_navigation_to_data_urls", TRUE, "allow_universal_access_from_file_urls",
        TRUE, "enable_back_forward_navigation_gestures", TRUE, "enable_media_capabilities", TRUE,
        "enable_mock_capture_devices", TRUE, "enable_page_cache", TRUE, "enable_webrtc", TRUE,
        "javascript_can_open_windows_automatically", TRUE,

        "allow_file_access_from_file_urls", _fileSystemAccessEnabled, "disable_web_security", !_webSecurityEnabled,
        "enable_developer_extras", enableDeveloperExtras, "enable_media_stream", _mediaStreamEnabled,
        "enable_smooth_scrolling", _smoothScrollingEnabled, "javascript_can_access_clipboard",
        _javascriptClipboardAccessEnabled, "media_playback_requires_user_gesture", !_mediaAutoplayEnabled, "user_agent",
        _userAgent.c_str(),

        NULL
    );

    if (!_browserControlInitParameters.empty())
        set_webkit_customsettings(settings);

    WebKitWebsiteDataManager* manager = webkit_web_view_get_website_data_manager(WEBKIT_WEB_VIEW(_webview));
    if (_ignoreCertificateErrorsEnabled)
        webkit_website_data_manager_set_tls_errors_policy(manager, WEBKIT_TLS_ERRORS_POLICY_IGNORE);
    else
        webkit_website_data_manager_set_tls_errors_policy(manager, WEBKIT_TLS_ERRORS_POLICY_FAIL);

    webkit_web_view_set_settings(WEBKIT_WEB_VIEW(_webview), settings);
}

void InfiniFrameWindow::Impl::set_webkit_customsettings(WebKitSettings* settings) {
    try {
        simdjson::ondemand::parser parser;
        auto padded = simdjson::padded_string(_browserControlInitParameters);
        auto doc = parser.iterate(padded);

        for (auto field : doc.get_object()) {
            std::string_view keyView = field.unescaped_key();
            auto value = field.value();

            gchar* propertyName = g_strdup(std::string(keyView).c_str());
            GValue propertyValue = G_VALUE_INIT;
            bool hasValidValue = false;

            switch (value.type()) {
                case simdjson::ondemand::json_type::string: {
                    std::string_view strVal;
                    if (value.get(strVal) == simdjson::SUCCESS) {
                        g_value_init(&propertyValue, G_TYPE_STRING);
                        g_value_set_string(&propertyValue, std::string(strVal).c_str());
                        hasValidValue = true;
                    }
                    break;
                }
                case simdjson::ondemand::json_type::boolean: {
                    bool boolVal;
                    if (value.get(boolVal) == simdjson::SUCCESS) {
                        g_value_init(&propertyValue, G_TYPE_BOOLEAN);
                        g_value_set_boolean(&propertyValue, boolVal);
                        hasValidValue = true;
                    }
                    break;
                }
                case simdjson::ondemand::json_type::number: {
                    int64_t intVal;
                    if (value.get(intVal) == simdjson::SUCCESS) {
                        g_value_init(&propertyValue, G_TYPE_INT);
                        g_value_set_int(&propertyValue, static_cast<int>(intVal));
                        hasValidValue = true;
                    } else {
                        double doubleVal;
                        if (value.get(doubleVal) == simdjson::SUCCESS) {
                            g_value_init(&propertyValue, G_TYPE_DOUBLE);
                            g_value_set_double(&propertyValue, doubleVal);
                            hasValidValue = true;
                        }
                    }
                    break;
                }
                default:
                    break;
            }

            if (hasValidValue) {
                g_object_set_property(G_OBJECT(settings), propertyName, &propertyValue);
                g_value_unset(&propertyValue);
            }

            g_free(propertyName);
        }
    } catch (const simdjson::simdjson_error&) {}
}
