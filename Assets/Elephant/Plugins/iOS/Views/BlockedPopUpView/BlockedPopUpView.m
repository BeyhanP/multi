#import "BlockedPopUpView.h"

@implementation BlockedPopUpView

// MARK: - Properties

// MARK: - Initializers

- (id)init {
    self = [super init];
    
    if (self) {
        [self setupContentStackView];
        [self setupTitleLabel];
        [self setupContentTextView];
        [self setupWarningView];
        [self setupStartPlayingGameButton];
    }
    
    return self;
}


// MARK: - Setup

- (void)setupContentStackView {
    [self setContentStackView:[UIStackView new]];
    
    [[self contentStackView] setAxis:UILayoutConstraintAxisVertical];
    [[self contentStackView] setAlignment:UIStackViewAlignmentCenter];
    [[self contentView] addSubview:[self contentStackView]];
    [[self contentStackView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[self contentStackView] setSpacing:15.0];
    [[[[self contentStackView] topAnchor] constraintEqualToAnchor:[[self contentView] topAnchor] constant:15.0] setActive:YES];
    [[[[self contentStackView] leadingAnchor] constraintEqualToAnchor:[[self contentView] leadingAnchor]] setActive:YES];
    [[[[self contentStackView] trailingAnchor] constraintEqualToAnchor:[[self contentView] trailingAnchor]] setActive:YES];
    [[[[self contentStackView] bottomAnchor] constraintEqualToAnchor:[[self contentView] bottomAnchor]] setActive:YES];
}

- (void)setupTitleLabel {
    [self setTitleLabel:[UILabel new]];
    
    [[self titleLabel] setFont:[Fonts popupTitle]];
    [[self titleLabel] setTextColor:[Colors textViewText]];
    [[self titleLabel] setTextAlignment:NSTextAlignmentCenter];
    
    [[self contentStackView] addArrangedSubview:[self titleLabel]];
    [[[[self titleLabel] leadingAnchor] constraintEqualToAnchor:[[self contentStackView] leadingAnchor]] setActive:YES];
    [[[[self titleLabel] trailingAnchor] constraintEqualToAnchor:[[self contentStackView] trailingAnchor]] setActive:YES];
}

- (void)setupContentTextView {
    [self setContentTextView:[TextView new]];
    
    [[self contentStackView] addArrangedSubview:[self contentTextView]];
    [[[[self contentTextView] leadingAnchor] constraintEqualToAnchor:[[self contentStackView] leadingAnchor] constant:10.0] setActive:YES];
    [[[[self contentTextView] trailingAnchor] constraintEqualToAnchor:[[self contentStackView] trailingAnchor] constant:-10.0] setActive:YES];
}

- (void)setupWarningView {
    [self setWarningView:[WarningView new]];
    
    [[self contentStackView] addArrangedSubview:[self warningView]];
    [[[[self warningView] leadingAnchor] constraintEqualToAnchor:[[self contentStackView] leadingAnchor] constant:15.0] setActive:YES];
    [[[[self warningView] trailingAnchor] constraintEqualToAnchor:[[self contentStackView] trailingAnchor] constant:-15.0] setActive:YES];
}

- (void)setupStartPlayingGameButton {
    [self setStartPlayingGameButton:[Button new]];
    
    [[self startPlayingGameButton] setTitleColor:[Colors buttonTitle]];
    [[[self startPlayingGameButton] titleLabel] setFont:[Fonts buttonTitle]];
    [[self startPlayingGameButton] setBackgroundColor:[Colors buttonBackground]];
    [[self startPlayingGameButton] setBorderWithWidth:1.0 color:[Colors separatorBackground]];
    [[self startPlayingGameButton] addTarget:self action:@selector(startPlayingGameButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    
    [[self contentStackView] addArrangedSubview:[self startPlayingGameButton]];
    [[[[self startPlayingGameButton] heightAnchor] constraintGreaterThanOrEqualToConstant:60.0] setActive:YES];
    [[[[self startPlayingGameButton] leadingAnchor] constraintEqualToAnchor:[[self contentStackView] leadingAnchor]] setActive:YES];
    [[[[self startPlayingGameButton] trailingAnchor] constraintEqualToAnchor:[[self contentStackView] trailingAnchor]] setActive:YES];
}


// MARK: - Configure

- (void)configureWithModel:(BasePopUpViewModel *)model {
    [super configureWithModel:model];
    
    BlockedPopUpViewModel* popUpModel = (BlockedPopUpViewModel*) model;
    
    [self configureTitleLabelWithTitle:[popUpModel title]];
    [self configureContentTextViewWithText:[popUpModel text] hyperlinks:[popUpModel hyperlinks]];
    [self configureWarningViewWithModel:[[WarningViewModel alloc] initWithContent:[popUpModel warningContent]]];
    [self configureStartPlayingGameButtonWithTitle:[popUpModel buttonTitle]];
}

- (void)configureTitleLabelWithTitle:(NSString *)title {
    [[self titleLabel] setText:title];
}

- (void)configureContentTextViewWithText:(NSString *)text hyperlinks:(NSArray<Hyperlink *> *)hyperlinks {
    [[self contentTextView] setTextWithHtmlString:[Hyperlink configurePopUpHtmlContentWithContent:text hyperlinks:hyperlinks]];
}

- (void)configureWarningViewWithModel:(WarningViewModel *)model {
    [[self warningView] setHidden:[[model content] isEmpty]];
    [[self warningView] setLayoutMargins:UIEdgeInsetsMake(0.0, 10.0, 0.0, 10.0)];
    [[self warningView] configureWithModel:model];
}

- (void)configureStartPlayingGameButtonWithTitle:(NSString *)buttonTitle {
    [[self startPlayingGameButton] setTitle:buttonTitle];
}


// MARK: - Actions

- (void)startPlayingGameButtonTapped:(Button *)sender {
    [[self interactable] onButtonTapped:DELETE_REQUEST_CANCEL];
    [self hide];
}

@end
