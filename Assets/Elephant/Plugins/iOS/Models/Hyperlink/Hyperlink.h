#import <Foundation/Foundation.h>
#import "NSString+Common.h"


@interface Hyperlink : NSObject

// MARK: - Properties

@property(nonatomic, readwrite) NSString* mask;
@property(nonatomic, readwrite) NSString* urlReference;


// MARK: - Initializers

-(instancetype)initWithMask:(NSString*)mask text:(NSString*)text url:(NSString*)url;


// MARK: - Methods

+ (NSString *)configurePopUpHtmlContentWithContent:(NSString *)content hyperlinks:(NSArray *)hyperlinks;

@end
