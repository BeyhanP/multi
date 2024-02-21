#import <Foundation/Foundation.h>

@interface ErrorViewModel : NSObject

// MARK: - Properties

@property(nonatomic, readwrite) NSString* message;


// MARK: - Initializers

-(instancetype)initWithMessage:(NSString*)message;

@end
