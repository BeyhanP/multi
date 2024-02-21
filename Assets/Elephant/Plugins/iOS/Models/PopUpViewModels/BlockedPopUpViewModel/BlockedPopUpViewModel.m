#import "BlockedPopUpViewModel.h"

@implementation BlockedPopUpViewModel

// MARK: - Initializers

- (instancetype)initWithTitle:(NSString *)title text:(NSString *)text warningContent:(NSString *)warningContent buttonTitle:(NSString *)buttonTitle hyperlinks:(NSArray<Hyperlink *> *)hyperlinks interactable:(id<Interactable>)interactable {
    self = [super initWithTitle:title text:text buttonTitle:buttonTitle hyperlinks:hyperlinks interactable:interactable];
    
    if (self) {
        [self setWarningContent:warningContent];
    }
    
    return self;
}

@end
