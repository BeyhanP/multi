#import "InteractableHandler.h"

@implementation InteractableHandler

- (void)onButtonTapped:(InteractionType)interactionType {
    switch(interactionType) {
    case TOS_ACCEPT:
        UnitySendMessage("Elephant", "UserConsentAction", "TOS_ACCEPT");
        break;
    case GDPR_AD_CONSENT_AGREE:
        UnitySendMessage("Elephant", "UserConsentAction", "GDPR_AD_CONSENT_AGREE");
        break;
    case GDPR_AD_CONSENT_DECLINE:
        UnitySendMessage("Elephant", "UserConsentAction", "GDPR_AD_CONSENT_DECLINE");
        break;
    case PERSONALIZED_ADS_AGREE:
        UnitySendMessage("Elephant", "UserConsentAction", "PERSONALIZED_ADS_AGREE");
        break;
    case PERSONALIZED_ADS_DECLINE:
        UnitySendMessage("Elephant", "UserConsentAction", "PERSONALIZED_ADS_DECLINE");
        break;
    case CALL_DATA_REQUEST:
        UnitySendMessage("Elephant", "UserConsentAction", "CALL_DATA_REQUEST");
        break;
    case DELETE_REQUEST_CANCEL:
        UnitySendMessage("Elephant", "UserConsentAction", "DELETE_REQUEST_CANCEL");
        break;
    case RETRY_CONNECTION:
        UnitySendMessage("Elephant", "UserConsentAction", "RETRY_CONNECTION");
        break;
    }
}

@end
