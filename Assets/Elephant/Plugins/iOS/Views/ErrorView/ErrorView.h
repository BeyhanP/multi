#import "View.h"
#import "Button.h"
#import "Label.h"
#import "Fonts.h"
#import "Colors.h"
#import "ErrorViewModel.h"

@protocol ErrorViewDelegate <NSObject>

@required
-(void)didOkButtonTapped;

@end

@interface ErrorView : View

// MARK: - Properties

@property(nonatomic, readwrite) UIStackView* containerStackView;
@property(nonatomic, readwrite) Label* messageLabel;
@property(nonatomic, readwrite) Button* okButton;

@property(nonatomic, weak) id <ErrorViewDelegate> delegate;


// MARK: - Setup

-(void)setupContainerStackView;
-(void)setupMessageLabel;
-(void)setupOkButton;


// MARK: - Configure

-(void)configureWithModel:(ErrorViewModel*)model;


// MARK: - Actions

-(void)okButtonDidTapped:(Button*)sender;

@end
