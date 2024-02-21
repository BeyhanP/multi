#import "PopUpViewModel.h"
#import "ActionType.h"

@interface PersonalizedAdsConsentPopUpViewModel : PopUpViewModel

// MARK: - Properties

@property(nonatomic, readwrite) ActionType action;
@property(nonatomic, readwrite) NSString* declineButtonTitle;
@property(nonatomic, readwrite) NSString* agreeButtonTitle;


// MARK: - Initializers

-(instancetype)initWithAction:(ActionType)action
                        title:(NSString*)title
                         text:(NSString*)text
           declineButtonTitle:(NSString*)declineButtonTitle
             agreeButtonTitle:(NSString*)agreeButtonTitle
        backToGameButtonTitle:(NSString*)backToGameButtonTitle
                   hyperlinks:(NSArray<Hyperlink*>*)hyperlinks
                 interactable:(id<Interactable>)interactable;

@end
