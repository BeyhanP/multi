#import "DataRequestPayload.h"

@implementation DataRequestPayload

// MARK: - HyperlinkManager

- (NSArray<Hyperlink *> *)getHyperlinks {
    return [[NSArray<Hyperlink*> alloc] initWithObjects:
            [[Hyperlink alloc] initWithMask:[Constants privacyPolicyMask] text:[self privacy_policy_text] url:[self privacy_policy_url]],
            [[Hyperlink alloc] initWithMask:[Constants termsOfServiceMask] text:[self terms_of_service_text] url:[self terms_of_service_url]],
            [[Hyperlink alloc] initWithMask:[Constants dataRequestMask] text:[self data_request_text] url:[self data_request_url]], nil];
}

@end
