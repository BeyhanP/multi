#import "BasePopUpView.h"
#import "PopUpViewModel.h"
#import "PersonalizedAdsConsentPopUpViewModel.h"
#import "Constants.h"
#import "TextView.h"
#import "Button.h"
#import "Fonts.h"
#import "Hyperlink.h"

@interface PersonalizedAdsConsentPopUpView : BasePopUpView

// MARK: - Properties

@property(nonatomic, readwrite) UIStackView* contentStackView;
@property(nonatomic, readwrite) UILabel* titleLabel;
@property(nonatomic, readwrite) TextView* contentTextView;
@property(nonatomic, readwrite) UIStackView* consentActionButtonsStackView;
@property(nonatomic, readwrite) Button* agreeButton;
@property(nonatomic, readwrite) Button* declineButton;
@property(nonatomic, readwrite) Button* backToGameButton;

@property(nonatomic, readwrite) ActionType action;


// MARK: - Setup

-(void)setupContentStackView;
-(void)setupTitleLabel;
-(void)setupContentTextView;
-(void)setupConsentActionButtonsStackView;
-(void)setupDeclineButton;
-(void)setupAgreeButton;
-(void)setupBackToGameButton;


// MARK: - Configure

-(void)configureWithComplianceAction:(ComplianceAction*)complianceAction interactable:(id<Interactable>)interactable;
-(void)configureTitleLabelWithTitle:(NSString*) title;
-(void)configureContentTextViewWithText:(NSString*)text hyperlinks:(NSArray<Hyperlink*>*)hyperlinks;
-(void)configureDeclineButtonWithTitle:(NSString*) title;
-(void)configureAgreeButtonWithTitle:(NSString*) title;
-(void)configureBackToGameButtonWithTitle:(NSString*) title;


// MARK: - Actions

-(void)declineButtonTapped:(Button *)sender;
-(void)agreeButtonTapped:(Button *)sender;
-(void)backToGameButtonTapped:(Button *)sender;

@end
