#import <Foundation/Foundation.h>

@interface WarningViewModel : NSObject

// MARK: - Properties

@property(nonatomic, readwrite) NSString* content;


// MARK: - Initializers

-(instancetype)initWithContent:(NSString*)content;

@end
