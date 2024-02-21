#import "PersonalizedAdsConsentPopUpView.h"

@implementation PersonalizedAdsConsentPopUpView

// MARK: - Properties

// MARK: - Initializers

- (instancetype)init {
    self = [super init];
    
    if (self) {
        [self setupContentStackView];
        [self setupTitleLabel];
        [self setupContentTextView];
        [self setupConsentActionButtonsStackView];
        [self setupDeclineButton];
        [self setupAgreeButton];
        [self setupBackToGameButton];
    }
    
    return self;
}


// MARK: - Setup

- (void)setupContentStackView {
    [self setContentStackView:[UIStackView new]];
    
    [[self contentStackView] setAxis:UILayoutConstraintAxisVertical];
    
    [[self contentView] addSubview:[self contentStackView]];
    [[self contentStackView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self contentStackView] topAnchor] constraintEqualToAnchor: [[self contentView] topAnchor] constant:15.0] setActive:YES];
    [[[[self contentStackView] leadingAnchor] constraintEqualToAnchor: [[self contentView] leadingAnchor]] setActive:YES];
    [[[[self contentStackView] trailingAnchor] constraintEqualToAnchor: [[self contentView] trailingAnchor]] setActive:YES];
    [[[[self contentStackView] bottomAnchor] constraintEqualToAnchor: [[self contentView] bottomAnchor]] setActive:YES];
}

- (void)setupTitleLabel {
    [self setTitleLabel:[UILabel new]];
    
    [[self titleLabel] setFont:[Fonts popupTitle]];
    [[self titleLabel] setTextColor:[Colors textViewText]];
    [[self titleLabel] setTextAlignment:NSTextAlignmentCenter];
    
    [[self contentStackView] addArrangedSubview:[self titleLabel]];
}

- (void)setupContentTextView {
    [self setContentTextView:[TextView new]];
    
    [[self contentTextView] setTextContainerInset:UIEdgeInsetsMake(20.0, 10.0, 15.0, 10.0)];
    [[self contentStackView] addArrangedSubview:[self contentTextView]];
}

- (void)setupConsentActionButtonsStackView {
    [self setConsentActionButtonsStackView:[UIStackView new]];
    
    [[self consentActionButtonsStackView] setAxis:UILayoutConstraintAxisHorizontal];
    [[self consentActionButtonsStackView] setDistribution:UIStackViewDistributionFillEqually];
    [[[[self consentActionButtonsStackView] heightAnchor] constraintGreaterThanOrEqualToConstant:60.0] setActive:YES];
    [[self contentStackView] addArrangedSubview:[self consentActionButtonsStackView]];
}

- (void)setupDeclineButton {
    [self setDeclineButton:[Button new]];
    
    [[self declineButton] setTitleColor:[Colors buttonTitle]];
    [[self declineButton] setBackgroundColor:[Colors buttonBackground]];
    [[self declineButton] setBorderWithWidth:1.0 color:[Colors separatorBackground]];
    [[self declineButton] addTarget:self action:@selector(declineButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    [[self consentActionButtonsStackView] addArrangedSubview:[self declineButton]];
}

- (void)setupAgreeButton {
    [self setAgreeButton:[Button new]];
    
    [[self agreeButton] setTitleColor:[Colors buttonTitle]];
    [[self agreeButton] setBackgroundColor:[Colors buttonBackground]];
    [[self agreeButton] setBorderWithWidth:1.0 color:[Colors separatorBackground]];
    [[self agreeButton] addTarget:self action:@selector(agreeButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    [[self consentActionButtonsStackView] addArrangedSubview:[self agreeButton]];
}

- (void)setupBackToGameButton {
    [self setBackToGameButton:[Button new]];
    
    [[self backToGameButton] setTitleColor:[Colors buttonTitle]];
    [[self backToGameButton] setBackgroundColor:[Colors buttonBackground]];
    [[self backToGameButton] setBorderWithWidth:1.0 color:[Colors separatorBackground]];
    [[[[self backToGameButton] heightAnchor] constraintGreaterThanOrEqualToConstant:60.0] setActive:YES];
    [[self backToGameButton] addTarget:self action:@selector(backToGameButtonTapped:) forControlEvents:UIControlEventTouchUpInside];
    [[self contentStackView] addArrangedSubview:[self backToGameButton]];
}


// MARK: - Configure

- (void)configureWithModel:(BasePopUpViewModel *)model {
    [super configureWithModel:model];
    
    PersonalizedAdsConsentPopUpViewModel* personalizedAdsModel = (PersonalizedAdsConsentPopUpViewModel*) model;
    
    [self setAction:[personalizedAdsModel action]];
    [self configureTitleLabelWithTitle:[personalizedAdsModel title]];
    [self configureContentTextViewWithText:[personalizedAdsModel text] hyperlinks:[personalizedAdsModel hyperlinks]];
    [self configureDeclineButtonWithTitle:[personalizedAdsModel declineButtonTitle]];
    [self configureAgreeButtonWithTitle:[personalizedAdsModel agreeButtonTitle]];
    [self configureBackToGameButtonWithTitle:[personalizedAdsModel buttonTitle]];
}

- (void)configureWithComplianceAction:(ComplianceAction *)complianceAction interactable:(id<Interactable>)interactable {
    BasePopUpViewModel* model = [[BasePopUpViewModel alloc] initWithInteractable:interactable];
    PersonalizedAdsPayload* payload = [complianceAction getPayload];
    
    [super configureWithModel:model];
    
    [self setAction:complianceAction.action];
    [self configureTitleLabelWithTitle:[payload title]];
    [self configureContentTextViewWithText:[payload content] hyperlinks:[payload getHyperlinks]];
    [self configureDeclineButtonWithTitle:[payload decline_text_action_button]];
    [self configureAgreeButtonWithTitle:[payload agree_text_action_button]];
    [self configureBackToGameButtonWithTitle:[payload consent_text_action_button]];
}

- (void)configureTitleLabelWithTitle:(NSString *)title {
    [[self titleLabel] setText:title];
}

- (void)configureContentTextViewWithText:(NSString *)text hyperlinks:(NSArray<Hyperlink *> *)hyperlinks {
    [[self contentTextView] setTextWithHtmlString:[Hyperlink configurePopUpHtmlContentWithContent:text hyperlinks:hyperlinks]];
}

- (void)configureDeclineButtonWithTitle:(NSString *)title {
    [[self declineButton] setTitle:title];
}

- (void)configureAgreeButtonWithTitle:(NSString *)title {
    [[self agreeButton] setTitle:title];
}

- (void)configureBackToGameButtonWithTitle:(NSString *)title {
    [[self backToGameButton] setTitle:title];
}


// MARK: - Actions

- (void)declineButtonTapped:(Button *)sender {
    NSLog(@"Action type: %lu", (unsigned long)[self action]);
    switch ([self action]) {
        case CCPA:
            [[self interactable] onButtonTapped:PERSONALIZED_ADS_DECLINE];
            break;
        case GDPR_AD_CONSENT:
            [[self interactable] onButtonTapped:GDPR_AD_CONSENT_DECLINE];
            break;
        default:
            break;
    }
    
    [self hide];
}

- (void)agreeButtonTapped:(Button *)sender {
    NSLog(@"Action type: %lu", (unsigned long)[self action]);
    switch ([self action]) {
        case CCPA:
            [[self interactable] onButtonTapped:PERSONALIZED_ADS_AGREE];
            break;
        case GDPR_AD_CONSENT:
            [[self interactable] onButtonTapped:GDPR_AD_CONSENT_AGREE];
            break;
        default:
            break;
    }
    
    [self hide];
}

- (void)backToGameButtonTapped:(Button *)sender {
    [self hide];
}

@end
