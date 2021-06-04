using UnityEngine;
using UnityEngine.UI;
using InvisibleFiction;

public class SoundManager : MonoBehaviour {
    [Header("Background Music Audio Clip")]
    [SerializeField] private AudioClip _audioClip_bgMusic;

    [Header("Level Audio Clips")]
    [Tooltip("in following order: levelFailed, levelCleared")]
    [SerializeField] private AudioClip[] _audioClip_levelClip = new AudioClip[2];

    [Header("Extra Audio Clips")]
    [SerializeField] private AudioClip _audioClip_btnClick;
    [SerializeField] private AudioClip _audioClip_obstacleHit;
    [SerializeField] private AudioClip _audioClip_obstacleHitWrong;
    [SerializeField] private AudioClip _audioClip_pickUpGemPiles;
    [SerializeField] private AudioClip _audioClip_obstacleShattered;


    [Header("Buttons")]
    [SerializeField] private Button btnMusicOn;
    [SerializeField] private Button btnSoundOn;


    [Header("EXtra Variables")]
    private AudioSource _audioSourceSFX;
    private AudioSource _audioSourceBGMusic;
    private AudioSource _audioSourceLevelFinish;

    private static SoundManager Instance;



    private void Awake() {
        if (!Instance) {
            Instance = this;
            DontDestroyOnLoad(this);
        } else {
            Debug.Log("Reseting SoundManager References");

            Instance.btnMusicOn = this.btnMusicOn;
            Instance.btnSoundOn = this.btnSoundOn;

            GameManager.FetchSoundSetting();
            RefreshSettings();
            Destroy(gameObject);
        }

        GameManager.EVENT_MusicSettingsChanged += HANDLER_MusicSettingsChanged;
        GameManager.EVENT_SoundSettingsChanged += HANDLER_SoundSettingsChanged;

        _audioSourceBGMusic = this.GetComponents<AudioSource>()[0];
        _audioSourceBGMusic.loop = true;
        _audioSourceBGMusic.clip = _audioClip_bgMusic;

        _audioSourceSFX = this.GetComponents<AudioSource>()[1];
        _audioSourceSFX.clip = _audioClip_btnClick;

        _audioSourceLevelFinish = this.GetComponents<AudioSource>()[2];
        _audioSourceLevelFinish.clip = _audioClip_levelClip[1];

    }

    private void Start() {
        GameManager.FetchSoundSetting();
        RefreshSettings();
    }

    private void OnDestroy() {
        GameManager.EVENT_MusicSettingsChanged -= HANDLER_MusicSettingsChanged;
        GameManager.EVENT_SoundSettingsChanged -= HANDLER_SoundSettingsChanged;

    }

    private void StartStopBGMusic() {
        if (GameManager.MusicOn && !Instance._audioSourceBGMusic.isPlaying) {
            Instance._audioSourceBGMusic.Play();
        } else if (!GameManager.MusicOn && Instance._audioSourceBGMusic.isPlaying) {
            Instance._audioSourceBGMusic.Stop();
        }

    }

    public static void PlayAudio_ButtonClicked() {
        if (GameManager.SoundOn) {
            Debug.Log("Playing Audio : " + Instance._audioClip_btnClick.name);
            Instance._audioSourceSFX.clip = Instance._audioClip_btnClick; // Remove on Final Build
            Instance._audioSourceSFX.Play();
        }
    }

    public static void PlayAudio(AudioClip _audioClip, bool isLevelFinishClip = false) {
        if (GameManager.SoundOn) {
            Debug.Log("Playing Audio : " + _audioClip.name);
            if (isLevelFinishClip) {
                Instance._audioSourceLevelFinish.clip = _audioClip;
                Instance._audioSourceLevelFinish.Play();
            } else {
                Instance._audioSourceSFX.clip = _audioClip;
                Instance._audioSourceSFX.Play();
            }
        }
    }

    public static AudioClip Get(Sounds soundClip) {
        AudioClip newClip = null;
        switch (soundClip) {
            case Sounds.bgMusic:
                newClip = Instance._audioClip_bgMusic;
                break;
            case Sounds.levelFailed:
                newClip = Instance._audioClip_levelClip[0];
                break;
            case Sounds.levelCleared:
            case Sounds.levelFinished:
                newClip = Instance._audioClip_levelClip[1];
                break;
            case Sounds.btnClicked:
                newClip = Instance._audioClip_btnClick;
                break;
            case Sounds.obstacleHit:
                newClip = Instance._audioClip_obstacleHit;
                break;
            case Sounds.obstacleHitWrong:
                newClip = Instance._audioClip_obstacleHitWrong;
                break;
            case Sounds.obstacleShattered:
                newClip = Instance._audioClip_obstacleShattered;
                break;
            case Sounds.pickUpGemPiles:
                newClip = Instance._audioClip_pickUpGemPiles;
                break;

            default:
                Debug.Log("No Audio Clip Found !!!");
                break;
        }

        Debug.Log("Playing Reqested Audio Clip : " + newClip.name);
        return newClip;
    }

    public static void OnClick_MusicButton(bool value) {
        GameManager.SaveSoundSetting(GameManager.str_MUSIC, value);
    }

    public static void OnClick_SoundButton(bool value) {
        GameManager.SaveSoundSetting(GameManager.str_SOUND, value);
    }

    private void HANDLER_MusicSettingsChanged() {
        if (Instance.btnMusicOn) {
            Instance.btnMusicOn.gameObject.SetActive(GameManager.MusicOn);
        }

        StartStopBGMusic();
    }

    private void HANDLER_SoundSettingsChanged() {
        if (Instance.btnMusicOn) {
            Instance.btnSoundOn.gameObject.SetActive(GameManager.SoundOn);
        }

    }

    public void RefreshSettings() {
        HANDLER_MusicSettingsChanged();
        HANDLER_SoundSettingsChanged();
    }

}
