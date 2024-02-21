#import "LoadingView.h"
#import "Colors.h"

@implementation LoadingView

// MARK: - Initializers

- (instancetype)init {
    self = [super init];
    
    if (self) {
        [self setupContainerView];
        [self setupActivityIndicator];
    }
    
    return self;
}


// MARK: - Setup

- (void)setupContainerView {
    [self setContainerView:[[View alloc] initWithCornerRadius:10.0 backgroundColor:[Colors loadingContainerViewBackground]]];
    
    [self addSubview:[self containerView]];
    [[self containerView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self containerView] centerXAnchor] constraintEqualToAnchor:[self centerXAnchor]] setActive:YES];
    [[[[self containerView] centerYAnchor] constraintEqualToAnchor:[self centerYAnchor]] setActive:YES];
    [[[[self containerView] widthAnchor] constraintEqualToConstant:60] setActive:YES];
    [[[[self containerView] heightAnchor] constraintEqualToConstant:60.0] setActive:YES];
}

-(void)setupActivityIndicator {
    [self setActivityIndicator:[[UIActivityIndicatorView alloc] initWithActivityIndicatorStyle:UIActivityIndicatorViewStyleWhite]];
    
    [[self containerView] addSubview:[self activityIndicator]];
    [[self activityIndicator] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self activityIndicator] centerXAnchor] constraintEqualToAnchor:[[self containerView] centerXAnchor]] setActive:YES];
    [[[[self activityIndicator] centerYAnchor] constraintEqualToAnchor:[[self containerView] centerYAnchor]] setActive:YES];
    [[[[self activityIndicator] widthAnchor] constraintEqualToConstant:25.0] setActive:YES];
    [[[[self activityIndicator] heightAnchor] constraintEqualToConstant:25.0] setActive:YES];
    
    [self start];
}


// MARK: - Methods

- (void)start {
    [[self activityIndicator] startAnimating];
}

- (void)stop {
    [[self activityIndicator] stopAnimating];
}

- (void)setHidden:(BOOL)hidden {
    [super setHidden:hidden];
    
    if (hidden) {
        [self stop];
    } else {
        [self start];
    }
}

@end
