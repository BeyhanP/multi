#import "ErrorView.h"

@implementation ErrorView

// MARK: - Initializers

- (instancetype)init {
    self = [super init];
    
    if (self) {
        [self setupContainerStackView];
        [self setupMessageLabel];
        [self setupOkButton];
    }
    
    return self;
}


// MARK: - Setup

- (void)setupContainerStackView {
    [self setContainerStackView:[UIStackView new]];
    
    [[self containerStackView] setAxis:UILayoutConstraintAxisVertical];
    [[self containerStackView] setSpacing:10.0];
    
    [self addSubview:[self containerStackView]];
    [[self containerStackView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self containerStackView] topAnchor] constraintEqualToAnchor:[self topAnchor]] setActive:YES];
    [[[[self containerStackView] leadingAnchor] constraintEqualToAnchor:[self leadingAnchor]] setActive:YES];
    [[[[self containerStackView] trailingAnchor] constraintEqualToAnchor:[self trailingAnchor]] setActive:YES];
    [[[[self containerStackView] bottomAnchor] constraintEqualToAnchor:[self bottomAnchor]] setActive:YES];
}

- (void)setupMessageLabel {
    [self setMessageLabel:[[Label alloc] initWithTop:30.0 left:15.0 bottom:15.0 right:15.0]];
    
    [[self messageLabel] setFont:[Fonts textViewText]];
    [[self messageLabel] setTextAlignment:NSTextAlignmentCenter];
    [[self messageLabel] setNumberOfLines:0];
    [[self containerStackView] addArrangedSubview:[self messageLabel]];
}

- (void)setupOkButton {
    [self setOkButton:[[Button alloc] initWithTitle:@"OK" titleColor:[Colors buttonTitle] backgroundColor:[Colors buttonBackground]]];
    
    [[self okButton] setBorderWithWidth:1.0 color:[Colors separatorBackground]];
    [[[[self okButton] heightAnchor] constraintGreaterThanOrEqualToConstant:60.0] setActive:YES];
    [[self okButton] addTarget:self action:@selector(okButtonDidTapped:) forControlEvents:UIControlEventTouchUpInside];
    [[self containerStackView] addArrangedSubview:[self okButton]];
}


// MARK: - Configure

- (void)configureWithModel:(ErrorViewModel *)model {
    [[self messageLabel] setText:[model message]];
}


// MARK: - Actions

-(void)okButtonDidTapped:(Button*)sender {
    [[self delegate] didOkButtonTapped];
}

@end
