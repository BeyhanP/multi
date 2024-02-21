#import "View.h"

@implementation View

// MARK: - Initializers

-(id) initWithCornerRadius:(CGFloat)radius backgroundColor:(UIColor *)backgroundColor {
    self = [super init];
    
    if (self) {
        [self setBackgroundColor:backgroundColor];
        [self setCornerRadiusWithValue:radius];
    }
    
    return self;
}


// MARK: - Methods

- (void) setCornerRadiusWithValue:(CGFloat) radius {
    [self setClipsToBounds:YES];
    [[self layer] setCornerRadius:radius];
}

@end
