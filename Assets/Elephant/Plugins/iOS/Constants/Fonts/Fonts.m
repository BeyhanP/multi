#import "Fonts.h"

@implementation Fonts

+ (UIFont *)popupTitle {
    return [UIFont systemFontOfSize:17.0 weight:UIFontWeightBold];
}

+ (UIFont *)textViewText {
    return [UIFont systemFontOfSize:13.0];
}

+ (UIFont *)buttonTitle {
    return [UIFont systemFontOfSize:17.0 weight:UIFontWeightMedium];
}

+ (UIFont *)settingsButtonTitle {
    return [UIFont systemFontOfSize:20.0 weight:UIFontWeightBold];
}

+ (UIFont *)warningViewText {
    return [UIFont systemFontOfSize:11.0];
}

@end
