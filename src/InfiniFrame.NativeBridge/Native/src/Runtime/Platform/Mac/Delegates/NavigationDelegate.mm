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
        infiniFrame->CompleteNavigationAndSignalReady(
            reinterpret_cast<uint64_t>(navigation), true, 0, nullptr
        );
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
        infiniFrame->CompleteNavigationAndSignalReady(
            reinterpret_cast<uint64_t>(navigation), false,
            error == nil ? 0 : (int)error.code,
            error == nil ? "Navigation failed" : [error.localizedDescription UTF8String]
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
        infiniFrame->CompleteNavigationAndSignalReady(
            reinterpret_cast<uint64_t>(navigation), false,
            error == nil ? 0 : (int)error.code,
            error == nil ? "Provisional navigation failed" : [error.localizedDescription UTF8String]
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

    - (void)webView:(WKWebView *)webView
        decidePolicyForNavigationAction:(WKNavigationAction *)navigationAction
        decisionHandler:(void (^)(WKNavigationActionPolicy))decisionHandler {
        if (infiniFrame == nullptr) {
            decisionHandler(WKNavigationActionPolicyAllow);
            return;
        }

        NavigationStartingCallback callback = infiniFrame->GetNavigationStartingCallback();
        if (callback == nullptr) {
            decisionHandler(WKNavigationActionPolicyAllow);
            return;
        }

        NSURL *url = navigationAction.request.URL;
        if (url == nil) {
            decisionHandler(WKNavigationActionPolicyAllow);
            return;
        }

        bool isUserInitiated = (navigationAction.navigationType == WKNavigationTypeLinkActivated ||
                                navigationAction.navigationType == WKNavigationTypeFormSubmitted);
        bool isRedirect = (navigationAction.navigationType == WKNavigationTypeOther);
        bool isMainFrame = navigationAction.targetFrame.mainFrame;

        AutoString urlUtf8 = (AutoString)[url.absoluteString UTF8String];
        int cancel = callback(
            urlUtf8, isUserInitiated ? 1 : 0, isRedirect ? 1 : 0, isMainFrame ? 1 : 0
        );

        decisionHandler(cancel ? WKNavigationActionPolicyCancel : WKNavigationActionPolicyAllow);
    }

@end
