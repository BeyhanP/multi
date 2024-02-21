#import "TextView.h"

@implementation TextView

// MARK: - Initializers

-(id) init {
    self = [super init];
    
    if (self) {
        [self setScrollEnabled:NO];
        [self setSelectable:YES];
        [self setEditable:NO];
        [self setBackgroundColor:[UIColor clearColor]];
        [self setDelegate:self];
        [self setTintColor:[Colors textViewText]];
    }
    
    return self;
}

-(instancetype) initWithText:text textColor:(UIColor*)textColor font:(UIFont*)font hyperlinks:(Dictionary*) hyperlinks {
    self = [self init];
    
    if (self) {
        [self setText:text textColor:textColor font:font hyperlinks:hyperlinks];
    }
    
    return self;
}

- (instancetype)initWithHtmlString:(NSString *)html {
    self = [self init];
    
    if (self) {
        [self setTextWithHtmlString:html];
    }
    
    return self;
}


// MARK: - Methods

-(void)setTextWithHtmlString:(NSString*)html {
    html = [NSString stringWithFormat:@"<span style=\"font-family: -apple-system; font-size: 13.0\"><center>%@</center></span>", html];
    NSAttributedString* attributedString = [[NSAttributedString alloc]
                                            initWithData:[html dataUsingEncoding:NSUnicodeStringEncoding]
                                            options:@{ NSDocumentTypeDocumentAttribute: NSHTMLTextDocumentType }
                                            documentAttributes:nil error:nil];
    
    [self setAttributedText:attributedString];
}

-(void)setText:(NSString *)text textColor:(UIColor*)textColor font:(UIFont*)font hyperlinks:(Dictionary*)hyperlinks {
    NSMutableAttributedString* attributedString = [[NSMutableAttributedString alloc] initWithString:text];
    
    [attributedString addAttributes:@{NSFontAttributeName: font,
                                      NSForegroundColorAttributeName: textColor}
                              range:NSMakeRange(0, text.length)];
    
    [self setLinkForAttributedString:attributedString hyperlinks:hyperlinks];
    [self setAttributedText:attributedString];
    [self setLinkTextAttributes:@{NSForegroundColorAttributeName: [Colors textViewLink],
                                  NSUnderlineStyleAttributeName: @(NSUnderlineStyleSingle),
                                  NSUnderlineColorAttributeName: [Colors textViewLink]}];
}

-(void)setLinkForAttributedString:(NSMutableAttributedString*) attributedString hyperlinks:(Dictionary*) hyperlinks {
    for (id link in hyperlinks.allKeys) {
        [self setLinkForAttributedString:attributedString text:link url:hyperlinks[link]];
    }
}

-(void)setLinkForAttributedString:(NSMutableAttributedString*) attributedString text:(NSString *)text url:(NSString *)url {
    NSRange range = [attributedString.string rangeOfString:text];
    
    [attributedString addAttribute:NSLinkAttributeName value:url range:range];
}


// MARK: - UITextViewDelegate

- (BOOL)textView:(UITextView *)textView shouldInteractWithURL:(NSURL *)URL inRange:(NSRange)characterRange interaction:(UITextItemInteraction)interaction {
    [Utils presentURL:URL];
    
    return false;
}

@end
