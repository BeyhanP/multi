#import "NSString+Common.h"

@implementation NSString (Hyperlink)

- (BOOL)isEmpty {
    return [self length] == 0;
}

- (NSString *)createHtmlUrlReferenceWithUrl:(NSString *)url {
    return [NSString stringWithFormat:@"<a href='%@'>%@</a>", url, self];
}

@end
