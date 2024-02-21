#import "UIViewController+Common.h"

@implementation UIViewController (Common)

- (id)hasSubviewWithKindOfClass:(Class)kind {
    for (UIView* subview in [[self view] subviews]) {
        if ([subview isKindOfClass:kind]) {
            return subview;
        }
    }
    
    return nil;
}

@end
