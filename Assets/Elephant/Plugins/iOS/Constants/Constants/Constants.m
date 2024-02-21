#import "Constants.h"

@implementation Constants

+ (CGSize)screenSize {
    return [[UIScreen mainScreen] bounds].size;
}

+ (UIViewController *)rootViewController {
    return [[[UIApplication sharedApplication] keyWindow] rootViewController];
}

+ (NSString *)termsOfServiceMask {
    return @"{{terms}}";
}

+ (NSString *)privacyPolicyMask {
    return @"{{privacy}}";
}

+ (NSString *)dataRequestMask {
    return @"{{datarequest}}";
}

+ (NSArray *)actionTypeArray {
    static NSArray *actionTypeArray = nil;
    static dispatch_once_t onceToken;

    dispatch_once(&onceToken, ^{
        actionTypeArray = [[NSArray alloc] initWithObjects:ActionTypeArray];
    });
    
    return actionTypeArray;
}

+ (NSArray *)popUpSubviewTypeArray {
    static NSArray *popUpSubviewTypeArray = nil;
    static dispatch_once_t onceToken;

    dispatch_once(&onceToken, ^{
        popUpSubviewTypeArray = [[NSArray alloc] initWithObjects:PopUpSubviewTypeArray];
    });
    
    return popUpSubviewTypeArray;
}

@end
