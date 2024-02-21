#import "Utils.h"

@implementation Utils

// MARK: - Methods

+ (NSDictionary*)JSONStringToJSONDictionary:(NSString *)jsonString {
    NSData* jsonData = [jsonString dataUsingEncoding:NSUTF8StringEncoding];
    NSDictionary* jsonDict = [NSJSONSerialization JSONObjectWithData:jsonData options:0 error:nil];
    
    return jsonDict;
}

+ (NSUInteger)getActionTypeFromString:(NSString *)actionTypeString {
    NSUInteger n = [[Constants actionTypeArray] indexOfObject:actionTypeString];
    if(n < 1) n = URL;
    return n;
}

+ (NSUInteger)getPopUpSubviewTypeFromString:(NSString *)subviewTypeString {
    NSUInteger n = [[Constants popUpSubviewTypeArray] indexOfObject:subviewTypeString];
    if(n < 1) n = LOADING;
    return n;
}

+ (void)openURL:(NSString *)urlString {
    NSURL* url = [NSURL URLWithString:urlString];
    [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
}

+ (void)presentURL:(NSURL*)url {
    WebViewController* webViewController = [[WebViewController alloc] init];
    
    [webViewController configureWithURL:url];
    
    [[Constants rootViewController] presentViewController:webViewController animated:YES completion:nil];
}

@end
