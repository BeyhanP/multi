#import "BasePopUpView.h"
#import "PopUpViewModel.h"
#import "Constants.h"
#import "View.h"
#import "TextView.h"
#import "Button.h"

@interface PopUpView : BasePopUpView

// MARK: - Properties

@property(nonatomic, readwrite) TextView* textView;
@property(nonatomic, readwrite) View* separatorView;
@property(nonatomic, readwrite) Button* button;

@property(copy) void (^buttonCallback)(void);


// MARK: - Setup

-(void)setupTextView;
-(void)setupSeparator;
-(void)setupButton;


// MARK: - Configure

-(void)configureTextViewWithText:(NSString*)text hyperlinks:(NSArray<Hyperlink*>*)hyperlinks;
-(void)configureButtonWithTitle:(NSString*)title;


// MARK: - Actions

-(void)buttonTapped:(UIButton *)sender;


@end

