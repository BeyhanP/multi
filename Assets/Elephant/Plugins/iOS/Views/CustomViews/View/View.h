#import "UIKit/UIKit.h"

@interface View : UIView

// MARK: - Initializers

-(id) initWithCornerRadius:(CGFloat)radius backgroundColor:(UIColor*)backgroundColor;


// MARK: - Methods

-(void) setCornerRadiusWithValue:(CGFloat) radius;

@end
