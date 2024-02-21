#import "UIKit/UIKit.h"
#import "ComplianceAction.h"

@interface Button : UIButton

// MARK: - Properties

@property(nonatomic, readwrite) ComplianceAction* complianceAction;


// MARK: - Initializers

-(instancetype) initWithTitle:(NSString*)title titleColor:(UIColor*)titleColor backgroundColor:(UIColor*)backgroundColor;
-(instancetype) initWithAction:(ComplianceAction*)action titleColor:(UIColor*)titleColor backgroundColor:(UIColor*)backgroundColor;


// MARK: - Methods

-(void) setTitle:(NSString*)title;
-(void) setTitleColor:(UIColor*)color;
-(void) setBorderWithWidth:(CGFloat)width color:(UIColor*)color;

@end
