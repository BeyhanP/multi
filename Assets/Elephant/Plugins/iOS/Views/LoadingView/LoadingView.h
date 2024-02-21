#import "View.h"

@interface LoadingView : View

// MARK: - Properties

@property(nonatomic, readwrite) View* containerView;
@property(nonatomic, readwrite) UIActivityIndicatorView* activityIndicator;


// MARK: - Setup

-(void)setupContainerView;
-(void)setupActivityIndicator;


// MARK: - Methods

-(void)start;
-(void)stop;

@end
