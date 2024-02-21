#import <Foundation/Foundation.h>
#import "ActionType.h"
#import "BasePayload.h"
#import "URLPayload.h"
#import "DataRequestPayload.h"
#import "PersonalizedAdsPayload.h"
#import "CustomPayload.h"
#import "Constants.h"
#import "Utils.h"

@interface ComplianceAction : NSObject

// MARK: - Properties

@property (nonatomic, readwrite) NSString* title;
@property (nonatomic, readwrite) ActionType action;
@property (nonatomic, readwrite) BasePayload* payload;


// MARK: - Initializers

-(instancetype)initWithTitle:(NSString*)title action:(ActionType)action;
-(instancetype)initWithJSONDictionary:(NSDictionary*)jsonDict;


// MARK: - Methods

-(id) getPayload;

@end
