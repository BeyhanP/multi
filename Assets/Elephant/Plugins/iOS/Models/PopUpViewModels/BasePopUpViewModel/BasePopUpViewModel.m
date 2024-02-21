#import "BasePopUpViewModel.h"

@implementation BasePopUpViewModel

// MARK: - Initializers

- (instancetype)initWithInteractable:(id<Interactable>)interactable {
    self = [super init];
    
    if (self) {
        [self setInteractable:interactable];
    }
    
    return self;
}

@end
