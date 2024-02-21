#import "Label.h"

@implementation Label

// MARK: - Initializers

- (instancetype)initWithTop:(CGFloat)top left:(CGFloat)left bottom:(CGFloat)bottom right:(CGFloat)right {
    self = [super init];
    
    if (self) {
        [self setEdgeInsets:UIEdgeInsetsMake(top, left, bottom, right)];
    }
    
    return self;
}


// MARK: - Override Methods

- (void)drawTextInRect:(CGRect)rect {
    [super drawTextInRect:UIEdgeInsetsInsetRect(rect, [self edgeInsets])];
}

- (CGSize)intrinsicContentSize {
    CGSize intrinsicContentSize = [super intrinsicContentSize];
    
    intrinsicContentSize.width += [self edgeInsets].left + [self edgeInsets].right;
    intrinsicContentSize.height += [self edgeInsets].top + [self edgeInsets].bottom;
    
    return intrinsicContentSize;
}

@end
