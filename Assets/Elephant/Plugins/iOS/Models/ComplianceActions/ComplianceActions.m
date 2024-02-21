#import "ComplianceActions.h"

@implementation ComplianceActions

// MARK: - Initializers

- (instancetype)initWithJSONDictionary:(NSDictionary *)jsonDict {
    self = [super init];
    
    if (self) {
        [self setValuesForKeysWithDictionary:jsonDict];
    }
    
    return self;
}


// MARK: - Override Methods

- (void)setValue:(id)value forKey:(NSString *)key {
    if ([key isEqual:@"actions"]) {
        NSArray* jsonModels = value;
        NSMutableArray<ComplianceAction*>* actions = [[NSMutableArray<ComplianceAction*> alloc] initWithCapacity:jsonModels.count];
        
        for(NSDictionary* actionDict in jsonModels) {
            [actions addObject:[[ComplianceAction alloc] initWithJSONDictionary:actionDict]];
        }
        
        [self setActions:actions];
    }
}

@end
