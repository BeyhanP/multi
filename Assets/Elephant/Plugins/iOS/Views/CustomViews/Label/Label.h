#import <UIKit/UIKit.h>

@interface Label : UILabel

// MARK: - Properties

@property(nonatomic, readwrite) UIEdgeInsets edgeInsets;


// MARK: - Initializers

-(instancetype)initWithTop:(CGFloat)top left:(CGFloat)left bottom:(CGFloat)bottom right:(CGFloat)right;

@end
