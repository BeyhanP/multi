#import "BasePopUpViewModel.h"
#import "ComplianceAction.h"
#import "Interactable.h"

@interface SettingsPopUpViewModel : BasePopUpViewModel

// MARK: - Properties

@property(nonatomic, readwrite) NSArray<ComplianceAction*>* actions;


// MARK: - Initializers

-(instancetype)initWithActions:(NSArray<ComplianceAction *> *)actions interactable:(id<Interactable>)interactable;

@end
