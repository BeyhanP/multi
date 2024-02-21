#import "View.h"
#import "Label.h"
#import "Fonts.h"
#import "Colors.h"
#import "WarningViewModel.h"

@interface WarningView : View

// MARK: - Properties

@property(nonatomic, readwrite) UIStackView* containerStackView;
@property(nonatomic, readwrite) UIImageView* iconImageView;
@property(nonatomic, readwrite) Label* contentLabel;


// MARK: - Setup

-(void)setupContainerStackView;
-(void)setupIconImageView;
-(void)setupContentLabel;


// MARK: - Configure

-(void)configureWithModel:(WarningViewModel*)model;
-(void)configureContentLabelWithContent:(NSString*)content;

@end
