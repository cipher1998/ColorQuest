using UnityEngine;
using InvisibleFiction;

public class LevelManager : MonoBehaviour {

    [SerializeField] private GameObject _prefabPlatform;
    public static GameObject Platform { get { return Instance._prefabPlatform; } }

    [SerializeField] private GameObject _prefabGemPiles;
    [SerializeField] private GameObject _gemPiles;
    public static GameObject GemPiles { get { return Instance._gemPiles; } set { Instance._gemPiles = value; } }
    public static Transform GemStacks {
        get { return Instance._gemPiles.transform.GetChild(1).transform; }
    }

    private int _currentLevel, _scoreLevel, _gemsTotal, _gemsLevelReward;

    private bool _adReviveClaimed, _ad2xClaimed;

    private bool _intialized = false;

    public static bool AdReviveClaimed { get { return Instance._adReviveClaimed; } set { Instance._adReviveClaimed = value; } }
    public static bool Ad2xClaimed { get { return Instance._ad2xClaimed; } set { Instance._ad2xClaimed = value; } }
    public static int Current_Level { get { return Instance._currentLevel; } set { Instance._currentLevel = value; } }
    public static int Score { get { return Instance._scoreLevel; } set { Instance._scoreLevel = value; } }
    public static int TotalGems { get { return Instance._gemsTotal; } set { Instance._gemsTotal = value; } }
    public static int LevelRewardGems { get { return Instance._gemsLevelReward; } set { Instance._gemsLevelReward = value; } }

    private static LevelManager Instance;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        Score = 0;
    }

    public static void AddScore(int _scoreToAdd = 1) {
        Score += _scoreToAdd;
        Debug.Log("New Score : " + Score);
        UIManager.Instance.Refresh_Levelscore();
    }

    public static void SaveLevelData() {
        PlayerPrefs.SetInt(GameManager.str_TOTAL_GEMS_KEY, TotalGems);

        string strLevelKey = $"{GameManager.str_LEVEL_KEY}_{Current_Level}";
        string strValue = $"{Current_Level}, {Score}"; // LEVEL_XX => XX, score

        Debug.Log("Storing New Level Data into PlayerPrefs");
        PlayerPrefs.SetString(strLevelKey, strValue); // LEVEL_XX => XX, score

        //if (Current_Level >= PlayerPrefs.GetInt(GameManager.str_LAST_LEVEL_KEY)) { // Current_Level > LEVEL_LAST_UNLOCKED
        Current_Level = PlayerPrefs.GetInt(GameManager.str_LAST_LEVEL_KEY);
        Current_Level++;
        PlayerPrefs.SetInt(GameManager.str_LAST_LEVEL_KEY, Current_Level);
        Debug.Log("Next Unlocked Level : " + Current_Level);
        //}

    }

    public static void FetchLevelData() {
        Score = 0;
        LevelRewardGems = Random.Range(3, 7);
        AdReward_Revived(true);
        AdReward_2x(true);

        if (PlayerPrefs.HasKey(GameManager.str_LAST_LEVEL_KEY)) {
            Current_Level = PlayerPrefs.GetInt(GameManager.str_LAST_LEVEL_KEY); // LEVEL_LAST_UNLOCKED => XX
            Debug.Log($"Current Last Unlocked Level : {Current_Level}");
        } else {
            Debug.Log("NO LEVEL DATA FOUND & RESETING DATA, Unlocking Level: 1");
            Current_Level = 1;
            PlayerPrefs.SetInt(GameManager.str_LAST_LEVEL_KEY, Current_Level); // LEVEL_LAST_UNLOCKED => 1

            TotalGems = 0;
            PlayerPrefs.SetInt(GameManager.str_TOTAL_GEMS_KEY, TotalGems); // TOTAL_GEMS => 0

            //ScrollPanel.ResetCharacterData();

        }

        Debug.Log("Current Selected Level : " + Current_Level);


        if (PlayerPrefs.HasKey(GameManager.str_TOTAL_GEMS_KEY)) {
            TotalGems = PlayerPrefs.GetInt(GameManager.str_TOTAL_GEMS_KEY); // TOTAL_GEMS => XX
        } else {
            TotalGems = 0;
            PlayerPrefs.SetInt(GameManager.str_TOTAL_GEMS_KEY, TotalGems); // TOTAL_GEMS => 0
        }

        if (!Instance._intialized) {
            if (!PlayerPrefs.HasKey(GameManager.str_CURRENT_CHARACTER_KEY)) {
                Debug.LogError("NO CHARACTER DATA FOUND & RESETING DATA, Unlocking First Character");
                ScrollPanel.ResetCharacterData();
            }

            Instance._intialized = true;
            /* 
            if (PlayerPrefs.HasKey(GameManager.str_CURRENT_CHARACTER_KEY)) {
                string tempCharName = PlayerPrefs.GetString(GameManager.str_CURRENT_CHARACTER_KEY);
                ScrollPanel.FetchCurrentCharacterData(tempCharName);
            } else {
                ScrollPanel.ResetCharacterData();
            } */

        }

        string tempCharName = PlayerPrefs.GetString(GameManager.str_CURRENT_CHARACTER_KEY);
        ScrollPanel.FetchCurrentCharacterData(tempCharName);

        UIManager.Instance.Refresh_LevelIndex();
        UIManager.Instance.Refresh_Levelscore();
        UIManager.Instance.Refresh_TotalGems();

    }

    public static void GenerateGemPiles() {
        if (GemPiles == null) {
            Debug.Log("Level GemPiles Generated");
            GemPiles = Instantiate(Instance._prefabGemPiles, Obstacle.CurrentObstacleTransform.position, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.up));
            GemPiles.transform.SetParent(Instance.transform.parent);
        } else {
            Debug.Log("Level GemPiles Already Exists");
        }

    }

    public static void DestroyGemPiles() {
        if (GemPiles != null) {
            Debug.Log("Level GemPiles Destroyed");
            Destroy(GemPiles.gameObject);
            GemPiles = null;
        }
    }

    public static void AddGems(int _gemsToAdd = 1, bool _destroyGemPile = false) {
        TotalGems += _gemsToAdd;
        PlayerPrefs.SetInt(GameManager.str_TOTAL_GEMS_KEY, TotalGems);

        if (_gemsToAdd > 0) {
            Debug.Log("Level GemPiles Collected");
            SoundManager.PlayAudio(SoundManager.Get(Sounds.pickUpGemPiles));
            if (_destroyGemPile) {
                Destroy(GemPiles);
            }

            Debug.Log($"New Gems : {TotalGems}");

        } else {
            Debug.Log($"Remaining Gems : {TotalGems}");
        }

        UIManager.Instance.Refresh_TotalGems();
    }

    public static void AdReward_Revived(bool _resetRewardClaimed = false) {
        if (_resetRewardClaimed) {
            Debug.Log("Ad-Reward Revive Claimed Reseted");
            AdReviveClaimed = false;
        } else {
            Debug.Log("Ad-Reward Revive Claimed");
            AdReviveClaimed = true;
            GameManager.UpdateGameState(LevelState.PLAYING);
        }

    }

    public static void AdReward_2x(bool _resetRewardClaimed = false) {
        if (_resetRewardClaimed) {
            Debug.Log("Ad-Reward 2x Claimed Reseted");
            Ad2xClaimed = false;
        } else {
            //AddGems(LevelRewardGems);
            Instance.StartCoroutine(UIManager.Instance.Gem2x.GetComponent<MoveObjects>().GemCollected(UIManager.Instance.GemTotal));
            Debug.Log("Ad-Reward 2x Claimed");
            Ad2xClaimed = true;
            UIManager.Instance.LevelClearPanel_Refresh();
        }


    }


}