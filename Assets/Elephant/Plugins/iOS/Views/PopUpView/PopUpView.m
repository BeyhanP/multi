#import "PopUpView.h"
#import "Colors.h"
#import "Fonts.h"

@implementation PopUpView

// MARK: - Initializers

- (instancetype)init {
    self = [super init];
    
    if (self) {
        [self setupTextView];
        [self setupSeparator];
        [self setupButton];
    }
    
    return self;
}


// MARK: - Setup

- (void)setupTextView {
    [self setTextView:[TextView new]];
    
    [[self contentView] addSubview:[self textView]];
    [[self textView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self textView] topAnchor] constraintEqualToAnchor:[[self contentView] topAnchor] constant:10.0] setActive:YES];
    [[[[self textView] leadingAnchor] constraintEqualToAnchor:[[self contentView] leadingAnchor] constant:10.0] setActive:YES];
    [[[[self textView] trailingAnchor] constraintEqualToAnchor:[[self contentView] trailingAnchor] constant:-10.0] setActive:YES];
}

- (void)setupSeparator {
    [self setSeparatorView:[View new]];
    
    [[self separatorView] setBackgroundColor:[Colors separatorBackground]];
    
    [[self contentView] addSubview:[self separatorView]];
    [[self separatorView] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self separatorView] topAnchor] constraintEqualToAnchor:[[self textView] bottomAnchor] constant:10.0] setActive:YES];
    [[[[self separatorView] leadingAnchor] constraintEqualToAnchor:[[self contentView] leadingAnchor]] setActive:YES];
    [[[[self separatorView] trailingAnchor] constraintEqualToAnchor:[[self contentView] trailingAnchor]] setActive:YES];
    [[[[self separatorView] heightAnchor] constraintEqualToConstant:1.0] setActive:YES];
}

- (void)setupButton {
    [self setButton:[Button new]];
    
    [[self button] setTitleColor:[Colors buttonTitle]];
    [[[self button] titleLabel] setFont:[Fonts buttonTitle]];
    [[self button] setBackgroundColor:[Colors buttonBackground]];
    [[self button] addTarget:self action:@selector(buttonTapped:) forControlEvents:UIControlEventTouchUpInside];
    
    [[self contentView] addSubview:[self button]];
    [[self button] setTranslatesAutoresizingMaskIntoConstraints:NO];
    [[[[self button] topAnchor] constraintEqualToAnchor:[[self separatorView] bottomAnchor]] setActive:YES];
    [[[[self button] leadingAnchor] constraintEqualToAnchor:[[self contentView] leadingAnchor]] setActive:YES];
    [[[[self button] trailingAnchor] constraintEqualToAnchor:[[self contentView] trailingAnchor]] setActive:YES];
    [[[[self button] bottomAnchor] constraintEqualToAnchor:[[self contentView] bottomAnchor]] setActive:YES];
    [[[[self button] heightAnchor] constraintGreaterThanOrEqualToConstant:60.0] setActive:YES];
}


// MARK: - Configure

- (void)configureWithModel:(BasePopUpViewModel *)model {
    [super configureWithModel:model];
    
    PopUpViewModel* popUpModel = (PopUpViewModel*) model;
    
    [self configureTextViewWithText:[popUpModel text] hyperlinks:[popUpModel hyperlinks]];
    [self configureButtonWithTitle:[popUpModel buttonTitle]];
}

- (void)configureTextViewWithText:(NSString *)text hyperlinks:(NSArray<Hyperlink *> *)hyperlinks {
    [[self textView] setTextWithHtmlString:[Hyperlink configurePopUpHtmlContentWithContent:text
                                                                                hyperlinks:hyperlinks]];
}

- (void)configureButtonWithTitle:(NSString *)title {
    [[self button] setTitle:title];
}


// MARK: - Actions


- (void)buttonTapped:(UIButton *)sender {
    [self buttonCallback]();
    [self hide];
}


@end
