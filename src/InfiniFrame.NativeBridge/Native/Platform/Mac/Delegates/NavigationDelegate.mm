// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#import "NavigationDelegate.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

@implementation NavigationDelegate : NSObject

    - (void)webView:(WKWebView *)webView
        didReceiveAuthenticationChallenge:(NSURLAuthenticationChallenge *)challenge
        completionHandler:(void (^)(NSURLSessionAuthChallengeDisposition disposition, NSURLCredential *credential))completionHandler {
            bool ignoreCertificateErrorsEnabled = false;
            infiniFrame->GetIgnoreCertificateErrorsEnabled(&ignoreCertificateErrorsEnabled);
            if(ignoreCertificateErrorsEnabled)
            {
                SecTrustRef serverTrust = challenge.protectionSpace.serverTrust;
                CFDataRef exceptions = SecTrustCopyExceptions(serverTrust);
                CFRelease(exceptions);
                completionHandler(NSURLSessionAuthChallengeUseCredential, [NSURLCredential credentialForTrust:serverTrust]);
            }
            else
            {
                completionHandler(NSURLSessionAuthChallengePerformDefaultHandling, nil);
            }
        }

@end
