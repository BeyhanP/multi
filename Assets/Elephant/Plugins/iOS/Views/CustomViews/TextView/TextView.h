#import <UIKit/UIKit.h>
#import "Constants.h"
#import "Colors.h"
#import "Fonts.h"
#import "Utils.h"

@interface TextView: UITextView <UITextViewDelegate>

// MARK: - Initializers

-(instancetype) initWithText:text textColor:(UIColor*)textColor font:(UIFont*)font hyperlinks:(Dictionary*) hyperlinks;
-(instancetype) initWithHtmlString:(NSString*)html;


// MARK: - Methods

-(void)setTextWithHtmlString:(NSString*)html;
-(void)setText:(NSString*)text textColor:(UIColor*)textColor font:(UIFont*)font hyperlinks:(Dictionary*)hyperlinks;
-(void)setLinkForAttributedString:(NSMutableAttributedString*) attributedString hyperlinks:(Dictionary*) hyperlinks;
-(void)setLinkForAttributedString:(NSMutableAttributedString*) attributedString text:(NSString*)text url:(NSString*) url;

@end

