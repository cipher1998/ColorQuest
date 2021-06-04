using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ScrollPanel : MonoBehaviour {


    [Header("Max  No of Elements to Choose from in Selection Panel")]
    [Range(1, 100)]
    [SerializeField] private int _elementsToDisplayCount;


    [Header("Select Smooth speed")]
    [Range(0.05f, 0.5f)]
    [SerializeField] private float _smoothSpeed;

    [SerializeField] private float _smoothSpeedX = 10f;


    [Header("Select distance between objects")]
    [Range(5, 20)]
    [SerializeField] private int _distanceBetweenElements;


    [Header("Select names for your objects")]
    //public string[] names;
    private GameObject[] instatiatedObj;

    [SerializeField] private static List<Character> _characters;
    private bool _charactersInitialized = false;

    private Vector2[] points;
    public GameObject parentScroll;

    private float smoothedX, smoothedScale;
    private float defaultScaleIncreament = 50, bigScaleIncreament = 250;
    //private float defaultScaleIncreament = 0, bigScaleIncreament = 0;
    private Vector3[] defaultScale, bigScale;

    private static ScrollPanel Instance;




    private void Awake() {
        Instance = this;
    }


    void Start() {
        instatiatedObj = new GameObject[_elementsToDisplayCount];

        points = new Vector2[_elementsToDisplayCount + 1];
        defaultScale = new Vector3[_elementsToDisplayCount];
        bigScale = new Vector3[_elementsToDisplayCount];

        CreateCharacterData();


        for (int i = 0; i < _elementsToDisplayCount; i++) {
            if (i == 0) instatiatedObj[i] = Instantiate(_characters[i].Prefab, new Vector3(0, parentScroll.transform.position.y, 75), Quaternion.identity);
            if (i != 0) instatiatedObj[i] = Instantiate(_characters[i].Prefab, new Vector3(instatiatedObj[i - 1].transform.position.x + _distanceBetweenElements,
                     instatiatedObj[i - 1].transform.position.y, instatiatedObj[i - 1].transform.position.z), Quaternion.identity);

            instatiatedObj[i].transform.parent = parentScroll.transform;
            instatiatedObj[i].transform.eulerAngles = new Vector3(0, 209f, 0);

            if (instatiatedObj[i].GetComponent<Rigidbody>()) {
                Destroy(instatiatedObj[i].GetComponent<Rigidbody>());
            }

            defaultScale[i] = new Vector3(instatiatedObj[i].transform.localScale.x + defaultScaleIncreament, instatiatedObj[i].transform.localScale.y + defaultScaleIncreament, instatiatedObj[i].transform.localScale.z + defaultScaleIncreament);

            bigScale[i] = new Vector3(instatiatedObj[i].transform.localScale.x + bigScaleIncreament, instatiatedObj[i].transform.localScale.y + bigScaleIncreament, instatiatedObj[i].transform.localScale.z + bigScaleIncreament);

        }

        for (int y = 0; y < _elementsToDisplayCount + 1; y++) {
            if (y == 0) points[y] = new Vector2(parentScroll.transform.position.x + _distanceBetweenElements / 2, parentScroll.transform.position.y);
            if (y != 0) points[y] = new Vector2(points[y - 1].x - _distanceBetweenElements, parentScroll.transform.position.y);
        }

        //RotateCharacters();

    }

    void Update() {

        for (int i = 0; i < _elementsToDisplayCount; i++) {

            if (parentScroll.transform.position.x < points[i].x && parentScroll.transform.position.x > points[i + 1].x) {
                smoothedX = Mathf.SmoothStep(parentScroll.transform.position.x, points[i].x - _distanceBetweenElements / 2, _smoothSpeed);
                parentScroll.transform.position = Vector2.Lerp(parentScroll.transform.position, new Vector2(points[i].x - _distanceBetweenElements / 2, parentScroll.transform.position.y), _smoothSpeed);
                smoothedScale = Mathf.SmoothStep(bigScale[i].x, defaultScale[i].x, _smoothSpeed);

                UIManager.Instance.Refresh_Character(_characters[i]);

            } else {
                smoothedScale = Mathf.SmoothStep(defaultScale[i].x, bigScale[i].x, _smoothSpeed);

            }

            instatiatedObj[i].transform.localScale = new Vector3(smoothedScale, smoothedScale, smoothedScale);
        }

        if (parentScroll.transform.position.x > points[0].x) {
            parentScroll.transform.position = Vector2.Lerp(parentScroll.transform.position, new Vector2(points[0].x - _distanceBetweenElements / 2, parentScroll.transform.position.y), _smoothSpeed);

        } else if (parentScroll.transform.position.x < points[_elementsToDisplayCount].x) {
            parentScroll.transform.position = Vector2.Lerp(parentScroll.transform.position, new Vector2(points[_elementsToDisplayCount].x + _distanceBetweenElements / 2, parentScroll.transform.position.y), _smoothSpeed);

        }


        //parentScroll.transform.position = new Vector2(smoothedX, parentScroll.transform.position.y);
        //parentScroll.transform.position = Vector2.Lerp(parentScroll.transform.position, new Vector2(smoothedX, parentScroll.transform.position.y), 5f);

    }

    public static void CreateCharacterData() {
        if (!Instance._charactersInitialized) {
            _characters = new List<Character>();
            for (int i = 0; i < Instance._elementsToDisplayCount; i++) {
                _characters.Add(Instance.GetComponent<PlayerManager>().GetCharacter(i));
            }
            Instance._charactersInitialized = true;
        }

    }

    public static void ResetCharacterData() {
        CreateCharacterData();

        for (int i = 0; i < _characters.Count; i++) {
            _characters[i].Lock();
        }

        _characters[0].Unlock(true);
        PlayerManager.Changed(_characters[0].Selected());
        UIManager.Instance.Refresh_Character(_characters[0], false);

    }

    public static void FetchCurrentCharacterData(string _characterName) {
        CreateCharacterData();
        int _currCharacterIndex = 0;

        for (int i = 0; i < _characters.Count; i++) {
            if (string.Equals(_characterName, _characters[i].Name)) {
                _currCharacterIndex = i;
                Debug.Log("Saved Charcter Data Found");
                break;
            }
        }

        PlayerManager.Changed(_characters[_currCharacterIndex].Selected());
        UIManager.Instance.Refresh_Character(_characters[_currCharacterIndex], false);

    }



}
