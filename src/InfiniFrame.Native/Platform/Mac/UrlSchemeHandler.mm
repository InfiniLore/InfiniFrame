#ifdef __APPLE__
#import "UrlSchemeHandler.h"
#include "Shared/CustomSchemeResponse.h"

@implementation UrlSchemeHandler : NSObject

- (void)webView:(WKWebView *)webView startURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
    NSURL *url = [[urlSchemeTask request] URL];
    auto *urlUtf8 = const_cast<char *>([url.absoluteString UTF8String]);
    auto dotNetResponse = InfiniFrame::Native::Shared::InvokeCustomSchemeCallback(requestHandler, urlUtf8);

    NSInteger statusCode = dotNetResponse.HasBody() ? 200 : 404;
    NSString* nsContentType = [NSString stringWithUTF8String:dotNetResponse.ContentTypeOrDefault()];

    NSDictionary* headers = @{ @"Content-Type" : nsContentType, @"Cache-Control": @"no-cache" };
    NSHTTPURLResponse *response = [[NSHTTPURLResponse alloc] initWithURL:url statusCode:statusCode HTTPVersion:nil headerFields:headers];
    [urlSchemeTask didReceiveResponse:response];
    if (dotNetResponse.HasBody() && dotNetResponse.length > 0)
        [urlSchemeTask didReceiveData:[NSData dataWithBytes:dotNetResponse.body.get() length:static_cast<NSUInteger>(dotNetResponse.length)]];
    [urlSchemeTask didFinish];
}

- (void)webView:(WKWebView *)webView stopURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
}

@end
#endif
