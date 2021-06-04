using System.Collections;
using GoogleMobileAds.Api;
using UnityEngine;

public class AdManager : MonoBehaviour {

    public static AdManager Instance;

    private int _actionCountForDisplayAd = -1;
    private int _actionCountMax = 3;



    private void Awake() {
        Instance = this;
    }

    private void Start() {
        GameManager.RequestInterstitial();
    }


    public static void AdReward_2x(object sender, Reward args) {
        LevelManager.AdReward_2x();
    }


    public static void AdReward_Revived(object sender, Reward args) {
        LevelManager.AdReward_Revived();
    }

    public static void ShowInterstitialAd() {
        Instance._actionCountForDisplayAd++;
        if (GameManager.CheckNetworkConnection()) {
            Debug.Log("Requesting GoogleMobileAd for Displaying Interstitial Ad...");

            if (Instance._actionCountForDisplayAd >= Instance._actionCountMax) {
                Instance._actionCountForDisplayAd = 0;
                GameManager.ShowInterstitial();
            }
        }

    }

    public static void ShowBannerAd() {
        GameManager.RequestBanner(AdSize.IABBanner, AdPosition.Bottom);
    }

    public static void RemoveBannerAd() {
        GameManager.RemoveBanner();
    }






}
