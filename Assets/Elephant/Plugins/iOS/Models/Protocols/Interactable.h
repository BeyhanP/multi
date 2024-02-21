#import <Foundation/Foundation.h>
#import "InteractionType.h"

@protocol Interactable <NSObject>

-(void)onButtonTapped:(InteractionType) interactionType;

@end
