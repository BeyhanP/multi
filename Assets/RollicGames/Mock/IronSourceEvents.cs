using System;
using System.Collections.Generic;
public class IronSourceEvents
{
    public static event Action<IronSourceImpressionData> onImpressionDataReadyEvent;
    public static event Action<IronSourceImpressionData> onImpressionSuccessEvent;
    
    public static event Action onSdkInitializationCompletedEvent;

    public static event Action<IronSourceError> onRewardedVideoAdShowFailedEvent;
    public static event Action onRewardedVideoAdOpenedEvent;
    public static event Action onRewardedVideoAdClosedEvent;
    public static event Action onRewardedVideoAdStartedEvent;
    public static event Action onRewardedVideoAdEndedEvent;
    public static event Action<IronSourcePlacement> onRewardedVideoAdRewardedEvent;
    public static event Action<IronSourcePlacement> onRewardedVideoAdClickedEvent;
    public static event Action<bool> onRewardedVideoAvailabilityChangedEvent;

    public static event Action<IronSourceError> onRewardedVideoAdLoadFailedEvent;
    public static event Action onRewardedVideoAdReadyEvent;

    public static event Action<string> onRewardedVideoAdOpenedDemandOnlyEvent;
    public static event Action<string> onRewardedVideoAdClosedDemandOnlyEvent;
    public static event Action<string> onRewardedVideoAdLoadedDemandOnlyEvent;
    public static event Action<string> onRewardedVideoAdRewardedDemandOnlyEvent;
    public static event Action<string, IronSourceError> onRewardedVideoAdShowFailedDemandOnlyEvent;
    public static event Action<string> onRewardedVideoAdClickedDemandOnlyEvent;
    public static event Action<string, IronSourceError> onRewardedVideoAdLoadFailedDemandOnlyEvent;

    public static event Action onInterstitialAdReadyEvent;
    public static event Action<IronSourceError> onInterstitialAdLoadFailedEvent;
    public static event Action onInterstitialAdOpenedEvent;
    public static event Action onInterstitialAdClosedEvent;
    public static event Action onInterstitialAdShowSucceededEvent;
    public static event Action<IronSourceError> onInterstitialAdShowFailedEvent;
    public static event Action onInterstitialAdClickedEvent;

    public static event Action<string> onInterstitialAdReadyDemandOnlyEvent;
    public static event Action<string> onInterstitialAdOpenedDemandOnlyEvent;
    public static event Action<string> onInterstitialAdClosedDemandOnlyEvent;
    public static event Action<string, IronSourceError> onInterstitialAdLoadFailedDemandOnlyEvent;
    public static event Action<string> onInterstitialAdClickedDemandOnlyEvent;
    public static event Action<string, IronSourceError> onInterstitialAdShowFailedDemandOnlyEvent;

    public static event Action<bool> onOfferwallAvailableEvent;
    public static event Action onOfferwallOpenedEvent;
    public static event Action<Dictionary<string, object>> onOfferwallAdCreditedEvent;
    public static event Action<IronSourceError> onGetOfferwallCreditsFailedEvent;
    public static event Action onOfferwallClosedEvent;
    public static event Action<IronSourceError> onOfferwallShowFailedEvent;

    public static event Action onBannerAdLoadedEvent;
    public static event Action onBannerAdLeftApplicationEvent;
    public static event Action onBannerAdScreenDismissedEvent;
    public static event Action onBannerAdScreenPresentedEvent;
    public static event Action onBannerAdClickedEvent;
    public static event Action<IronSourceError> onBannerAdLoadFailedEvent;

    public static event Action<string> onSegmentReceivedEvent;
}