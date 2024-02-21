#import "RollicAdsController.h"
#import <StoreKit/StoreKit.h>
#import <FBAudienceNetwork/FBAdSettings.h>


@implementation RollicAdsController

void updateConversionValue(int value) {
    if (@available(iOS 14.0, *)) {
        [SKAdNetwork updateConversionValue:value];
    }
}

void setTrackingEnabled(bool isEnabled) {
    [FBAdSettings setAdvertiserTrackingEnabled:isEnabled];
}

float getPixelValue(float point) {
    return point * UIScreen.mainScreen.scale;
}

@end

