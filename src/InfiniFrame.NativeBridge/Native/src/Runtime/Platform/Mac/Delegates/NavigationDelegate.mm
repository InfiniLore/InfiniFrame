// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <chrono>
#include <string>

#import "NavigationDelegate.h"
#include "../MacDiagnostics.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
namespace {
    int64_t unix_timestamp_milliseconds_utc() {
        return std::chrono::duration_cast<std::chrono::milliseconds>(
                   std::chrono::system_clock::now().time_since_epoch()
               )
            .count();
    }
}

@implementation NavigationDelegate : NSObject

    - (void)webView:(WKWebView *)webView
        didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
        completionHandler:(void (^)(NSURLSessionAuthChallengeDisposition disposition, NSURLCredential *credential))completionHandler {
            bool ignoreCertificateErrorsEnabled = false;
            if (infiniFrame != nullptr)
                infiniFrame->GetIgnoreCertificateErrorsEnabled(&ignoreCertificateErrorsEnabled);
            if(ignoreCertificateErrorsEnabled && challenge.protectionSpace.serverTrust != nullptr)
            {
                SecTrustRef serverTrust = challenge.protectionSpace.serverTrust;
                completionHandler(NSURLSessionAuthChallengeUseCredential, [NSURLCredential credentialForTrust:serverTrust]);
            }
            else
            {
                completionHandler(NSURLSessionAuthChallengePerformDefaultHandling, nil);
            }
        }

    - (void)webView:(WKWebView *)webView didFinishNavigation:(WKNavigation *)navigation {
        if (infiniFrame == nullptr) return;
        NSString* currentUrl = webView.URL.absoluteString;
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeDebugEvent(
            "Navigation",
            "Navigation finished",
            "Info",
            currentUrl == nil ? nullptr : [currentUrl UTF8String],
            0,
            unix_timestamp_milliseconds_utc(),
            nullptr
        );
        infiniFrame->FlushPendingWebMessages();
    }

    - (void)webView:(WKWebView *)webView didFailNavigation:(WKNavigation *)navigation withError:(NSError *)error {
        if (infiniFrame == nullptr) return;
        NSString* currentUrl = webView.URL.absoluteString;
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeDebugEvent(
            "ScriptError",
            error == nil ? "Navigation failed" : [error.localizedDescription UTF8String],
            "Error",
            currentUrl == nil ? nullptr : [currentUrl UTF8String],
            error == nil ? 0 : (int)error.code,
            unix_timestamp_milliseconds_utc(),
            nullptr
        );
    }

    - (void)webView:(WKWebView *)webView didFailProvisionalNavigation:(WKNavigation *)navigation withError:(NSError *)error {
        if (infiniFrame == nullptr) return;
        NSString* currentUrl = webView.URL.absoluteString;
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeDebugEvent(
            "ScriptError",
            error == nil ? "Provisional navigation failed" : [error.localizedDescription UTF8String],
            "Error",
            currentUrl == nil ? nullptr : [currentUrl UTF8String],
            error == nil ? 0 : (int)error.code,
            unix_timestamp_milliseconds_utc(),
            nullptr
        );
    }

    - (void)webViewWebContentProcessDidTerminate:(WKWebView *)webView {
        if (infiniFrame == nullptr) return;
        NSString* currentUrl = webView.URL.absoluteString;
        infiniframe::macos::NativeCallbackScope callbackScope;
        infiniFrame->InvokeDebugEvent(
            "Process",
            "WKWebView content process terminated",
            "Error",
            currentUrl == nil ? nullptr : [currentUrl UTF8String],
            0,
            unix_timestamp_milliseconds_utc(),
            nullptr
        );
    }

@end
