#import <Foundation/Foundation.h>
#import "ComplianceAction.h"

@interface ComplianceActions : NSObject

// MARK: - Properties

@property(nonatomic, readwrite) NSArray<ComplianceAction*>* actions;


// MARK: - Initializers

-(instancetype)initWithJSONDictionary:(NSDictionary*)jsonDict;

@end
