#pragma once
// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#include <WebKit/WKWebView.h>
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
@interface InfiniFrameWebView : WKWebView {
    @private
    BOOL _infiniFrameContextMenuEnabled;
    BOOL _infiniFrameZoomEnabled;
}

- (void)setInfiniFrameContextMenuEnabled:(BOOL)enabled;
- (void)setInfiniFrameZoomEnabled:(BOOL)enabled;

@end
