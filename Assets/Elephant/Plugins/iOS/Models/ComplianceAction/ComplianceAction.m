#import "ComplianceAction.h"

@implementation ComplianceAction

// MARK: - Initializers

- (instancetype)initWithTitle:(NSString *)title action:(ActionType)action {
    self = [super init];
    
    if (self) {
        [self setTitle:title];
        [self setAction:action];
    }
    
    return self;
}

- (instancetype)initWithJSONDictionary:(NSDictionary *)jsonDict {
    self = [super init];
    
    if (self) {
        [self setValuesForKeysWithDictionary:jsonDict];
    }
    
    return self;
}


// MARK: - Methods

-(id) getPayload {
    return [self payload];
}


// MARK: - Override Methods

- (void)setValue:(id)value forKey:(NSString *)key {
    if ([key  isEqual: @"title"]) {
        [self setTitle:value];
    }
    else if ([key  isEqual:@"action"]) {
        [self setAction:[Utils getActionTypeFromString:value]];
    }
    else if ([key  isEqual:@"payload"]) {
        NSDictionary* payloadData = [Utils JSONStringToJSONDictionary:value];
        
        switch ([self action]) {
            case URL:
                [self setPayload:[[URLPayload alloc] initWithJSONDictionary:payloadData]];
                break;
            case DATA_REQUEST:
                break;
            case CCPA: case GDPR_AD_CONSENT:
                [self setPayload:[[PersonalizedAdsPayload alloc] initWithJSONDictionary:payloadData]];
                break;
            case CUSTOM_POPUP:
                [self setPayload:[[CustomPayload alloc] initWithJSONDictionary:payloadData]];
                break;
            default:
                break;
        }
    }
}

@end
