#import <UIKit/UIKit.h>
#import "UIViewController+Common.h"
#import "Constants.h"
#import "BasePopUpViewModel.h"
#import "Colors.h"
#import "LoadingView.h"
#import "ErrorView.h"
#import "Interactable.h"
#import "PopUpSubviewType.h"

@interface BasePopUpView : UIView <ErrorViewDelegate>

// MARK: - Properties

@property(nonatomic, readwrite) UIVisualEffectView* blurBackground;
@property(nonatomic, readwrite) UIStackView* containerStackView;
@property(nonatomic, readwrite) LoadingView* loadingView;
@property(nonatomic, readwrite) ErrorView* errorView;
@property(nonatomic, readwrite) View* contentView;

@property(nonatomic, weak) id <Interactable> interactable;


// MARK: - Initializers


// MARK: - Setup

-(void)setupView;
-(void)setupBackground;
-(void)setupContainerStackView;
-(void)setupLoadingView;
-(void)setupErrorView;
-(void)setupContentView;


// MARK: - Configure

-(void)configureWithModel:(BasePopUpViewModel*)model;


// MARK: - Methods

- (void)showSubViewWithyType:(PopUpSubviewType)type;
- (void)showWithSubviewType:(PopUpSubviewType)type;
- (void)hide;


// MARK: - Class Methods

+(instancetype)createView;

@end
