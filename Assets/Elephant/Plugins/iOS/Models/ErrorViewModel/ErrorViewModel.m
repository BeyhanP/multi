#import "ErrorViewModel.h"

@implementation ErrorViewModel

// MARK: - Initializers

- (instancetype)initWithMessage:(NSString *)message {
    self = [super init];
    
    if (self) {
        [self setMessage:message];
    }
    
    return self;
}

@end
