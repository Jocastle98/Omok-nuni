using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;



public class AdmobAdsManager : MonoBehaviour
{
#if UNITY_ANDROID
    private string mRewardedAdUnitID = "ca-app-pub-3940256099942544/5224354917";// 보상형 광고 Test ID
    //private string mRewardedAdUnitID = "ca-app-pub-7882694754839983/3707741912" //보상형 광고 ID;
#endif

    private RewardedAd mRewardedAd;
    
    private void Start()
    {
        //구글 모바일 Ads SDK 초기화
        MobileAds.Initialize(initStatus =>
        {
            //보상형광고 표시
            LoadRewardedAd();
        });
    }

    #region Rewarded Ads

    public void LoadRewardedAd()
    {
        if (mRewardedAd != null)
        {
            mRewardedAd.Destroy();
            mRewardedAd = null;
        }

        Debug.Log("보상형광고 로딩중");

        var adRequest = new AdRequest();
        RewardedAd.Load(mRewardedAdUnitID, adRequest, (ad, error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("보상형광고 로드가 실패 :" + error);
                return;
            }

            Debug.Log("보상형 광고 로드:" + ad.GetResponseInfo());
            mRewardedAd = ad;

            RegisterRewardedAdEventHandlers(mRewardedAd);
        });
    }

    public void ShowRewardedAd()
    {
        const string REWARD_MESSAGE = "보상 제공. Type: {0}, Amount : {1}";

        
        
        if (mRewardedAd != null && mRewardedAd.CanShowAd())
        {
            mRewardedAd.Show((Reward reward) =>
            {
                Debug.Log(string.Format(REWARD_MESSAGE, reward.Type, reward.Amount));
            });
        }
    }
    
    private void RegisterRewardedAdEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            
            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
            
            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }

    #endregion
}
