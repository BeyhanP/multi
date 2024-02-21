#import <Foundation/Foundation.h>
#import "Interactable.h"

@interface BasePopUpViewModel: NSObject

// MARK: - Properties

@property(nonatomic, weak) id <Interactable> interactable;


// MARK: - Initializers

-(instancetype)initWithInteractable:(id<Interactable>)interactable;

@end
