#import "SettingsPopUpViewModel.h"

@implementation SettingsPopUpViewModel

// MARK: - Initializers

- (instancetype)initWithActions:(NSArray<ComplianceAction *> *)actions interactable:(id<Interactable>)interactable {
    self = [super initWithInteractable:interactable];
    
    if (self) {
        [self setActions:actions];
    }
    
    return self;
}

@end
