#import "PopUpViewModel.h"

@implementation PopUpViewModel

// MARK: - Initializers

- (instancetype)initWithText:(NSString *)content buttonTitle:(NSString *)buttonTitle interactable:(id<Interactable>)interactable {
    self = [super initWithInteractable:interactable];
    
    if (self) {
        [self setTitle:@""];
        [self setText:content];
        [self setButtonTitle:buttonTitle];
    }
    
    return self;
}

- (instancetype)initWithText:(NSString *)text buttonTitle:(NSString *)buttonTitle hyperlinks:(NSArray *)hyperlinks interactable:(id<Interactable>)interactable {
    self = [super initWithInteractable:interactable];
    
    if (self) {
        [self setTitle:@""];
        [self setText:text];
        [self setButtonTitle:buttonTitle];
        [self setHyperlinks:hyperlinks];
    }
    
    return self;
}

- (instancetype)initWithTitle:(NSString *)title text:(NSString *)text buttonTitle:(NSString *)buttonTitle hyperlinks:(NSArray<Hyperlink *> *)hyperlinks interactable:(id<Interactable>)interactable {
    self = [super initWithInteractable:interactable];
    
    if (self) {
        [self setTitle:title];
        [self setText:text];
        [self setButtonTitle:buttonTitle];
        [self setHyperlinks:hyperlinks];
    }
    
    return self;
}

@end
