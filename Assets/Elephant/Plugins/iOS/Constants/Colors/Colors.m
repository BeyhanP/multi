#import "Colors.h"

@implementation Colors

+ (UIColor *)containerViewBackground {
    return [[UIColor alloc] initWithRed:220.0/255.0 green:222.0/255.0 blue:223.0/255.0 alpha:1.0];
}

+ (UIColor *)textViewText {
    return [[UIColor alloc] initWithRed:15.0/255.0 green:15.0/255.0 blue:15.0/255.0 alpha:1.0];
}

+ (UIColor *)textViewLink {
    return [UIColor systemBlueColor];
}

+ (UIColor *)separatorBackground {
    return [[UIColor alloc] initWithRed:185.0/255.0 green:190.0/255.0 blue:190.0/255.0 alpha:1.0];
}

+ (UIColor *)buttonTitle {
    return [UIColor systemBlueColor];
}

+ (UIColor *)buttonBackground {
    return [UIColor clearColor];
}

+ (UIColor *)settingsButtonTitle {
    return [UIColor blackColor];
}

+ (UIColor *)settingsButtonBackground {
    return [[UIColor alloc] initWithRed:220.0/255.0 green:222.0/255.0 blue:223.0/255.0 alpha:1.0];
}

+ (UIColor *)settingsButtonBorder {
    return [[UIColor alloc] initWithRed:185.0/255.0 green:190.0/255.0 blue:190.0/255.0 alpha:1.0];
}

+ (UIColor *)loadingContainerViewBackground {
    return [[UIColor alloc] initWithRed:60.0/255.0 green:60.0/255.0 blue:60.0/255.0 alpha:1.0];
}

+ (UIColor *)warningViewText {
    return [[UIColor alloc] initWithRed:65.0/255.0 green:65.0/255.0 blue:65.0/255.0 alpha:1.0];
}

@end
