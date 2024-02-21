#import "BasePopUpViewModel.h"
#import "Hyperlink.h"

@interface PopUpViewModel : BasePopUpViewModel

// MARK: - Properties

@property(nonatomic, readwrite) NSString* title;
@property(nonatomic, readwrite) NSString* text;
@property(nonatomic, readwrite) NSString* buttonTitle;
@property(nonatomic, readwrite) NSArray<Hyperlink*>* hyperlinks;


// MARK: - Initializers

-(instancetype)initWithText:(NSString*)content buttonTitle:(NSString*)buttonTitle interactable:(id<Interactable>)interactable;
-(instancetype)initWithText:(NSString*)text buttonTitle:(NSString*)buttonTitle hyperlinks:(NSArray<Hyperlink*>*)hyperlinks interactable:(id<Interactable>)interactable;
-(instancetype)initWithTitle:(NSString*)title text:(NSString*)text buttonTitle:(NSString*)buttonTitle hyperlinks:(NSArray<Hyperlink*>*)hyperlinks interactable:(id<Interactable>)interactable;

@end
