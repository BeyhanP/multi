#import <Foundation/Foundation.h>

@interface BasePayload: NSObject

// MARK: - Initializers

-(instancetype)initWithJSONDictionary:(NSDictionary*)jsonDict;

@end
