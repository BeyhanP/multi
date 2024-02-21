#import "ElephantController.h"
#import "IdfaConsentViewController.h"
#import <AdSupport/AdSupport.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>
#import <mach/mach.h>

@implementation ElephantController



void TestFunction(const char * a){
    if(a != nullptr)
        NSLog(@"From Unity -> %s", a);
}

void showForceUpdate(const char * _ttl, const char * _msg) {
     const char * _titleTextCopy = ElephantCopyString(_ttl);
     const char * _messsageTextCopy = ElephantCopyString(_msg);
     
     
     NSString *ttlString = [NSString stringWithCString:_titleTextCopy encoding:NSUTF8StringEncoding];
     NSString *msgString = [NSString stringWithCString:_messsageTextCopy encoding:NSUTF8StringEncoding];
 
     IdfaConsentViewController *viewController = [IdfaConsentViewController sharedInstance];
     [viewController showForceUpdate:ttlString :msgString];
}

void showAlertDialog(const char * _ttl, const char * _msg) {
     const char * _titleTextCopy = ElephantCopyString(_ttl);
     const char * _messsageTextCopy = ElephantCopyString(_msg);
     
     
     NSString *ttlString = [NSString stringWithCString:_titleTextCopy encoding:NSUTF8StringEncoding];
     NSString *msgString = [NSString stringWithCString:_messsageTextCopy encoding:NSUTF8StringEncoding];
 
     IdfaConsentViewController *viewController = [IdfaConsentViewController sharedInstance];
     [viewController showAlertDialog:ttlString :msgString];
 }

void showIdfaConsent(int type, int delay, int position, const char * titleText,
                     const char * descriptionText,
                     const char * buttonText,
                     const char * termsText,
                     const char * policyText,
                     const char * termsUrl,
                     const char * policyUrl) {
    const char * titleTextCopy = ElephantCopyString(titleText);
    const char * descriptionTextCopy = ElephantCopyString(descriptionText);
    const char * buttonTextCopy = ElephantCopyString(buttonText);
    const char * termsTextCopy = ElephantCopyString(termsText);
    const char * policyTextCopy = ElephantCopyString(policyText);
    const char * termsUrlCopy = ElephantCopyString(termsUrl);
    const char * policyUrlCopy = ElephantCopyString(policyUrl);
    
    NSString *title = [NSString stringWithCString:titleTextCopy encoding:NSUTF8StringEncoding];
    NSString *description = [NSString stringWithCString:descriptionTextCopy encoding:NSUTF8StringEncoding];
    NSString *button = [NSString stringWithCString:buttonTextCopy encoding:NSUTF8StringEncoding];
    NSString *terms = [NSString stringWithCString:termsTextCopy encoding:NSUTF8StringEncoding];
    NSString *policy = [NSString stringWithCString:policyTextCopy encoding:NSUTF8StringEncoding];
    NSString *termsUrlString = [NSString stringWithCString:termsUrlCopy encoding:NSUTF8StringEncoding];
    NSString *policyUrlString = [NSString stringWithCString:policyUrlCopy encoding:NSUTF8StringEncoding];
    
    dispatch_async(dispatch_get_main_queue(), ^(void){
        if (@available(iOS 14.0, *)) {
            
            NSString * status = [NSString stringWithUTF8String:getConsentStatus()];
            if ([status isEqualToString:@"NotDetermined"]) {
                IdfaConsentViewController *viewController = [IdfaConsentViewController sharedInstance];
                switch (type) {
                    case 0:
                        [viewController requstPermissionForAppTracking];
                        break;
                    case 1:
                        [viewController presentConsentView];
                        break;
                    case 2:
                        [viewController presentConsentView2];
                        break;
                    case 3:
                        [viewController presentConsentView3:title :delay :position :description :button :terms :policy :termsUrlString :policyUrlString];
                        break;
                    default:
                        [viewController requstPermissionForAppTracking];
                        break;
                }
            } else {
                UnitySendMessage("Elephant", "triggerConsentResult", status.UTF8String);
            }
        } else {
            UnitySendMessage("Elephant", "triggerConsentResult", "");
        }
    });
}

const char* IDFA(){
    NSString *emptyUserIdfa = @"00000000-0000-0000-0000-000000000000";
    NSUUID *u = [[ASIdentifierManager sharedManager] advertisingIdentifier];
    const char *a = [[u UUIDString] cStringUsingEncoding:NSUTF8StringEncoding];
    if ([emptyUserIdfa isEqualToString:[NSString stringWithUTF8String:a]]) {
        return ElephantCopyString("");
    } else {
        return ElephantCopyString(a);
    }
}

const char* getBuildNumber() {
    NSDictionary *infoDict = [[NSBundle mainBundle] infoDictionary];
    NSString *buildNumber = [infoDict objectForKey:@"CFBundleVersion"];
    
    return ElephantCopyString(buildNumber.UTF8String);
}

const char* getLocale() {
    NSString * language = [[NSLocale preferredLanguages] firstObject];
    return ElephantCopyString(language.UTF8String);
}

const char* getConsentStatus(){
    NSString *statusText = @"NotDetermined";
    if (@available(iOS 14.0, *)) {
       ATTrackingManagerAuthorizationStatus status = [ATTrackingManager trackingAuthorizationStatus];
       switch (status) {
           case ATTrackingManagerAuthorizationStatusAuthorized:
               statusText = @"Authorized";
               break;
           case ATTrackingManagerAuthorizationStatusDenied:
               statusText = @"Denied";
               break;
           case ATTrackingManagerAuthorizationStatusRestricted:
               statusText = @"Restricted";
               break;
           case ATTrackingManagerAuthorizationStatusNotDetermined:
               statusText = @"NotDetermined";
               break;
           default:
               statusText = @"NotDetermined";
               break;
       }
    }
    return ElephantCopyString(statusText.UTF8String);
}

void ElephantPost(const char * _url, const char * _body, const char * _gameID, const char * _authToken, int tryCount){


    
            const char * url1 = ElephantCopyString(_url);
            const char * body1 = ElephantCopyString(_body);
            const char * gameID1 = ElephantCopyString(_gameID);
            const char * authToken1 = ElephantCopyString(_authToken);
            
            NSString *urlSt = [NSString stringWithCString:url1 encoding:NSUTF8StringEncoding];
            NSString *body = [NSString stringWithCString:body1 encoding:NSUTF8StringEncoding];
            NSString *gameID = [NSString stringWithCString:gameID1 encoding:NSUTF8StringEncoding];
            NSString *authToken = [NSString stringWithCString:authToken1 encoding:NSUTF8StringEncoding];
            
            NSError *error;

            NSURL *url = [NSURL URLWithString:urlSt];
            NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:url
                                                                   cachePolicy:NSURLRequestUseProtocolCachePolicy
                                                               timeoutInterval:300.0];

            [request addValue:@"gzip" forHTTPHeaderField:@"Content-Encoding"];
            [request addValue:@"application/json; charset=utf-8" forHTTPHeaderField:@"Content-Type"];
            [request addValue:authToken forHTTPHeaderField:@"Authorization"];
            [request addValue:gameID forHTTPHeaderField:@"GameID"];

            [request setHTTPMethod:@"POST"];

            NSData *requestBodyData = [body dataUsingEncoding:NSUTF8StringEncoding];
            [request setHTTPBody:requestBodyData];


            NSURLSessionDataTask *postDataTask = [[NSURLSession sharedSession] dataTaskWithRequest:request completionHandler:^(NSData *data, NSURLResponse *response, NSError *error) {
                bool failed = false;
                bool isOffline = false;
                long statusCode = -1;
                if(error != nil){
                    if (error.code == -1009) {
                        isOffline = true;
                    }
                    failed = true;
                }
                else if ([response isKindOfClass:[NSHTTPURLResponse class]]){
                    NSHTTPURLResponse *r = (NSHTTPURLResponse*)response;
                    if(r.statusCode != 200){
                        statusCode = r.statusCode;
                        failed = true;
                    }
                }
                
                if(failed){
                    NSDictionary *failedReq = @{ @"url": urlSt, @"data": body, @"tryCount": [NSNumber numberWithInt:tryCount],
                                                 @"isOffline": isOffline ? @"true" : @"false" , @"statusCode": [NSString stringWithFormat:@"%ld", statusCode] };
                            NSData *jsonData = [NSJSONSerialization dataWithJSONObject:failedReq options:NSJSONWritingPrettyPrinted error:nil];
                            NSString *jsonSt = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                            UnitySendMessage("Elephant", "FailedRequest", [jsonSt cStringUsingEncoding:NSUTF8StringEncoding]);
                } else {
                   if (data != nil &&  data.length > 0) {
                       NSData *jsonData = data;
                       NSString *jsonSt = [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
                       UnitySendMessage("RLAdvertisementManager", "SuccessResponse", [jsonSt cStringUsingEncoding:NSUTF8StringEncoding]);
                   }
                   
               }
                
            }];

            [postDataTask resume];
            
            
            
            free((void*)url1);
            free((void*)body1);
            free((void*)gameID1);
            free((void*)authToken1);
   
}

void showPopUpView(const char * popUpSubviewType, const char * text, const char * buttonTitle, const char * privacyPolicyText, 
                   const char * privacyPolicyUrl, const char * termsOfServiceText, const char * termsOfServiceUrl,
                    const char * dataRequestText, const char * dataRequestUrl) {
    NSString* textStr = [NSString stringWithCString:text encoding:NSUTF8StringEncoding];
    NSString* buttonTitleStr = [NSString stringWithCString:buttonTitle encoding:NSUTF8StringEncoding];
    NSString* dataRequestTextStr = [NSString stringWithCString:dataRequestText encoding:NSUTF8StringEncoding];
    NSString* dataRequestUrlStr = [NSString stringWithCString:dataRequestUrl encoding:NSUTF8StringEncoding];
    NSString* privacyPolicyTextStr = [NSString stringWithCString:privacyPolicyText encoding:NSUTF8StringEncoding];
    NSString* privacyPolicyUrlStr = [NSString stringWithCString:privacyPolicyUrl encoding:NSUTF8StringEncoding];
    NSString* termsOfServiceTextStr = [NSString stringWithCString:termsOfServiceText encoding:NSUTF8StringEncoding];
    NSString* termsOfServiceUrlStr = [NSString stringWithCString:termsOfServiceUrl encoding:NSUTF8StringEncoding];
    NSString* popUpSubviewTypeStr = [NSString stringWithCString:popUpSubviewType encoding:NSUTF8StringEncoding];
    NSMutableArray<Hyperlink*>* hyperlinks = [[NSMutableArray<Hyperlink*> alloc] initWithObjects:
                                                  [[Hyperlink alloc] initWithMask:[Constants privacyPolicyMask] text:privacyPolicyTextStr url:privacyPolicyUrlStr],
                                                  [[Hyperlink alloc] initWithMask:[Constants termsOfServiceMask] text:termsOfServiceTextStr url:termsOfServiceUrlStr], nil];
    BOOL isPin = ![dataRequestTextStr isEqualToString:@""] && ![dataRequestUrlStr isEqualToString:@""];
    
    if (isPin) {
        [hyperlinks addObject:[[Hyperlink alloc] initWithMask:[Constants dataRequestMask] text:dataRequestTextStr url:dataRequestUrlStr]];
    }
    
    PopUpSubviewType subviewType = (PopUpSubviewType) [Utils getPopUpSubviewTypeFromString:popUpSubviewTypeStr];
    PopUpView* popUpView = [PopUpView createView];
    PopUpViewModel* model = [[PopUpViewModel alloc] initWithText:textStr
                                                     buttonTitle:buttonTitleStr
                                                      hyperlinks:hyperlinks
                                                    interactable:interactableHandler];
    
    [popUpView configureWithModel:model];
    
    popUpView.buttonCallback = ^{
        if (!isPin) {
            [interactableHandler onButtonTapped:TOS_ACCEPT];
        }
    };
    
    [popUpView showWithSubviewType:subviewType];
}

void showCcpaPopUpView(const char * action, const char * title, const char * content,
                       const char * privacyPolicyText, const char * privacyPolicyUrl, 
                       const char * declineActionButtonText, const char * agreeActionButtonText,
                       const char * backToGameActionButtonText) {
    NSString* actionStr = [NSString stringWithCString:action encoding:NSUTF8StringEncoding];
    NSString* titleStr = [NSString stringWithCString:title encoding:NSUTF8StringEncoding];
    NSString* contentStr = [NSString stringWithCString:content encoding:NSUTF8StringEncoding];
    NSString* privacyPolicyTextStr = [NSString stringWithCString:privacyPolicyText encoding:NSUTF8StringEncoding];
    NSString* privacyPolicyUrlStr = [NSString stringWithCString:privacyPolicyUrl encoding:NSUTF8StringEncoding];
    NSString* agreeActionButtonTextStr = [NSString stringWithCString:agreeActionButtonText encoding:NSUTF8StringEncoding];
    NSString* declineActionButtonTextStr = [NSString stringWithCString:declineActionButtonText encoding:NSUTF8StringEncoding];
    NSString* backToGameActionButtonTextStr = [NSString stringWithCString:backToGameActionButtonText encoding:NSUTF8StringEncoding];
    NSArray<Hyperlink*>* hyperlinks = [[NSArray<Hyperlink*> alloc] initWithObjects:
                                           [[Hyperlink alloc] initWithMask:[Constants privacyPolicyMask] text:privacyPolicyTextStr url:privacyPolicyUrlStr],
                                           nil];
    
    ActionType actionEnum = (ActionType) [Utils getActionTypeFromString:actionStr];
    PersonalizedAdsConsentPopUpView* personalizedAdsConsentPopUpView = [PersonalizedAdsConsentPopUpView createView];
    PersonalizedAdsConsentPopUpViewModel* model = [[PersonalizedAdsConsentPopUpViewModel alloc] initWithAction:actionEnum
                                                                                                         title:titleStr
                                                                                                          text:contentStr
                                                                                            declineButtonTitle:declineActionButtonTextStr
                                                                                              agreeButtonTitle:agreeActionButtonTextStr
                                                                                         backToGameButtonTitle:backToGameActionButtonTextStr
                                                                                                    hyperlinks:hyperlinks
                                                                                                  interactable:interactableHandler];
    
    [personalizedAdsConsentPopUpView configureWithModel:model];
    [personalizedAdsConsentPopUpView showWithSubviewType:CONTENT];
}

void showSettingsView(const char * popUpSubviewType, const char * actions) {
    NSString* actionsJson = [NSString stringWithCString:actions encoding:NSUTF8StringEncoding];
    NSString* popUpSubviewTypeStr = [NSString stringWithCString:popUpSubviewType encoding:NSUTF8StringEncoding];
    ComplianceActions* complianceActions = [[ComplianceActions alloc] initWithJSONDictionary:[Utils JSONStringToJSONDictionary:actionsJson]];
    PopUpSubviewType subviewType = (PopUpSubviewType) [Utils getPopUpSubviewTypeFromString:popUpSubviewTypeStr];
    SettingsPopUpViewModel* model = [[SettingsPopUpViewModel alloc] initWithActions:[complianceActions actions]
                                                                       interactable:interactableHandler];
    SettingsView* settingsView = [SettingsView createView];
    
    [settingsView configureWithModel:model];
    [settingsView showWithSubviewType:subviewType];
}

void showBlockedPopUpView(const char * title, const char * content, const char * warningContent, const char * buttonTitle) {
    NSString* titleStr = [NSString stringWithCString:title encoding:NSUTF8StringEncoding];
    NSString* contentStr = [NSString stringWithCString:content encoding:NSUTF8StringEncoding];
    NSString* warningContentStr = [NSString stringWithCString:warningContent encoding:NSUTF8StringEncoding];
    NSString* buttonTitleStr = [NSString stringWithCString:buttonTitle encoding:NSUTF8StringEncoding];
    BlockedPopUpView* deleteRequestPopUpView = [BlockedPopUpView createView];
    BlockedPopUpViewModel* model = [[BlockedPopUpViewModel alloc] initWithTitle:titleStr text:contentStr warningContent:warningContentStr buttonTitle:buttonTitleStr hyperlinks:nil interactable:interactableHandler];
    
    [deleteRequestPopUpView configureWithModel:model];
    [deleteRequestPopUpView showWithSubviewType:CONTENT];
}

void showNetworkOfflinePopUpView(const char * content, const char * buttonTitle) {
    NSString* contentStr = [NSString stringWithCString:content encoding:NSUTF8StringEncoding];
    NSString* buttonTitleStr = [NSString stringWithCString:buttonTitle encoding:NSUTF8StringEncoding];
    PopUpView* popUpView = [PopUpView createView];
    PopUpViewModel* model = [[PopUpViewModel alloc] initWithText:contentStr buttonTitle:buttonTitleStr interactable:interactableHandler];
    
    [popUpView configureWithModel:model];
    
    popUpView.buttonCallback = ^{
        [interactableHandler onButtonTapped:RETRY_CONNECTION];
    };
    
    [popUpView showWithSubviewType:CONTENT];
}

int gameMemoryUsage() {
    task_vm_info_data_t vmInfo;
    mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
    if (task_info(mach_task_self(), TASK_VM_INFO, (task_info_t) &vmInfo, &count) != KERN_SUCCESS) return 0;
    if (vmInfo.phys_footprint <= 0) return 0;
    return (int)(vmInfo.phys_footprint / 1000000);
}

int gameMemoryUsagePercent() {
    int memoryUsage = gameMemoryUsage();
    if (memoryUsage <= 0) return 0;
    int totalMemory = (int)([NSProcessInfo processInfo].physicalMemory / 1000000);
    int memoryUsagePercent = (memoryUsage * 100) / totalMemory;
    return memoryUsagePercent;
}

const long long getFirstInstallTime() {
    NSURL* urlToDocumentsFolder = [[[NSFileManager defaultManager] URLsForDirectory:NSDocumentDirectory inDomains:NSUserDomainMask] lastObject];
        
    if (!urlToDocumentsFolder) return -1;
    
    NSError *error;
    NSDate *installDate = [[[NSFileManager defaultManager] attributesOfItemAtPath:urlToDocumentsFolder.path error:&error] objectForKey:NSFileCreationDate];
    
    if (!installDate || error) return -1;
    
    return [@(floor([installDate timeIntervalSince1970] * 1000)) longLongValue];
}

@end
