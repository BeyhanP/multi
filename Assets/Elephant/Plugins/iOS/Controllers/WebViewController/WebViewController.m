#import "WebViewController.h"

@implementation WebViewController

// MARK: - Life Cycle

- (void)viewDidLoad {
    [super viewDidLoad];
    
    [self setupNavigationBar];
    [self setupDoneButton];
    [self setupDismissButton];
    [self setupWebView];
}


// MARK: - Setup

- (void)setupNavigationBar {
    [self setNavigationBarView:[UIView new]];
    
    [[self navigationBarView] setBackgroundColor:[UIColor lightGrayColor]];
    
    [[self view] addSubview:[self navigationBarView]];
    [[self navigationBarView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self navigationBarView] topAnchor] constraintEqualToAnchor:[[self view] topAnchor]] setActive:YES];
    [[[[self navigationBarView] leadingAnchor] constraintEqualToAnchor:[[self view] leadingAnchor]] setActive:YES];
    [[[[self navigationBarView] trailingAnchor] constraintEqualToAnchor:[[self view] trailingAnchor]] setActive:YES];
    [[[[self navigationBarView] heightAnchor] constraintEqualToConstant:40.0] setActive:YES];
}

- (void)setupDoneButton {
    [self setDoneButton:[UIButton new]];
    [[self doneButton] setTitle:@"Done" forState:UIControlStateNormal];
    [[self doneButton] addTarget:self action:@selector(doneButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    
    [[self doneButton] setTitleEdgeInsets:UIEdgeInsetsMake(0.0, -25.0, 0.0, 0.0)];
    
    [[self navigationBarView] addSubview:[self doneButton]];
    [[self doneButton] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self doneButton] topAnchor] constraintEqualToAnchor:[[self navigationBarView] topAnchor]] setActive:YES];
    [[[[self doneButton] leadingAnchor] constraintEqualToAnchor:[[self navigationBarView] leadingAnchor]] setActive:YES];
    [[[[self doneButton] bottomAnchor] constraintEqualToAnchor:[[self navigationBarView] bottomAnchor]] setActive:YES];
    [[[[self doneButton] widthAnchor] constraintEqualToConstant:100.0] setActive:YES];
}

- (void)setupDismissButton {
    [self setDismissButton:[UIButton new]];
    [[self dismissButton] setTitle:@"X" forState:UIControlStateNormal];
    [[self dismissButton] addTarget:self action:@selector(doneButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    
    [[self dismissButton] setTitleEdgeInsets:UIEdgeInsetsMake(0.0, 0.0, 0.0, -50.0)];
    
    [[self navigationBarView] addSubview:[self dismissButton]];
    [[self dismissButton] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self dismissButton] topAnchor] constraintEqualToAnchor:[[self navigationBarView] topAnchor]] setActive:YES];
    [[[[self dismissButton] trailingAnchor] constraintEqualToAnchor:[[self navigationBarView] trailingAnchor]] setActive:YES];
    [[[[self dismissButton] bottomAnchor] constraintEqualToAnchor:[[self navigationBarView] bottomAnchor]] setActive:YES];
    [[[[self dismissButton] widthAnchor] constraintEqualToConstant:100.0] setActive:YES];
}

- (void)setupWebView {
    [self setWebView:[WKWebView new]];
    [[self webView] setAllowsBackForwardNavigationGestures:YES];
    [[self webView] loadRequest:[[NSURLRequest alloc] initWithURL:[self url]]];
    
    [[self view] addSubview:[self webView]];
    [[self webView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self webView] topAnchor] constraintEqualToAnchor:[[self navigationBarView] bottomAnchor]] setActive:YES];
    [[[[self webView] leadingAnchor] constraintEqualToAnchor:[[self view] leadingAnchor]] setActive:YES];
    [[[[self webView] trailingAnchor] constraintEqualToAnchor:[[self view] trailingAnchor]] setActive:YES];
    [[[[self webView] bottomAnchor] constraintEqualToAnchor:[[self view] bottomAnchor]] setActive:YES];
}


// MARK: - Configure

- (void)configureWithURL:(NSURL *)url {
    [self setUrl:url];
}

- (void)doneButtonTapped:(UIButton *)sender {
    [self dismissViewControllerAnimated:YES completion:nil];
}

@end
