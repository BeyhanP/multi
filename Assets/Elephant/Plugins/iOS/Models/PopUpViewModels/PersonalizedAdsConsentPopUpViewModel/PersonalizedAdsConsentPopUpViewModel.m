#import "PersonalizedAdsConsentPopUpViewModel.h"

@implementation PersonalizedAdsConsentPopUpViewModel

// MARK: - Initializers

- (instancetype)initWithAction:(ActionType)action title:(NSString *)title text:(NSString *)text declineButtonTitle:(NSString *)declineButtonTitle agreeButtonTitle:(NSString *)agreeButtonTitle backToGameButtonTitle:(NSString *)backToGameButtonTitle hyperlinks:(NSArray<Hyperlink *> *)hyperlinks interactable:(id<Interactable>)interactable {
    self = [super initWithTitle:title text:text buttonTitle:backToGameButtonTitle hyperlinks:hyperlinks interactable:interactable];
    
    if (self) {
        [self setAction:action];
        [self setDeclineButtonTitle:declineButtonTitle];
        [self setAgreeButtonTitle:agreeButtonTitle];
    }
    
    return self;
}

@end
