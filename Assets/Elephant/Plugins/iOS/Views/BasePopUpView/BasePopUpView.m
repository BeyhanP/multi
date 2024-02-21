#import "BasePopUpView.h"

@implementation BasePopUpView

// MARK: - Properties


// MARK: - Initializers

- (instancetype)init {
    self = [super init];
    
    if (self) {
        [self setupView];
        [self setupBackground];
        [self setupContainerStackView];
        [self setupLoadingView];
        [self setupErrorView];
        [self setupContentView];
    }
    
    return self;
}


// MARK: - Setup

- (void)setupView {
    [self setFrame:CGRectMake(0, 0, [Constants screenSize].width, [Constants screenSize].height)];
}

- (void)setupBackground {
    [self setBlurBackground:[[UIVisualEffectView alloc] initWithEffect:[UIBlurEffect effectWithStyle:UIBlurEffectStyleDark]]];
    
    [self addSubview:[self blurBackground]];
    [[self blurBackground] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self blurBackground] topAnchor] constraintEqualToAnchor:[self topAnchor]] setActive:YES];
    [[[[self blurBackground] leadingAnchor] constraintEqualToAnchor:[self leadingAnchor]] setActive:YES];
    [[[[self blurBackground] trailingAnchor] constraintEqualToAnchor:[self trailingAnchor]] setActive:YES];
    [[[[self blurBackground] bottomAnchor] constraintEqualToAnchor:[self bottomAnchor]] setActive:YES];
}

- (void)setupContainerStackView {
    [self setContainerStackView:[UIStackView new]];
    
    [[self containerStackView] setClipsToBounds:YES];
    [[[self containerStackView] layer] setCornerRadius:14.0];
    [[self containerStackView] setBackgroundColor:[Colors containerViewBackground]];
    [[self containerStackView] setAxis:UILayoutConstraintAxisVertical];
    
    [self addSubview:[self containerStackView]];
    [[self containerStackView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self containerStackView] centerXAnchor] constraintEqualToAnchor:[self centerXAnchor]] setActive:YES];
    [[[[self containerStackView] centerYAnchor] constraintEqualToAnchor:[self centerYAnchor]] setActive:YES];
    [[[[self containerStackView] widthAnchor] constraintEqualToConstant:[Constants screenSize].width-100.0] setActive:YES];
}

- (void)setupLoadingView {
    [self setLoadingView:[LoadingView new]];
    
    [[[[self loadingView] heightAnchor] constraintEqualToConstant:350.0] setActive:YES];
    [[self containerStackView] addArrangedSubview:[self loadingView]];
}

- (void)setupErrorView {
    [self setErrorView:[ErrorView new]];
    
    [[self errorView] setDelegate:self];
    [[self errorView] configureWithModel:[[ErrorViewModel alloc] initWithMessage:@"Something went wrong. Please try again."]];
    [[self containerStackView] addArrangedSubview:[self errorView]];
}

- (void)setupContentView {
    [self setContentView:[[View alloc] initWithCornerRadius:14.0 backgroundColor:[Colors containerViewBackground]]];
    
    [[self containerStackView] addArrangedSubview:[self contentView]];
}


// MARK: - Configure

- (void)configureWithModel:(BasePopUpViewModel *)model {
    [self setInteractable:[model interactable]];
}


// MARK: - Methods

- (void)showSubViewWithyType:(PopUpSubviewType)type {
    [[self loadingView] setHidden:type != LOADING];
    [[self errorView] setHidden:type != ERROR];
    [[self contentView] setHidden:type != CONTENT];
}

- (void)showWithSubviewType:(PopUpSubviewType)type {
    [self showSubViewWithyType:type];
    [[[Constants rootViewController] view] addSubview:self];
}

- (void)hide {
    [self removeFromSuperview];
}


// MARK: - Class Methods

+ (instancetype)createView {
    BasePopUpView* view = [[Constants rootViewController] hasSubviewWithKindOfClass:[self class]];
    
    if (view) {
        return view;
    }
    
    return [self new];
}


// MARK: - ErrorViewDelegate

- (void)didOkButtonTapped {
    [self hide];
}

@end
