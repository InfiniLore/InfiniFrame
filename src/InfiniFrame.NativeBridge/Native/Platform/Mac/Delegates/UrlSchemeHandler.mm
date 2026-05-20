// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------

#import "UrlSchemeHandler.h"

// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------

@implementation UrlSchemeHandler : NSObject

- (void)webView:(WKWebView *)webView startURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
    NSURL *url = [[urlSchemeTask request] URL];
    auto *urlUtf8 = const_cast<char *>([url.absoluteString UTF8String]);
    int numBytes = 0;
    char* contentType = nullptr;
    void* dotNetResponse = requestHandler == nullptr
        ? nullptr
        : requestHandler(urlUtf8, &numBytes, &contentType);

    NSInteger statusCode = dotNetResponse == nullptr ? 404 : 200;
    NSString* nsContentType = contentType == nullptr
        ? @"application/octet-stream"
        : [[NSString stringWithUTF8String:contentType] autorelease];

    NSDictionary* headers = @{ @"Content-Type" : nsContentType, @"Cache-Control": @"no-cache" };
    NSHTTPURLResponse *response = [[NSHTTPURLResponse alloc] initWithURL:url statusCode:statusCode HTTPVersion:nil headerFields:headers];
    [urlSchemeTask didReceiveResponse:response];
    if (dotNetResponse != nullptr && numBytes > 0)
        [urlSchemeTask didReceiveData:[NSData dataWithBytes:dotNetResponse length:numBytes]];
    [urlSchemeTask didFinish];

    free(dotNetResponse);
    free(contentType);
}

- (void)webView:(WKWebView *)webView stopURLSchemeTask:(id <WKURLSchemeTask>)urlSchemeTask
{
}

@end
