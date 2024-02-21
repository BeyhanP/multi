#import "CustomPayload.h"
#import "HyperlinkManager.h"
#import "Constants.h"

@interface DataRequestPayload : CustomPayload <HyperlinkManager>

// MARK: - Properties

@property(nonatomic, readwrite) NSString* data_request_text;
@property(nonatomic, readwrite) NSString* data_request_url;
@property(nonatomic, readwrite) NSString* privacy_policy_text;
@property(nonatomic, readwrite) NSString* privacy_policy_url;
@property(nonatomic, readwrite) NSString* terms_of_service_text;
@property(nonatomic, readwrite) NSString* terms_of_service_url;

@end
