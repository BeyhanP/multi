
#import <Foundation/Foundation.h>

NS_ASSUME_NONNULL_BEGIN

@interface RollicAdsController : NSObject
@end

#ifdef __cplusplus
extern "C" {
#endif

void updateConversionValue(int value);
void setTrackingEnabled(bool isEnabled);
float getPixelValue(float point);

const char * RollicAdsCopyString(const char * string)
{
   char * copy = (char*)malloc(strlen(string) + 1);
   strcpy(copy, string);
   return copy;
}

#ifdef __cplusplus
} // extern "C"
#endif

NS_ASSUME_NONNULL_END
