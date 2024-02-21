#import "CustomPayload.h"
#import "HyperlinkManager.h"
#import "Constants.h"

@interface PersonalizedAdsPayload : CustomPayload <HyperlinkManager>

@property(nonatomic, readwrite) NSString* privacy_policy_text;
@property(nonatomic, readwrite) NSString* privacy_policy_url;
@property(nonatomic, readwrite) NSString* decline_text_action_button;
@property(nonatomic, readwrite) NSString* agree_text_action_button;

@end
