#import "BasePayload.h"

@implementation BasePayload

// MARK: - Properties

- (instancetype)initWithJSONDictionary:(NSDictionary *)jsonDict {
    self = [super init];
    
    if (self) {
        [self setValuesForKeysWithDictionary:jsonDict];
    }
    
    return self;
}

@end
