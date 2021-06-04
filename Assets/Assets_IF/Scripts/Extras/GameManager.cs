using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GoogleMobileAds.Api;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//using UnityEngine.Purchasing;
using InvisibleFiction;

namespace InvisibleFiction {

    #region ENUMS_DATATYPES
    public enum LevelState {
        IDLE, LOADING, PLAYING, PAUSED, FINISHED, CLEARED, FAILED
    }

    public enum Sounds {
        btnClicked, bgMusic, levelFailed, levelCleared, levelFinished, obstacleHit, obstacleHitWrong, obstacleShattered, pickUpGemPiles,
    }

    namespace TwistHit {
        public enum IFColor { White, Red, Yellow, Blue, Green, Purple, Orange, Brown, Black }

        public enum ObstacleType { side4, side5, side6, side7, side8, side9, side10 }



    };

    #endregion

    #region EXTENSION METHODS
    public static class ExtensionMethods {

        public static void ResetMaterial(this Button _btn, bool _setDefaultMaterial = true) {
            if (_setDefaultMaterial) {
                _btn.GetComponent<Image>().material = UIManager.Instance.MaterialSprite_Default;
            } else {
                _btn.GetComponent<Image>().material = UIManager.Instance.MaterialSprite_Zoom;
            }
        }

        public static Vector3 ToVector3(this string str) {
            if (str.StartsWith("(") && str.EndsWith(")")) {
                str = str.Substring(1, str.Length - 2);
            }
            string[] sArray = str.Split(',');
            Vector3 result = new Vector3(
                float.Parse(sArray[0]),
                float.Parse(sArray[1]),
                float.Parse(sArray[2]));
            return result;
        }

        public static void LerpTransform(this Transform t1, Transform t2, float t) {
            t1.position = Vector3.Lerp(t1.position, t2.position, t);
            t1.rotation = Quaternion.Lerp(t1.rotation, t2.rotation, t);
            t1.localScale = Vector3.Lerp(t1.localScale, t2.localScale, t);
        }

        public static void SetLeft(this RectTransform rt, float left) {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right) {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top) {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom) {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }

        public static void Set(this RectTransform rt, float top, float right, float bottom, float left) {
            rt.offsetMin = new Vector2(left, bottom);
            rt.offsetMax = new Vector2(right, top);
        }

    }

    #endregion

    #region EDITOR METHODS
#if UNITY_EDITOR
    // Editor specific code here
    public class DeletePlayerPrefsScript : EditorWindow {
        [MenuItem("Window/Delete PlayerPrefs (All)")]
        static void DeleteAllPlayerPrefs() {
            if (EditorUtility.DisplayDialog("Delete all Player Preferences.",
                "Are you sure you want to delete all the Player Preferences? " +
                "This action cannot be undone.", "Yes", "No")) {
                Debug.LogError("All PlayerPrefs Deleted !!!");
                PlayerPrefs.DeleteAll();
            }
        }
    }
#endif
    #endregion


};

public class GameManager : MonoBehaviour {

    #region AD

    #region AD_DATA
    private static BannerView AD_bannerView;
    private static InterstitialAd AD_interstitial;
    private static RewardedAd AD_rewardedAd;

#if UNITY_ANDROID
    private const string AD_STAG_APPID_ANDROID = "ca-app-pub-2187784968688148~6881560789";
    private const string AD_STAG_BANNER_ID_ANDROID = "ca-app-pub-3940256099942544/6300978111";
    private const string AD_STAG_INTERSTITIAL_ID_ANDROID = "ca-app-pub-3940256099942544/1033173712";
    private const string AD_STAG_REWARD_ID_ANDROID = "ca-app-pub-3940256099942544/5224354917";

#elif UNITY_IPHONE
    private const string AD_STAG_APPID_IOS = "ca-app-pub-3940256099942544~1458002511";
    private const string AD_STAG_BANNER_ID_IOS = "ca-app-pub-3940256099942544/2934735716";
    private const string AD_STAG_INTERSTITIAL_ID_IOS = "ca-app-pub-3940256099942544/4411468910";
    private const string AD_STAG_REWARD_ID_IOS = "ca-app-pub-3940256099942544/1712485313";
#endif

#if UNITY_ANDROID
    private const string AD_REAL_APPID_ANDROID = "ca-app-pub-2187784968688148~6881560789";
    private const string AD_REAL_BANNER_ID_ANDROID = "ca-app-pub-3940256099942544/6300978111";
    private const string AD_REAL_INTERSTITIAL_ID_ANDROID = "ca-app-pub-3940256099942544/1033173712";
    private const string AD_REAL_REWARD_ID_ANDROID = "ca-app-pub-3940256099942544/5224354917";

#elif UNITY_IPHONE
    private const string AD_REAL_APPID_IOS = "ca-app-pub-3940256099942544~1458002511";
    private const string AD_REAL_BANNER_ID_IOS = "ca-app-pub-3940256099942544/2934735716";
    private const string AD_REAL_INTERSTITIAL_ID_IOS = "ca-app-pub-3940256099942544/4411468910";
    private const string AD_REAL_REWARD_ID_IOS = "ca-app-pub-3940256099942544/1712485313";
#endif
    private static string AD_APPID;
    private static string AD_BANNER_ID;
    private static string AD_INTERSTITIAL_ID;
    private static string AD_REWARD_ID;

    [Obsolete]
    private static void InitializeAdData() {
        Debug.Log("Setting Advertisment IDs according to end-device platform architecture");
#if UNITY_EDITOR
        AD_APPID = "unexpected_platform";
        AD_BANNER_ID = "unexpected_platform";
        AD_INTERSTITIAL_ID = "unexpected_platform";
        AD_REWARD_ID = "unexpected_platform";
#elif UNITY_ANDROID
        if (TESTBUILD) {
            AD_APPID = AD_STAG_APPID_ANDROID;
            AD_BANNER_ID = AD_STAG_BANNER_ID_ANDROID;
            AD_INTERSTITIAL_ID = AD_STAG_INTERSTITIAL_ID_ANDROID;
            AD_REWARD_ID = AD_STAG_REWARD_ID_ANDROID;
        } else {
            AD_APPID = AD_REAL_APPID_ANDROID;
            AD_BANNER_ID = AD_REAL_BANNER_ID_ANDROID;
            AD_INTERSTITIAL_ID = AD_REAL_INTERSTITIAL_ID_ANDROID;
            AD_REWARD_ID = AD_REAL_REWARD_ID_ANDROID;
        }
#elif UNITY_IPHONE
        if (TESTBUILD) {
            AD_APPID = AD_STAG_APPID_IOS;
            AD_BANNER_ID = AD_STAG_BANNER_ID_IOS;
            AD_INTERSTITIAL_ID = AD_STAG_INTERSTITIAL_ID_IOS;
            AD_REWARD_ID = AD_STAG_REWARD_ID_IOS;
        } else {
            AD_APPID = AD_REAL_APPID_IOS;
            AD_BANNER_ID = AD_REAL_BANNER_ID_IOS;
            AD_INTERSTITIAL_ID = AD_REAL_INTERSTITIAL_ID_IOS;
            AD_REWARD_ID = AD_REAL_REWARD_ID_IOS;
        }
#endif
        Debug.Log("TESTBUILD : " + TESTBUILD);
        Debug.Log("AD_APPID : " + AD_APPID);
        Debug.Log("AD_BANNER_ID : " + AD_BANNER_ID);
        Debug.Log("AD_INTERSTITIAL_ID : " + AD_INTERSTITIAL_ID);
        Debug.Log("AD_REWARD_ID : " + AD_REWARD_ID);
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.Initialize(appId: AD_APPID); // Initialize the Google Mobile Ads SDK.
        CreateAndLoadRewardedAd();
        //RequestInterstitial();
    }

    #endregion // AD_DATA

    #region AD_FUNCTIONS
    private static AdRequest CreateAdRequest() {
        return new AdRequest.Builder()
            .AddTestDevice(AdRequest.TestDeviceSimulator)
            .AddTestDevice("0123456789ABCDEF0123456789ABCDEF")
            .AddKeyword("game")
            .SetGender(Gender.Male)
            .SetBirthday(new DateTime(1985, 1, 1))
            .TagForChildDirectedTreatment(false)
            .AddExtra("color_bg", "9B30FF")
            .Build();
    }

    public static void RequestBanner(AdSize adsize, AdPosition adPosition) {
        if (PlayerPrefs.GetInt("NoAds") == 1) {
            // If NO-Ads has been purchased by user.
            Debug.Log("NO-ADS Purchased by user.");
        } else {
            Debug.Log("Requested Banner-Ad to show");
            if (AD_bannerView != null) {
                AD_bannerView.Destroy(); // Clean up banner ad before creating a new one.
            }

            //AD_bannerView = new BannerView(AD_BANNER_ID, AdSize.Banner, AdPosition.Bottom); // Create a 320x50 banner at the top of the screen.
            AD_bannerView = new BannerView(AD_BANNER_ID, adsize, adPosition);

            AdRequest request = new AdRequest.Builder() // Create an empty ad request.
                .AddTestDevice("8dec72667d2a441183357a03a5417ceb")
                .Build();
            AD_bannerView.LoadAd(request);
            // Register for ad events.
            AD_bannerView.OnAdLoaded += HandleAdLoaded;
            AD_bannerView.OnAdFailedToLoad += HandleAdFailedToLoad;
            AD_bannerView.OnAdOpening += HandleAdOpened;
            AD_bannerView.OnAdClosed += HandleAdClosed;
            AD_bannerView.OnAdLeavingApplication += HandleAdLeftApplication;
        }

    }

    public static void RemoveBanner() {
        Debug.Log("Removing Banner-Ad if any displayed");
        if (AD_bannerView != null) {
            AD_bannerView.Destroy();
        }
    }

    public static void RequestInterstitial() {
        Debug.Log("Request Interstitial Ad...");
        if (AD_interstitial != null) { AD_interstitial.Destroy(); } // Clean up interstitial ad before creating a new one.
        AD_interstitial = new InterstitialAd(AD_INTERSTITIAL_ID); // Create an interstitial.
        // Register for ad events.
        AD_interstitial.OnAdLoaded += HandleInterstitialLoaded;
        AD_interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
        AD_interstitial.OnAdOpening += HandleInterstitialOpened;
        AD_interstitial.OnAdClosed += HandleInterstitialClosed;
        AD_interstitial.OnAdLeavingApplication += HandleInterstitialLeftApplication;
        AD_interstitial.LoadAd(CreateAdRequest()); // Load an interstitial ad.

    }

    public static void CreateAndLoadRewardedAd() {
        AD_rewardedAd = new RewardedAd(AD_REWARD_ID); // Create new rewarded ad instance.
        AD_rewardedAd.OnAdLoaded += HandleRewardedAdLoaded; // Called when an ad request has successfully loaded.
        AD_rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad; // Called when an ad request failed to load.
        AD_rewardedAd.OnAdOpening += HandleRewardedAdOpening; // Called when an ad is shown.
        AD_rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow; // Called when an ad request failed to show.
        //AD_rewardedAd.OnUserEarnedReward += HandleUserEarnedReward; // (Issue adding two callbacks, keep it in comments only)  Called when the user should be rewarded for interacting with the ad.
        AD_rewardedAd.OnAdClosed += HandleRewardedAdClosed; // Called when the ad is closed.
        AdRequest request = CreateAdRequest(); // Create an empty ad request.
        AD_rewardedAd.LoadAd(request); // Load the rewarded ad with the request.
    }

    public static void ShowInterstitial() {
        if (PlayerPrefs.GetInt("NoAds") == 1) {
            // If NO-Ads has been purchased by user.
            Debug.Log("NO-ADS Purchased by user.");
        } else {
            if (AD_interstitial.IsLoaded()) {
                AD_interstitial.Show();
            } else {
                Debug.Log("Interstitial Ad is not yet ready");
            }
        }

    }

    public static void ShowRewardedAd(System.EventHandler<GoogleMobileAds.Api.Reward> rewardFuctionName) {
        if (AD_rewardedAd.IsLoaded()) {
            Debug.Log("Reward Ad is Loaded");
            AD_rewardedAd.OnUserEarnedReward += rewardFuctionName;
            AD_rewardedAd.Show();
            Debug.Log("Reward Ad is Showing");
        } else {
            Debug.Log("Rewarded ad is not yet ready");
        }

#if UNITY_EDITOR
        Debug.Log("Always returns Video-Ad Reward in Unity-Editor");
        rewardFuctionName(new object(), new Reward());
        CreateAndLoadRewardedAd();
#endif

    }

    #endregion // AD_FUNCTIONS

    #region AD_CALLBACK_HANDLERS
    public static void HandleAdLoaded(object sender, EventArgs args) { Debug.Log("HandleAdLoaded event received"); }
    public static void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) { Debug.Log("HandleFailedToReceiveAd event received with message: " + args.Message); }
    public static void HandleAdOpened(object sender, EventArgs args) { Debug.Log("HandleAdOpened event received"); }
    public static void HandleAdClosed(object sender, EventArgs args) { Debug.Log("HandleAdClosed event received"); }
    public static void HandleAdLeftApplication(object sender, EventArgs args) { Debug.Log("HandleAdLeftApplication event received"); }

    public static void HandleInterstitialLoaded(object sender, EventArgs args) { Debug.Log("HandleInterstitialLoaded event received"); }
    public static void HandleInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args) { Debug.Log("HandleInterstitialFailedToLoad event received with message: " + args.Message); }
    public static void HandleInterstitialOpened(object sender, EventArgs args) { Debug.Log("HandleInterstitialOpened event received"); }
    public static void HandleInterstitialClosed(object sender, EventArgs args) {
        Debug.Log("HandleInterstitialClosed event received");
        RequestInterstitial();
    }
    public static void HandleInterstitialLeftApplication(object sender, EventArgs args) { Debug.Log("HandleInterstitialLeftApplication event received"); }

    public static void HandleRewardedAdLoaded(object sender, EventArgs args) { Debug.Log("HandleRewardedAdLoaded event received"); }
    public static void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args) { Debug.Log("HandleRewardedAdFailedToLoad event received with message: " + args.Message); }
    public static void HandleRewardedAdOpening(object sender, EventArgs args) { Debug.Log("HandleRewardedAdOpening event received"); }
    public static void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args) { Debug.Log("HandleRewardedAdFailedToShow event received with message: " + args.Message); }
    public static void HandleUserEarnedReward(object sender, Reward args) { Debug.Log("HandleRewardedAdRewarded event received for " + args.Amount.ToString() + " " + args.Type); }
    public static void HandleRewardedAdClosed(object sender, EventArgs args) {
        Debug.Log("HandleRewardedAdClosed event received");
        CreateAndLoadRewardedAd();
    }

    #endregion // AD_CALLBACK_HANDLERS

    #endregion // AD


    #region DIRECTORY
    public static string GetPath(string fileName) {
#if UNITY_EDITOR
        return Application.persistentDataPath + "/" + fileName;
#elif UNITY_ANDROID
        return Application.persistentDataPath + fileName;
#elif UNITY_IPHONE
        return Application.persistentDataPath + "/" + fileName;
#else
        return Application.persistentDataPath + "/" + fileName;
#endif

    }

    #endregion


    #region GAME_DETAIL
    public const string GAME_NAME = "Color Twist Hit";
    public const string GAME_PRIVACY_POLICY_LINK = "http://www.invisiblefiction.com/privacy-policy.html";
    public const string GAME_PLAYSTORE_LINK = "https://play.google.com/store/apps/details?id=com.IF.rageball";
    public const string GAME_APPLESTORE_LINK = "https://play.google.com/store/apps/details?id=com.IF.rageball";
    public static string GAME_DOWNLOAD_LINK { get; set; }
    public static bool TESTBUILD { get; set; } = true;
    public static bool INITIALIZED { get; set; } = false;


    private static void InitializeGameDetails() {

#if UNITY_EDITOR
        GAME_DOWNLOAD_LINK = "Download from Play Store : " + GAME_PLAYSTORE_LINK + ", Download from App Store : " + GAME_APPLESTORE_LINK;
#elif UNITY_ANDROID
        GAME_DOWNLOAD_LINK = "Download from Play Store : " + GAME_PLAYSTORE_LINK;
#elif UNITY_IPHONE
        GAME_DOWNLOAD_LINK = "Download from App Store : " + GAME_APPLESTORE_LINK;
#endif

    }

    #endregion


    #region GAMEDATA

    public const string str_LEVEL_KEY = "LEVEL";
    public const string str_LAST_LEVEL_KEY = "LEVEL_LAST_UNLOCKED";
    public const string str_TOTAL_GEMS_KEY = "TOTAL_GEMS";
    public const string str_CURRENT_CHARACTER_KEY = "CURRENT_CHARACTER";



    #endregion


    #region GAMEPLAY

    private static void InitializeDebugger(bool debuger = true) {
        GameObject reporter = GameObject.Find("Reporter");
        Debug.unityLogger.logEnabled = false;
        reporter.SetActive(false);

        if (debuger) {
            Debug.unityLogger.logEnabled = true;
            reporter.SetActive(true);
#if UNITY_EDITOR
            Debug.Log("Destroying Reporter in-Unity Editor View !!!");
            Destroy(reporter);
#endif

        } else {
            Debug.unityLogger.logEnabled = false;
            Destroy(reporter);
        }
    }

    [Obsolete]
    public static void InitializeGameManager() {
        if (!INITIALIZED) {
            /*
                if (Debug.isDebugBuild) {
                Debug.Log("GameManager : This is a Development build!");
                TESTBUILD = true;
            } else {
                Debug.Log("GameManager : This is a Release build!");
                TESTBUILD = false;
            } */

            //InitializeDebugger(TESTBUILD);
            InitializeAdData();
            InitializeGameDetails();
            InitializeSoundData();
            //IAPManager.InitializeIAPManager();
            INITIALIZED = true;
        }

        InitializeLevelData();
    }


    #region LEVEL_DATA

    public static LevelState GameState { get; set; }

    #endregion


    #region LEVEL_CALLBACK_HANDLERS
    public delegate void DEL_GameStateChanged();
    public static event DEL_GameStateChanged EVENT_LEVELLOADING;
    public static event DEL_GameStateChanged EVENT_LEVELSTARTED;
    public static event DEL_GameStateChanged EVENT_LEVELPAUSED;
    public static event DEL_GameStateChanged EVENT_LEVELRESUMED;
    public static event DEL_GameStateChanged EVENT_LEVELCLEARED;
    public static event DEL_GameStateChanged EVENT_LEVELFINISHED;
    public static event DEL_GameStateChanged EVENT_LEVELFAILED;

    private static void HANDLER_LevelLoading() { Debug.Log("HANDLER_LevelLoading Event Received"); }
    private static void HANDLER_LevelStarted() { Debug.Log("HANDLER_LevelStarted Event Received"); }
    private static void HANDLER_LevelPaused() { Debug.Log("HANDLER_LevelPaused Event Received"); }
    private static void HANDLER_LevelResumed() { Debug.Log("HANDLER_LevelResumed Event Received"); }
    private static void HANDLER_LevelCleared() { Debug.Log("HANDLER_LevelCleared Event Received"); }
    private static void HANDLER_LevelFinished() { Debug.Log("HANDLER_LevelFinished Event Received"); }
    private static void HANDLER_LevelFailed() { Debug.Log("HANDLER_LevelFailed Event Received"); }

    #endregion


    #region LEVEL_FUNCTIONS
    private static void InitializeLevelData() {
        EVENT_LEVELLOADING += HANDLER_LevelLoading;
        EVENT_LEVELSTARTED += HANDLER_LevelStarted;
        EVENT_LEVELPAUSED += HANDLER_LevelPaused;
        EVENT_LEVELRESUMED += HANDLER_LevelResumed;
        EVENT_LEVELCLEARED += HANDLER_LevelCleared;
        EVENT_LEVELFINISHED += HANDLER_LevelFinished;
        EVENT_LEVELFAILED += HANDLER_LevelFailed;

        UpdateGameState(LevelState.IDLE);
    }


    public static void UpdateGameState(LevelState NewState) {
        if (GameState != NewState) {
            Debug.Log("Changing Level State, From : " + GameState + " => To : " + NewState);
            switch (NewState) {
                case LevelState.PAUSED:
                    EVENT_LEVELPAUSED(); break;

                case LevelState.FAILED:
                    if ((GameState != LevelState.CLEARED) || (GameState != LevelState.FINISHED)) {
                        EVENT_LEVELFAILED();
                    }
                    break;

                case LevelState.CLEARED:
                    if (GameState != LevelState.FAILED) {
                        EVENT_LEVELCLEARED();
                    } else {
                        NewState = GameState;
                    }
                    break;

                case LevelState.FINISHED:
                    if (GameState != LevelState.FAILED) {
                        EVENT_LEVELFINISHED();
                    } else {
                        NewState = GameState;
                    }
                    break;

                case LevelState.PLAYING:
                    if (GameState == LevelState.PAUSED) {
                        EVENT_LEVELRESUMED();

                    } else if (GameState == LevelState.LOADING) {
                        EVENT_LEVELSTARTED();

                    } else if (GameState == LevelState.IDLE) {
                        EVENT_LEVELSTARTED();

                    } else if (GameState == LevelState.FAILED) {
                        EVENT_LEVELRESUMED();
                    }
                    break;

                case LevelState.LOADING:
                    if (GameState == LevelState.FAILED) {
                        EVENT_LEVELSTARTED();

                    } else if (GameState == LevelState.CLEARED) {
                        EVENT_LEVELSTARTED();

                    } else if (GameState == LevelState.PAUSED) {
                        EVENT_LEVELSTARTED();

                    } else {
                        EVENT_LEVELLOADING();
                    }

                    break;
            }

            GameState = NewState;
        } else {
            Debug.Log("In same Level State : => " + GameState);
        }

    }


    #endregion

    #endregion


    #region NETWORK
    public static bool CheckNetworkConnection() {
        bool result = false;
        string m_ReachabilityText = "";
        Debug.Log("Internet : Checking Internet Connectivity");
        if (Application.internetReachability == NetworkReachability.NotReachable) { //Check if the device cannot reach the internet
            m_ReachabilityText = "No Internet Connectivity";
        } else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) { //Check if the device can reach the internet via a carrier data network
            m_ReachabilityText = "Reachable via carrier data network.";
            result = true;
        } else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) { //Check if the device can reach the internet via a LAN
            m_ReachabilityText = "Reachable via Local Area Network.";
            result = true;
        }

        Debug.Log("Internet : " + m_ReachabilityText);
        return result;
    }

    #endregion


    #region SOUND

    #region SOUND_DATA
    public const string str_SOUND = "SoundOn";
    public const string str_MUSIC = "MusicOn";
    public static bool SoundOn { get; set; }
    public static bool MusicOn { get; set; }

    #endregion

    #region SOUND_CALLBACK_HANDLERS
    public delegate void DEL_SOUNDS_SETTINGS();
    public static event DEL_SOUNDS_SETTINGS EVENT_MusicSettingsChanged;
    public static event DEL_SOUNDS_SETTINGS EVENT_SoundSettingsChanged;
    private static void HANDLER_MusicSettingsChanged() { Debug.Log("HANDLER_MusicSettingsChanged Event Received"); }
    private static void HANDLER_SoundSettingsChanged() { Debug.Log("HANDLER_SoundSettingsChanged Event Received"); }


    #endregion

    #region SOUND_FUNCTION
    private static void InitializeSoundData() {
        EVENT_MusicSettingsChanged += HANDLER_MusicSettingsChanged;
        EVENT_SoundSettingsChanged += HANDLER_SoundSettingsChanged;
        FetchSoundSetting();
    }

    public static void SaveSoundSetting(string musicOrSound, bool value) {
        if (value) {
            PlayerPrefs.SetInt(musicOrSound, 1);
        } else {
            PlayerPrefs.SetInt(musicOrSound, 0);
        }

        if (musicOrSound == str_SOUND) {
            SoundOn = value;
            EVENT_SoundSettingsChanged();
        } else if (musicOrSound == str_MUSIC) {
            MusicOn = value;
            EVENT_MusicSettingsChanged();
        }

        Debug.Log(musicOrSound + " : " + value + " => Settings Saved");
    }

    public static void FetchSoundSetting() {
        if (PlayerPrefs.HasKey(str_SOUND)) {
            Debug.Log(str_SOUND + " Settings Fetched");
            SoundOn = PlayerPrefs.GetInt(str_SOUND) == 1;
            Debug.Log("soundOn  : " + SoundOn);
        } else {
            Debug.Log(str_SOUND + " Settings Reseted.");
            SoundOn = true;
            SaveSoundSetting(str_SOUND, SoundOn);
        }

        if (PlayerPrefs.HasKey(str_MUSIC)) {
            Debug.Log(str_MUSIC + " Settings Fetched");
            MusicOn = PlayerPrefs.GetInt(str_MUSIC) == 1;
            Debug.Log("musicOn  : " + MusicOn);
        } else {
            Debug.Log(str_MUSIC + " Settings Reseted.");
            MusicOn = true;
            //MusicOn = false;
            SaveSoundSetting(str_MUSIC, MusicOn);
        }
    }

    #endregion

    #endregion



    #region UI
    public static bool IsPointerOverUIObject() { //When Touching UI
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public static GameObject GetUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current) {
            position = new Vector2(Input.mousePosition.x, Input.mousePosition.y)
        };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        foreach (var item in results) {
            Debug.Log("GetUIObject result : " + item.gameObject.name);
        }
        if (results.Count > 0) {
            return results[0].gameObject;
        } else {
            return null;
        }
    }

    #endregion


    #region NATIVESHARE
    public static IEnumerator TakeSSAndShare(string strText) {
        yield return new WaitForEndOfFrame();

        Texture2D ss = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        ss.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        ss.Apply();

        string filePath = Path.Combine(Application.temporaryCachePath, "shared img.png");
        File.WriteAllBytes(filePath, ss.EncodeToPNG());
        Destroy(ss); // To avoid memory leaks
        new NativeShare().AddFile(filePath).SetSubject(GAME_NAME).SetText(strText).Share();
        // Share on WhatsApp only, if installed (Android only)
        //if( NativeShare.TargetExists( "com.whatsapp" ) )
        //	new NativeShare().AddFile( filePath ).SetText( "Hello world!" ).SetTarget( "com.whatsapp" ).Share();
    }

    #endregion




    #region IN-APP PURCHASE MANAGER
    /*
    public class IAPManager : IStoreListener {

        #region IAPManager_CORE_FUNCTIONALITY
        public static IAPManager Instance { set; get; }
        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

        public static void InitializeIAPManager() {
            Instance = new IAPManager();
            ResetPurchaseEvent();
            if (m_StoreController == null) {
                if (Instance.IsInitialized()) { // ... we are done here.
                    return;
                }
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = Instance.Initialize_IAPBuilder(ConfigurationBuilder.Instance(StandardPurchasingModule.Instance()));
            UnityPurchasing.Initialize(Instance, builder);

        }

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.

        private ConfigurationBuilder Initialize_IAPBuilder(ConfigurationBuilder builder) {
            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.

            //builder.AddProduct(Product_1000_Coins, ProductType.Consumable);
            //builder.AddProduct(Product_1500_Coins, ProductType.NonConsumable);
            builder.AddProduct(STR_PURCHASE_PRODUCT_NO_ADS, ProductType.NonConsumable);

            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // must only be referenced here. 

            //			builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            //				{ kProductNameAppleSubscription, AppleAppStore.Name },
            //				{ kProductNameGooglePlaySubscription, GooglePlay.Name },
            //			});

            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.

            return builder;
        }

        private static void ResetPurchaseEvent() {
            EVENT_PURCHASE_NOADS = null;
            EVENT_PURCHASE_NOADS += HANDLER_PurchaseComplete;

        }



        private bool IsInitialized() {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("GameManager.IAPManager : OnInitialized: PASS");
            m_StoreController = controller; // Overall Purchasing system, configured with products for this application.
            m_StoreExtensionProvider = extensions; // Store specific subsystem, for accessing device-specific store features.

#if UNITY_ANDROID
            // Manual Checking and restoring
            Debug.LogError("GameManager.IAPManager : Manual Check if already purchased !!!");
            if (m_StoreController.products.WithID(STR_PURCHASE_PRODUCT_NO_ADS).hasReceipt) {
                // IAP success results. 
                Debug.LogError("GameManager.IAPManager : Has Receipt and restoring !!!");
                PlayerPrefs.SetInt("NoAds", 1);
                PlayerPrefs.Save();
                RemoveBanner();
            }
#endif

        }

        public void OnInitializeFailed(InitializationFailureReason error) {
            Debug.Log("GameManager.IAPManager : OnInitializeFailed InitializationFailureReason:" + error); // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        }

        private void BuyProductID(string productId) {
            // If Purchasing has been initialized ...
            if (IsInitialized()) {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase) {
                    Debug.Log(string.Format("GameManager.IAPManager : Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else {
                    // ... report the product look-up failure situation  
                    Debug.Log("GameManager.IAPManager : BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("GameManager.IAPManager : BuyProductID FAIL. Not initialized.");
            }
        }



        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases() {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized()) {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer) {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        #endregion

        #region IAPManager_CALLBACK_HANDLERS
        public delegate void DEL_PurchaseComplete();
        public static event DEL_PurchaseComplete EVENT_PURCHASE_NOADS;
        private static void HANDLER_PurchaseComplete() { Debug.Log("HANDLER_PurchaseComplete Event Received"); }
        #endregion

        #region IAPManager_PRODUCTS
        public static string STR_PURCHASE_PRODUCT_NO_ADS = "com.if.rageball.no_ads";

        private static string STR_PRODUCT_NAME_APPLE_SUBSCRIPTION = "com.unity3d.subscription.new";    // Apple App Store-specific product identifier for the subscription product.
        private static string STR_PRODUCT_NAME_GOOGLE_PLAY_SUBSCRIPTION = "com.unity3d.subscription.original";    // Google Play Store-specific product identifier subscription product.

        public void BuyNoAds() {
            BuyProductID(STR_PURCHASE_PRODUCT_NO_ADS);
        }

        public void BuyNonConsumable() {
            // Buy the non-consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            //BuyProductID(kProductIDNonConsumable);
        }

        public void BuySubscription() {
            // Buy the subscription product using its the general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            // Notice how we use the general product identifier in spite of this ID being mapped to
            // custom store-specific identifiers above.
            //BuyProductID(kProductIDSubscription);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
            // A consumable product has been purchased by this user.
            Debug.LogError("GameManager.IAPManager : PurchaseProcessingResult Called");

            if (String.Equals(args.purchasedProduct.definition.id, STR_PURCHASE_PRODUCT_NO_ADS, StringComparison.Ordinal)) {
                Debug.Log("GameManager.IAPManager : Proccessing Remove all Ads Purchase");
                PlayerPrefs.SetInt("NoAds", 1);
                PlayerPrefs.Save();
                RemoveBanner();
                EVENT_PURCHASE_NOADS();

            } else {
                Debug.Log(string.Format("GameManager.IAPManager : ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed.
            ResetPurchaseEvent();
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("GameManager.IAPManager : OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
            if (String.Equals(product.definition.storeSpecificId, STR_PURCHASE_PRODUCT_NO_ADS)) {
                if (String.Equals(failureReason, "DuplicateTransaction")) {
                    Debug.LogError("GameManager.IAPManager : Duplicate Transaction Fix...");
                    PlayerPrefs.SetInt("NoAds", 1);
                    PlayerPrefs.Save();
                    RemoveBanner();
                    EVENT_PURCHASE_NOADS();

                }
            }

            ResetPurchaseEvent();
        }

        #endregion

    }
*/
    #endregion



} // End OF Class GameManager