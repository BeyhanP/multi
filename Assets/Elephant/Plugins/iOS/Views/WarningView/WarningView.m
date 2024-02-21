#import "WarningView.h"

@implementation WarningView

// MARK: Initializers

- (instancetype)init {
    self = [super init];
    
    if (self) {
        [self setupContainerStackView];
        [self setupContentLabel];
    }
    
    return self;
}


// MARK: - Setup

- (void)setupContainerStackView {
    [self setContainerStackView:[UIStackView new]];
    
    [[self containerStackView] setAxis:UILayoutConstraintAxisHorizontal];
    [[self containerStackView] setAlignment:UIStackViewAlignmentTop];
    [[self containerStackView] setSpacing:10.0];
    
    [self addSubview:[self containerStackView]];
    [[self containerStackView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self containerStackView] topAnchor] constraintEqualToAnchor:[self topAnchor]] setActive:YES];
    [[[[self containerStackView] leadingAnchor] constraintEqualToAnchor:[self leadingAnchor]] setActive:YES];
    [[[[self containerStackView] trailingAnchor] constraintEqualToAnchor:[self trailingAnchor]] setActive:YES];
    [[[[self containerStackView] bottomAnchor] constraintEqualToAnchor:[self bottomAnchor]] setActive:YES];
}

- (void)setupIconImageView {
    [self setIconImageView:[UIImageView new]];
    
    [[self containerStackView] addArrangedSubview:[self iconImageView]];
}

- (void)setupContentLabel {
    [self setContentLabel:[Label new]];
    
    [[self contentLabel] setNumberOfLines:0];
    [[self contentLabel] setFont:[Fonts warningViewText]];
    [[self contentLabel] setTextColor:[Colors warningViewText]];
    
    [[self containerStackView] addArrangedSubview:[self contentLabel]];
}


// MARK: - Configure

- (void)configureWithModel:(WarningViewModel*)model {
    [self configureContentLabelWithContent:[model content]];
}

- (void)configureContentLabelWithContent:(NSString *)content {
    [[self contentLabel] setText:content];
}


@end
