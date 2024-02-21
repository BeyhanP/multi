#import "Button.h"

@implementation Button

// MARK: - Initializers

- (instancetype)init {
    self = [super init];
    
    if (self) {
        [[self titleLabel] setNumberOfLines:0];
        [[self titleLabel] setLineBreakMode:NSLineBreakByWordWrapping];
        [[self titleLabel] setTextAlignment:NSTextAlignmentCenter];
    }
    
    return self;
}

- (instancetype)initWithTitle:(NSString *)title titleColor:(UIColor *)titleColor backgroundColor:(UIColor*)backgroundColor {
    self = [self init];
    
    if (self) {
        [self setTitle:title];
        [self setTitleColor:titleColor];
        [self setBackgroundColor:backgroundColor];
    }
    
    return self;
}

- (instancetype)initWithAction:(ComplianceAction *)action titleColor:(UIColor *)titleColor backgroundColor:(UIColor *)backgroundColor {
    self = [self initWithTitle:action.title titleColor:titleColor backgroundColor:backgroundColor];
    
    if (self) {
        [self setComplianceAction:action];
    }
    
    return self;
}


// MARK: - Methods

- (void)setTitle:(NSString *)title {
    [self setTitle:title forState:UIControlStateNormal];
}

- (void)setTitleColor:(UIColor *)color {
    [self setTitleColor:color forState:UIControlStateNormal];
    [self setTitleColor:[color colorWithAlphaComponent:0.2] forState:UIControlStateHighlighted];
}

- (void)setBorderWithWidth:(CGFloat)width color:(UIColor*)color {
    [[self layer] setBorderWidth:width];
    [[self layer] setBorderColor:[color CGColor]];
}

@end
