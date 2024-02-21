#import <UIKit/UIKit.h>
#import "ActionType.h"
#import "PopUpSubviewType.h"

typedef NSDictionary<NSString*, NSString*> Dictionary;

@interface Constants: NSObject

@property (class, nonatomic, readonly) CGSize screenSize;
@property (class, nonatomic, readonly) UIViewController* rootViewController;
@property (class, nonatomic, readonly) NSString* termsOfServiceMask;
@property (class, nonatomic, readonly) NSString* privacyPolicyMask;
@property (class, nonatomic, readonly) NSString* dataRequestMask;
@property (class, nonatomic, readonly) NSArray* actionTypeArray;
@property (class, nonatomic, readonly) NSArray* popUpSubviewTypeArray;

@end
