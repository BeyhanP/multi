#import "PopUpViewModel.h"

@interface BlockedPopUpViewModel : PopUpViewModel

// MARK: - Properties

@property(nonatomic, readwrite) NSString* warningContent;


// MARK: - Initializers

-(instancetype)initWithTitle:(NSString*)title text:(NSString*)text warningContent:(NSString*)warningContent buttonTitle:(NSString*)buttonTitle hyperlinks:(NSArray<Hyperlink*>*)hyperlinks interactable:(id<Interactable>)interactable;

@end
