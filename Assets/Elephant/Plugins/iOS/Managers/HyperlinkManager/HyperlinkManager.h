#import <Foundation/Foundation.h>
#import "Hyperlink.h"

@protocol HyperlinkManager <NSObject>

-(NSArray<Hyperlink*>*)getHyperlinks;

@end
