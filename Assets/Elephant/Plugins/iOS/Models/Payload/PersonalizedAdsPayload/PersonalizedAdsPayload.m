#import "PersonalizedAdsPayload.h"

@implementation PersonalizedAdsPayload


// MARK: - HyperlinkManager

- (NSArray<Hyperlink *> *)getHyperlinks {
    return [[NSArray<Hyperlink*> alloc] initWithObjects:
            [[Hyperlink alloc] initWithMask:[Constants privacyPolicyMask] text:[self privacy_policy_text] url:[self privacy_policy_url]], nil];
}

@end
