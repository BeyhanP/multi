#import "BasePopUpView.h"
#import "BlockedPopUpViewModel.h"
#import "TextView.h"
#import "WarningView.h"
#import "Button.h"
#import "Constants.h"
#import "Fonts.h"

@interface BlockedPopUpView : BasePopUpView

// MARK: - Properties

@property(nonatomic, readwrite) UIStackView* contentStackView;
@property(nonatomic, readwrite) UILabel* titleLabel;
@property(nonatomic, readwrite) TextView* contentTextView;
@property(nonatomic, readwrite) WarningView* warningView;
@property(nonatomic, readwrite) Button* startPlayingGameButton;


// MARK: - Setup

-(void)setupContentStackView;
-(void)setupTitleLabel;
-(void)setupContentTextView;
-(void)setupWarningView;
-(void)setupStartPlayingGameButton;


// MARK: - Configure

-(void)configureTitleLabelWithTitle:(NSString*)title;
-(void)configureContentTextViewWithText:(NSString*)text hyperlinks:(NSArray<Hyperlink*>*)hyperlinks;
-(void)configureWarningViewWithModel:(WarningViewModel*)model;
-(void)configureStartPlayingGameButtonWithTitle:(NSString*)buttonTitle;


// MARK: - Actions

-(void)startPlayingGameButtonTapped:(Button*)sender;

@end
