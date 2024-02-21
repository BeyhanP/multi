#import <WebKit/WebKit.h>

@interface WebViewController : UIViewController <UIWebViewDelegate>

// MARK: - Properties

@property(nonatomic) UIView* navigationBarView;
@property(nonatomic) UIButton* doneButton;
@property(nonatomic) UIButton* dismissButton;
@property(nonatomic) WKWebView* webView;
@property(nonatomic) NSURL* url;


// MARK: - Setup

-(void)setupNavigationBar;
-(void)setupDoneButton;
-(void)setupDismissButton;
-(void)setupWebView;


// MARK: - Configure

-(void)configureWithURL:(NSURL*)url;
-(void)doneButtonTapped:(UIButton*)sender;

@end
