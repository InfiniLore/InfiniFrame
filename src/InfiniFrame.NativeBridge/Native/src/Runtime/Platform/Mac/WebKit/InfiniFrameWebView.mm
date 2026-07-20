// ---------------------------------------------------------------------------------------------------------------------
// Imports
// ---------------------------------------------------------------------------------------------------------------------
#import "InfiniFrameWebView.h"
// ---------------------------------------------------------------------------------------------------------------------
// Code
// ---------------------------------------------------------------------------------------------------------------------
@implementation InfiniFrameWebView

- (instancetype)initWithFrame:(NSRect)frame configuration:(WKWebViewConfiguration*)configuration {
    self = [super initWithFrame:frame configuration:configuration];
    if (self != nil) {
        _infiniFrameContextMenuEnabled = YES;
        _infiniFrameZoomEnabled = YES;
    }
    return self;
}

- (void)setInfiniFrameContextMenuEnabled:(BOOL)enabled {
    _infiniFrameContextMenuEnabled = enabled;
}

- (void)setInfiniFrameZoomEnabled:(BOOL)enabled {
    _infiniFrameZoomEnabled = enabled;
}

- (NSMenu*)menuForEvent:(NSEvent*)event {
    return _infiniFrameContextMenuEnabled ? [super menuForEvent:event] : nil;
}

- (void)magnifyWithEvent:(NSEvent*)event {
    if (_infiniFrameZoomEnabled)
        [super magnifyWithEvent:event];
}

- (void)smartMagnifyWithEvent:(NSEvent*)event {
    if (_infiniFrameZoomEnabled)
        [super smartMagnifyWithEvent:event];
}

@end
