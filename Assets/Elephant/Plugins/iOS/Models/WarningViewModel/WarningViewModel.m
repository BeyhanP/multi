#import "WarningViewModel.h"

@implementation WarningViewModel

// MARK: - Initiailizers

- (instancetype)initWithContent:(NSString *)content {
    self = [super init];
    
    if (self) {
        [self setContent:content];
    }
    
    return self;
}

@end
