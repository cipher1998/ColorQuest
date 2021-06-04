using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using InvisibleFiction;


public class UIManager : MonoBehaviour {

    [Header("Light")]

    [SerializeField] private Light lightCharacter;


    [Header("Buttons")]

    [SerializeField] private Button btnShoot;
    [SerializeField] private Button btnPause;
    [SerializeField] private Button btnRevive;
    [SerializeField] private Button btnNoThanks;
    [SerializeField] private Button btn2x;
    [SerializeField] private Button btnNextLevel;

    [SerializeField] private GameObject _btnGroup_LevelFailed;

    [SerializeField] private Button btnUnlockCharacter;
    [SerializeField] private Button btnSelectCharacter;
    [SerializeField] private Button btnCurrentSelectedCharacter;



    [Header("TextMeshPro Text")]
    [SerializeField] private TextMeshProUGUI[] TMP_level_Index;
    [SerializeField] private TextMeshProUGUI[] TMP_totalGems;
    [SerializeField] private TextMeshProUGUI TMP_levelScore;
    [SerializeField] private TextMeshProUGUI TMP_labelCharacterName;
    [SerializeField] private TextMeshProUGUI TMP_labelCharacterPrice;


    [Header("UI Layers & Panels")]
    //[SerializeField] private GameObject _ui_splashScreenUI;
    [SerializeField] private GameObject _ui_homeScreenUI;
    [SerializeField] private GameObject _ui_inGameUI;
    [SerializeField] private GameObject _ui_ShopCharacter;
    //[SerializeField] private GameObject _ui_overlayMessageUI;

    //[SerializeField] private GameObject _panel_displayInformation;

    [SerializeField] private GameObject _panel_gameQuit;
    [SerializeField] private GameObject _panel_levelPaused;
    [SerializeField] private GameObject _panel_levelCleared;
    [SerializeField] private GameObject _panel_levelFailed;

    [SerializeField] private GameObject _layer_HomeScreen;
    [SerializeField] private GameObject _layer_HomeScreenContent;
    [SerializeField] private GameObject _layer_overlayOpacity_HomeScreen;
    [SerializeField] private GameObject _layer_GamePlay;
    [SerializeField] private GameObject _layer_overlayOpacity_inGame;



    public Transform GamePlayLayer { get => _layer_GamePlay.transform; }



    [Header("Manager References")]
    [SerializeField] private PaintBallManager paintBallManager;
    [SerializeField] private ObstacleManager obstacleManager;
    [SerializeField] private RotationManager rotationManager;


    [Header("Material References")]
    [SerializeField] private Material _matSprite_Zoom;
    [SerializeField] private Material _matSprite_Default;
    public Material MaterialSprite_Zoom { get { return _matSprite_Zoom; } }
    public Material MaterialSprite_Default { get { return _matSprite_Default; } }


    [Header("Extra References")]
    [SerializeField] private GameObject _totalGemImage;
    [SerializeField] private GameObject _gem2x;

    public GameObject Gem2x { get { return _gem2x; } }
    public GameObject GemTotal { get { return _totalGemImage; } }



    [Header("Private Variables")]
    private Character currDisplayCharacter;
    private IEnumerator coroutineUI;
    public static UIManager Instance;

    [System.Obsolete]
    private void Awake() {
        Instance = this;

        GameManager.EVENT_LEVELSTARTED += HANDLER_LEVELSTARTED;
        GameManager.EVENT_LEVELPAUSED += HANDLER_LEVELPAUSED;
        GameManager.EVENT_LEVELRESUMED += HANDLER_LEVELRESUMED;
        GameManager.EVENT_LEVELFAILED += HANDLER_LEVELFAILED;
        GameManager.EVENT_LEVELCLEARED += HANDLER_LEVELCLEARED;
        GameManager.InitializeGameManager();

    }

    private void Start() {
        //PlatformManager.Reset();
        LevelManager.FetchLevelData();

        _panel_levelPaused.SetActive(false);
        _panel_levelFailed.SetActive(false);
        _panel_levelCleared.SetActive(false);

        _layer_HomeScreen.SetActive(true);
        _layer_GamePlay.SetActive(false);

        _ui_homeScreenUI.SetActive(true);
        _ui_ShopCharacter.SetActive(false);

    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (_layer_HomeScreen.activeInHierarchy) {
                _panel_gameQuit.SetActive(true);
                _layer_overlayOpacity_HomeScreen.SetActive(true);
                _layer_HomeScreenContent.SetActive(false);
            }
        }
    }


    private void OnDestroy() {
        GameManager.EVENT_LEVELSTARTED -= HANDLER_LEVELSTARTED;
        GameManager.EVENT_LEVELPAUSED -= HANDLER_LEVELPAUSED;
        GameManager.EVENT_LEVELRESUMED -= HANDLER_LEVELRESUMED;
        GameManager.EVENT_LEVELFAILED -= HANDLER_LEVELFAILED;
        GameManager.EVENT_LEVELCLEARED -= HANDLER_LEVELCLEARED;
    }


    private void OnApplicationPause(bool pauseStatus) {
        if (pauseStatus) {
            if (btnPause.gameObject.activeInHierarchy && btnPause.interactable) {
                Debug.Log("Game Paused by platform and switched to another application");
                GameManager.UpdateGameState(LevelState.PAUSED);
            }
        }
    }

    public void HANDLER_LEVELPAUSED() {
        RotationManager.StartRotating(false);
        PaintBallManager.PaintBall_LoadUnload(false);

        btnPause.interactable = false;
        btnPause.ResetMaterial();
        _panel_levelPaused.SetActive(true);
        _layer_overlayOpacity_inGame.SetActive(true);
    }

    public void HANDLER_LEVELRESUMED() {
        rotationManager.enabled = true;
        RotationManager.StartRotating(true);

        obstacleManager.enabled = true;

        PaintBallManager.PaintBall_LoadUnload(true);

        btnPause.interactable = true;
        btnPause.ResetMaterial(false);
        btnPause.gameObject.SetActive(true);
        btnShoot.gameObject.SetActive(true);

        //_panel_levelPaused.SetActive(false);
        //_panel_levelFailed.SetActive(false);
        //_panel_levelCleared.SetActive(false);
        _panel_levelPaused.GetComponent<UITwean>().DisableTwean();
        _panel_levelFailed.GetComponent<UITwean>().DisableTwean();
        _panel_levelCleared.GetComponent<UITwean>().DisableTwean();

        _layer_overlayOpacity_inGame.SetActive(false);

        PlayerManager.Reset();

    }

    public void HANDLER_LEVELFAILED() {

        RotationManager.StartRotating(false);
        rotationManager.enabled = false;
        PaintBallManager.PaintBall_LoadUnload(false);
        obstacleManager.enabled = false;
        PlayerManager.Die();
        //_panel_levelFailed.SetActive(true);

        btnPause.gameObject.SetActive(false);
        btnShoot.gameObject.SetActive(false);
        SoundManager.PlayAudio(SoundManager.Get(Sounds.levelFailed), true);

    }

    public void LevelClearPanel_Refresh() {
        /*
            if (LevelManager.Ad2xClaimed) {
            btn2x.interactable = false;
            } else {
            btn2x.interactable = true;
            }
        */

        btn2x.interactable = !LevelManager.Ad2xClaimed;
        btn2x.ResetMaterial(LevelManager.Ad2xClaimed);
        btnNextLevel.ResetMaterial(!LevelManager.Ad2xClaimed);

    }

    public void LevelClearPanel_Open() {
        if (coroutineUI != null) {
            StopCoroutine(coroutineUI);
        }
        coroutineUI = DelayOpenPanel(_panel_levelCleared, 5f);
        StartCoroutine(coroutineUI);

        LevelClearPanel_Refresh();

    }

    public void LevelFailedPanel_Open() {
        if (coroutineUI != null) {
            StopCoroutine(coroutineUI);
        }
        coroutineUI = DelayOpenPanel(_panel_levelFailed, 1f);
        StartCoroutine(coroutineUI);

        if (LevelManager.AdReviveClaimed) {
            btnRevive.interactable = false;
            btnRevive.ResetMaterial();
            btnNoThanks.gameObject.SetActive(false);
            _btnGroup_LevelFailed.SetActive(true);
            btnRevive.GetComponent<AdButtonTimer>().StarTimer(false);
        } else {
            btnRevive.interactable = true;
            btnRevive.ResetMaterial(false);
            btnNoThanks.gameObject.SetActive(true);
            _btnGroup_LevelFailed.SetActive(false);
            btnRevive.GetComponent<AdButtonTimer>().StarTimer(true);
        }

    }

    public void HomeScreen_Open() {
        if (coroutineUI != null) {
            StopCoroutine(coroutineUI);
        }
        coroutineUI = DelayHomeScreenlayer(1f);
        StartCoroutine(coroutineUI);

    }

    public IEnumerator DelayHomeScreenlayer(float _time = 0.05f) {
        yield return new WaitForSeconds(_time);

        _layer_GamePlay.SetActive(false);
        _layer_HomeScreen.SetActive(true);

        _ui_inGameUI.SetActive(false);
        _ui_ShopCharacter.SetActive(false);
        _ui_homeScreenUI.SetActive(true);
    }

    public IEnumerator DelayOpenPanel(GameObject _panel, float _time = 1f) {
        yield return new WaitForSeconds(_time);
        Platform.Current.WinParticle(false);
        _panel.SetActive(true);
        _layer_overlayOpacity_inGame.SetActive(true);
    }


    public void HANDLER_LEVELCLEARED() {
        SoundManager.PlayAudio(SoundManager.Get(Sounds.levelFinished), true);

        PlayerManager.Run(Platform.Current.Base.GetChild(0).transform, true);
        Platform.Current.WinParticle(true);

        RotationManager.StartRotating(false);
        rotationManager.enabled = false;
        PaintBallManager.PaintBall_LoadUnload(false);
        obstacleManager.enabled = false;
        //_panel_levelCleared.SetActive(true);

        btnPause.gameObject.SetActive(false);
        btnShoot.gameObject.SetActive(false);
        LevelManager.SaveLevelData();
    }

    public void HANDLER_LEVELSTARTED() {
        _layer_HomeScreen.SetActive(false);
        _layer_GamePlay.SetActive(true);

        if (obstacleManager) {
            obstacleManager.enabled = true;
        }

        if (rotationManager) {
            rotationManager.enabled = true;
            //rotationManager.StartRotating(true);
        }

        if (paintBallManager) {
            paintBallManager.enabled = true;
        }

        LevelManager.FetchLevelData();
        ObstacleManager.Created();
        PlayerManager.Create();

        btnShoot.interactable = true;
        btnShoot.gameObject.SetActive(true);

        btnPause.interactable = true;
        btnPause.ResetMaterial(false);
        btnPause.gameObject.SetActive(true);

        _layer_overlayOpacity_inGame.SetActive(false);

        _ui_homeScreenUI.SetActive(false);
        _ui_inGameUI.SetActive(true);

        //_panel_levelPaused.SetActive(false);
        //_panel_levelCleared.SetActive(false);
        //_panel_levelFailed.SetActive(false);

        _panel_levelPaused.GetComponent<UITwean>().DisableTwean();
        _panel_levelCleared.GetComponent<UITwean>().DisableTwean();
        _panel_levelFailed.GetComponent<UITwean>().DisableTwean();




    }

    public void OnClick_Play() {
        SoundManager.PlayAudio_ButtonClicked();
        GameManager.UpdateGameState(LevelState.PLAYING);
    }

    public void OnClick_Pause() {
        SoundManager.PlayAudio_ButtonClicked();
        GameManager.UpdateGameState(LevelState.PAUSED);
    }

    public void OnClick_Continue() {
        SoundManager.PlayAudio_ButtonClicked();
        GameManager.UpdateGameState(LevelState.PLAYING);
    }

    public void OnClick_AdRevive() {
        SoundManager.PlayAudio_ButtonClicked();
        //LevelManager.AdReward_Revived();
        //GameManager.UpdateGameState(LevelState.PLAYING);

        if (GameManager.CheckNetworkConnection()) {
            Debug.Log("Requesting GoogleMobileAd for Watching Rewarded Ad...");
            GameManager.ShowRewardedAd(AdManager.AdReward_Revived);
        } else {

        }
    }

    public void OnClick_Ad2x() {
        SoundManager.PlayAudio_ButtonClicked();
        //LevelManager.AdReward_2x();
        //LevelClearPanel_Refresh();

        if (GameManager.CheckNetworkConnection()) {
            Debug.Log("Requesting GoogleMobileAd for Watching Rewarded Ad...");
            GameManager.ShowRewardedAd(AdManager.AdReward_2x);
        } else {

        }
    }

    public void OnClick_NoThanks() {
        Debug.Log("No Thanks Button Clicked");
        SoundManager.PlayAudio_ButtonClicked();
        AdManager.ShowInterstitialAd();

        btnNoThanks.gameObject.SetActive(false);
        _btnGroup_LevelFailed.SetActive(true);
        btnRevive.GetComponent<AdButtonTimer>().StarTimer(false);
        btnRevive.interactable = false;
        btnRevive.ResetMaterial();

    }

    public void OnClick_UnlockCharacter() {
        SoundManager.PlayAudio_ButtonClicked();
        if (currDisplayCharacter != null && !currDisplayCharacter.IsUnlocked) {
            currDisplayCharacter.Unlock();
            PlayerManager.Unlocked();

        }
    }

    public void OnClick_SelectCharacter() {
        SoundManager.PlayAudio_ButtonClicked();
        PlayerManager.Changed(currDisplayCharacter.Selected());
    }

    public void OnClick_Restart() {
        SoundManager.PlayAudio_ButtonClicked();
        AdManager.ShowInterstitialAd();

        GameManager.UpdateGameState(LevelState.LOADING);
    }

    public void OnClick_NextLevel() {
        SoundManager.PlayAudio_ButtonClicked();
        AdManager.ShowInterstitialAd();

        //_panel_levelCleared.SetActive(false);
        _panel_levelCleared.GetComponent<UITwean>().DisableTwean();
        _layer_overlayOpacity_inGame.SetActive(false);
        Platform.Current.ChangePlatform();

    }



    public void OnClick_ShopCharacter() {
        SoundManager.PlayAudio_ButtonClicked();
        AdManager.ShowInterstitialAd();

        _layer_overlayOpacity_HomeScreen.SetActive(true);

        if (_layer_HomeScreen.activeInHierarchy) {
            //_layer_HomeScreen.SetActive(false);

            _ui_inGameUI.SetActive(false);
            _ui_homeScreenUI.SetActive(false);
            _ui_ShopCharacter.SetActive(true);

        } else if (_layer_GamePlay.activeInHierarchy) {
            _layer_GamePlay.SetActive(false);
            _layer_HomeScreen.SetActive(true);

            _ui_inGameUI.SetActive(false);
            _ui_homeScreenUI.SetActive(false);
            _ui_ShopCharacter.SetActive(true);
        }

    }

    public void OnClick_BackToHomeScreen() {
        SoundManager.PlayAudio_ButtonClicked();
        AdManager.ShowInterstitialAd();

        if (_layer_HomeScreen.activeInHierarchy) {
            _layer_GamePlay.SetActive(false);
            _layer_overlayOpacity_HomeScreen.SetActive(false);
            _ui_homeScreenUI.SetActive(true);
            _ui_ShopCharacter.SetActive(false);

        } else if (_layer_GamePlay.activeInHierarchy) {
            _panel_levelPaused.GetComponent<UITwean>().DisableTwean();
            _panel_levelFailed.GetComponent<UITwean>().DisableTwean();
            _panel_levelCleared.GetComponent<UITwean>().DisableTwean();

            HomeScreen_Open();

            /*
                _layer_GamePlay.SetActive(false);
                _layer_HomeScreen.SetActive(true);

                _ui_inGameUI.SetActive(false);
                _ui_ShopCharacter.SetActive(false);
                _ui_homeScreenUI.SetActive(true);
            */
        }

        PlatformManager.Reset();
        PlayerManager.DestroyCharacterIfOnPlatform();
        LevelManager.FetchLevelData();
        Refresh_LevelIndex();

        GameManager.UpdateGameState(LevelState.IDLE);
    }

    public void OnClick_PrivacyPolicy() {
        SoundManager.PlayAudio_ButtonClicked();
        Debug.Log("Opening Privacy Policy URL");
        Application.OpenURL(GameManager.GAME_PRIVACY_POLICY_LINK);

    }

    public void OnClick_SoundSetting(bool value) {
        SoundManager.PlayAudio_ButtonClicked();
        SoundManager.OnClick_MusicButton(value);
        SoundManager.OnClick_SoundButton(value);
    }

    public void OnClick_ShareGame() {
        Debug.Log("Share Button Clicked");
        SoundManager.PlayAudio_ButtonClicked();
        string strText = "Hey, I'm Playing this Awsome Game " + GameManager.GAME_NAME;
        strText += GameManager.GAME_DOWNLOAD_LINK;
        StartCoroutine(GameManager.TakeSSAndShare(strText));
    }

    public void OnClick_QuitGame_Yes() {
        Debug.Log("Yes-QuitGame Button Clicked");
        SoundManager.PlayAudio_ButtonClicked();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

    }

    public void OnClick_QuitGame_No() {
        Debug.Log("No-QuitGame Button Clicked");
        SoundManager.PlayAudio_ButtonClicked();
        _layer_HomeScreenContent.SetActive(true);
        _layer_overlayOpacity_HomeScreen.SetActive(false);
        _panel_gameQuit.GetComponent<UITwean>().DisableTwean();

    }

    public void Refresh_ButtonMaterial(Button _btn, bool _setDefaultMaterial = true) {
        if (_setDefaultMaterial) {
            _btn.GetComponent<Image>().material = _matSprite_Default;
        } else {
            _btn.GetComponent<Image>().material = _matSprite_Zoom;
        }


    }


    public void Refresh_LevelIndex() {
        Debug.Log($"Refreshing Level Index : {LevelManager.Current_Level}");

        TMP_level_Index[0].text = $"Level : {LevelManager.Current_Level}";
        TMP_level_Index[1].text = $"Level : {LevelManager.Current_Level}";
        TMP_level_Index[2].text = $"LEVEL {LevelManager.Current_Level} \nCLEARED";
        TMP_level_Index[3].text = $"LEVEL {LevelManager.Current_Level} \nFAILED";
    }

    public void Refresh_Levelscore() {
        TMP_levelScore.text = $"Score : {LevelManager.Score}";
    }

    public void Refresh_TotalGems() {
        TMP_totalGems[0].text = $"TOTAL : <sprite name=gem> {LevelManager.TotalGems}"; // Shop Panel
        TMP_totalGems[1].text = $"{LevelManager.TotalGems} <sprite name=gem>"; // In-Game UI
    }

    public void Refresh_CharacterName(string _name) {
        TMP_labelCharacterName.text = _name;
    }

    public void Refresh_Character(Character _character, bool refresh = false) {
        if (currDisplayCharacter != _character || refresh) {
            if (!refresh || currDisplayCharacter == null) {
                Debug.Log($"Current Character on Shop Panel : {_character.Name}");
                currDisplayCharacter = _character;
                Refresh_TotalGems();
            }

            TMP_labelCharacterName.text = currDisplayCharacter.Name;
            if (currDisplayCharacter.IsUnlocked) {
                //TMP_labelCharacterPrice.text = "UNLOCKED";
                lightCharacter.enabled = true;
                btnUnlockCharacter.gameObject.SetActive(false);
                btnSelectCharacter.gameObject.SetActive(true);
                btnSelectCharacter.ResetMaterial(false);

                if (PlayerManager.CurrentCharacter == currDisplayCharacter) {
                    btnCurrentSelectedCharacter.gameObject.SetActive(true);
                    TMP_labelCharacterPrice.text = "CURRENTLY SELECTED";
                } else {
                    btnCurrentSelectedCharacter.gameObject.SetActive(false);
                    TMP_labelCharacterPrice.text = "UNLOCKED";
                }


            } else {
                TMP_labelCharacterPrice.text = $"UNLOCK : <sprite name=gem> {currDisplayCharacter.UnlockPrice}";
                lightCharacter.enabled = false;

                if (LevelManager.TotalGems < currDisplayCharacter.UnlockPrice) {
                    btnUnlockCharacter.interactable = false;
                } else {
                    btnUnlockCharacter.interactable = true;
                }
                btnUnlockCharacter.ResetMaterial(!btnUnlockCharacter.interactable);

                btnUnlockCharacter.gameObject.SetActive(true);
                btnSelectCharacter.gameObject.SetActive(false);
                btnSelectCharacter.ResetMaterial();
                btnCurrentSelectedCharacter.gameObject.SetActive(false);

            }

        }

    }


}
