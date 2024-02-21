#import <Foundation/Foundation.h>
#import "ActionType.h"
#import "Constants.h"
#import "WebViewController.h"


@interface Utils : NSObject

// MARK: - Methods

+(NSUInteger)getActionTypeFromString:(NSString*)actionTypeString;
+(NSUInteger)getPopUpSubviewTypeFromString:(NSString*)subviewTypeString;
+(NSDictionary*)JSONStringToJSONDictionary:(NSString*)jsonString;
+(void)openURL:(NSString*)urlString;
+(void)presentURL:(NSURL*)url;

@end
