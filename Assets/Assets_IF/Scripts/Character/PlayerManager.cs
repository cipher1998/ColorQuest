using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InvisibleFiction;


public class PlayerManager : MonoBehaviour {

    [Header("Player-Character")]
    [SerializeField] private Transform _characterParent;
    [SerializeField] private List<GameObject> _list_prefabCharacters;


    private static GameObject _playerCharacter;
    public static Character CurrentCharacter;


    public delegate void DEL_Character();
    public static event DEL_Character EVENT_Character_Changed;
    public static event DEL_Character EVENT_Character_Unlocked;
    public static event DEL_Character EVENT_Character_Created;
    public static event DEL_Character EVENT_Character_Attack;
    public static event DEL_Character EVENT_Character_Run;
    public static event DEL_Character EVENT_Character_Die;


    public IEnumerator DelayCharacterCreate(float _time = 1f) {
        yield return new WaitForSeconds(_time);

        if (!_playerCharacter) {
            // if (CurrentCharacter != null) {
            _playerCharacter = Instantiate(CurrentCharacter.Prefab, _characterParent);
            //  } else {
            //GameObject random_Character = _list_prefabCharacters[Random.Range(0, _list_prefabCharacters.Count)];
            //_playerCharacter = Instantiate(random_Character, _characterParent);
            //}

            _playerCharacter.transform.position = Platform.Current.CharacterBase.position;
        }

        CurrentCharacter = _playerCharacter.GetComponent<Character>();
        PlayerManager.Reset();

    }




    private void HANDLER_Character_Created() {
        Debug.Log("HANDLER_Character_Created Event Received");
        //StartCoroutine(DelayCharacterCreate(1f));


        if (!_playerCharacter) {
            if (CurrentCharacter == null) {
                string tempCharName = PlayerPrefs.GetString(GameManager.str_CURRENT_CHARACTER_KEY);
                ScrollPanel.FetchCurrentCharacterData(tempCharName);
            }

            _playerCharacter = Instantiate(CurrentCharacter.Prefab, _characterParent);
            _playerCharacter.transform.position = Platform.Current.CharacterBase.position;
        }

        CurrentCharacter = _playerCharacter.GetComponent<Character>();
        PlayerManager.Reset();


    }

    private void HANDLER_Character_Changed() {
        Debug.Log("HANDLER_Character_Changed Event Received");

        //Destroy(_playerCharacter);
        /* 
        if (_playerCharacter) {
            Destroy(_playerCharacter);
        } */

        //_playerCharacter = Instantiate(CurrentCharacter.Prefab, _characterParent);
        //_playerCharacter.transform.position = Platform.Current.CharacterBase.position;
        //CurrentCharacter = _playerCharacter.GetComponent<Character>();
        PlayerManager.Reset();


        string strCurrentCharacterKey = "CURRENT_CHARACTER";
        PlayerPrefs.SetString(strCurrentCharacterKey, CurrentCharacter.Name);

        Debug.Log("Current Character Name : " + CurrentCharacter.Name);
        UIManager.Instance.Refresh_Character(CurrentCharacter, true);

    }

    private void HANDLER_Character_Attack() {
        Debug.Log("HANDLER_Character_Attack Event Received");
    }

    private void HANDLER_Character_Unlocked() {
        Debug.Log("HANDLER_Character_Unlocked Event Received");
        UIManager.Instance.Refresh_Character(CurrentCharacter, true);

    }

    private void HANDLER_Character_Run() {
        Debug.Log("HANDLER_Character_Run Event Received");
    }

    private void HANDLER_Character_Die() {
        Debug.Log("HANDLER_Character_Die Event Received");

    }


    private void Awake() {
        EVENT_Character_Created += HANDLER_Character_Created;
        EVENT_Character_Attack += HANDLER_Character_Attack;
        EVENT_Character_Changed += HANDLER_Character_Changed;
        EVENT_Character_Unlocked += HANDLER_Character_Unlocked;
        EVENT_Character_Run += HANDLER_Character_Run;
        EVENT_Character_Die += HANDLER_Character_Die;
    }

    private void OnDestroy() {
        EVENT_Character_Created -= HANDLER_Character_Created;
        EVENT_Character_Attack -= HANDLER_Character_Attack;
        EVENT_Character_Changed -= HANDLER_Character_Changed;
        EVENT_Character_Unlocked -= HANDLER_Character_Unlocked;
        EVENT_Character_Run -= HANDLER_Character_Run;
        EVENT_Character_Die -= HANDLER_Character_Die;
    }

    public Character GetCharacter(int index) {
        return _list_prefabCharacters[index].GetComponent<Character>();

    }

    public static void Create() {
        EVENT_Character_Created();
    }

    public static void Unlocked() {
        EVENT_Character_Unlocked();
    }

    public static void Changed(Character _selectedCharater) {
        CurrentCharacter = _selectedCharater;
        EVENT_Character_Changed();
    }

    public static void Run(Transform _transTarget, bool victoryRun = false) {
        EVENT_Character_Run();
        CurrentCharacter.StartMoving(_transTarget, victoryRun);
    }


    public static void DestroyCharacterIfOnPlatform() {

        //Destroy(_playerCharacter);
        if (_playerCharacter) {
            Destroy(_playerCharacter);
        }
    }

    public static void Die() {
        CurrentCharacter.Die();
        EVENT_Character_Die();
    }

    public static void Reset() {

        Weapon.Changed();
        CurrentCharacter.Reset();
    }




}