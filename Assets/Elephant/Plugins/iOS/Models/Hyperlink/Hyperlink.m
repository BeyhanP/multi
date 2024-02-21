#import "Hyperlink.h"

@implementation Hyperlink

// MARK: - Initializers

- (instancetype)initWithMask:(NSString *)mask text:(NSString *)text url:(NSString *)url {
    self = [super init];
    
    if (self) {
        [self setMask:mask];
        [self setUrlReference:[text createHtmlUrlReferenceWithUrl:url]];
    }
    
    return self;
}


// MARK: - Methods

+ (NSString *)configurePopUpHtmlContentWithContent:(NSString *)content hyperlinks:(NSArray *)hyperlinks {
    content = [content stringByReplacingOccurrencesOfString:@"\n" withString:@"<br>"];
    
    for(int i = 0; i < hyperlinks.count; i++) {
        Hyperlink* hyperlink = [hyperlinks objectAtIndex:i];
        
        content = [content stringByReplacingOccurrencesOfString:[hyperlink mask] withString:[hyperlink urlReference]];
    }
    
    return content;
}

@end
