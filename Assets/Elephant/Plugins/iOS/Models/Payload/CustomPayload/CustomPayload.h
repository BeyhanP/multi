#import "BasePayload.h"

@interface CustomPayload: BasePayload

// MARK: - Properties

@property(nonatomic, readwrite) NSString* title;
@property(nonatomic, readwrite) NSString* content;
@property(nonatomic, readwrite) NSString* consent_text_action_button;

@end
