typedef NS_ENUM(NSUInteger, ActionType) {
    URL,
    DATA_REQUEST,
    GDPR_AD_CONSENT,
    CCPA,
    CUSTOM_POPUP
};
#define ActionTypeArray @"URL", @"DATA_REQUEST", @"GDPR_AD_CONSENT", @"CCPA", @"CUSTOM_POPUP", nil
