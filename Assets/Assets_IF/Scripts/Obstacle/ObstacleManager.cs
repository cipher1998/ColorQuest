using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InvisibleFiction;

public class ObstacleManager : MonoBehaviour {

    [SerializeField] private Material _blackMat;

    [SerializeField] private List<GameObject> _list_prefabObstacle;
    //[SerializeField] private GameObject _prefabObstacle;
    [SerializeField] private List<Obstacle> _listObstacle;
    [SerializeField] private int _minNoOfObstacle;
    [SerializeField] private int _maxNoOfObstacle;
    [SerializeField] private Vector3 _scaleChange;
    [SerializeField] private Vector3 _positionChange;

    private int _currentObstacleLevel;
    private int _maxObstacle;


    public delegate void DEL_Obstacles();
    public static event DEL_Obstacles EVENT_ObstaclesColorChanged;
    public static event DEL_Obstacles EVENT_ObstaclesColorMatched;
    public static event DEL_Obstacles EVENT_Obstacles_Destroyed;
    public static event DEL_Obstacles EVENT_Obstacles_Created;



    private void HANDLER_ObstaclesColorChanged() {
        Debug.Log("HANDLER_ObstaclesColorChanged Event Received");

    }

    private void HANDLER_ObstaclesColorMatched() {
        Debug.Log("HANDLER_ObstaclesColorMatched Event Received");
        _listObstacle.RemoveAt(0);
        PaintBallManager.PaintBall_ColorChange();

    }

    private void HANDLER_Obstacles_Destroyed() {
        Debug.Log("HANDLER_Obstacles_Destroyed Event Received");
        SoundManager.PlayAudio(SoundManager.Get(Sounds.obstacleShattered));
        //_listObstacle.RemoveAt(0);
        int _maxBlackSideCount = 0;
        if (_listObstacle.Count > 0) {
            _maxBlackSideCount = _listObstacle[0].GetObstacleSideCount() - 2;
            _listObstacle[0].SettingCurrentObstacle(true);
        }

        for (int i = 0; i < _currentObstacleLevel; i++) {
            if (_listObstacle.Count > 0 && i < _maxBlackSideCount) {
                _listObstacle[0].Set_Obstacle_Black_Color();
            }
        }

        _currentObstacleLevel++;

        if (_currentObstacleLevel > _maxObstacle) {
            Debug.Log("Last Obstacle Destroyed (Level Clear)");
            //PlayerManager.Run(Platform.Current.Base.GetChild(0).transform, true);
            //PaintBallManager.PaintBall_LoadUnload(false);
            GameManager.UpdateGameState(LevelState.CLEARED);

        } else {
            StartCoroutine(WaitForObstacleToLandOnPlane(true));
        }
    }

    private void HANDLER_Obstacles_Created() {

        this.transform.SetParent(Platform.Current.Base);
        this.transform.localPosition = Vector3.zero;
        this.transform.SetParent(UIManager.Instance.GamePlayLayer);

        Debug.Log("Creating Disk Obstacles");

        int prefabIndex = (int)LevelManager.Current_Level - 1;
        while (prefabIndex > _list_prefabObstacle.Count - 1) {
            prefabIndex -= _list_prefabObstacle.Count;
        }

        /*      if (LevelManager.Current_Level < _maxNoOfObstacle) {
                    _maxObstacle = LevelManager.Current_Level;
                } else {
                    _maxObstacle = _maxNoOfObstacle;
                }

                _maxObstacle = 5; */

        _maxObstacle = LevelManager.Current_Level %= _maxNoOfObstacle;
        if (_maxObstacle == 0) {
            _maxObstacle = _maxNoOfObstacle;
        }


        if (_maxObstacle < _minNoOfObstacle) {
            _maxObstacle += _minNoOfObstacle - 1;
        }

        Debug.Log("Current Obstacle Prefab Index : " + prefabIndex);
        GameObject random_prefabObstacle = _list_prefabObstacle[prefabIndex];

        if (_listObstacle.Count >= 0) {
            foreach (Obstacle go in _listObstacle) {
                if (go.gameObject) {
                    Destroy(go.gameObject);
                }
            }
        }


        _listObstacle = new List<Obstacle>();
        _currentObstacleLevel = 1;

        //int newColorIndex = Random.Range(1, 8);
        int newColorIndex = LevelManager.Current_Level %= 8;
        newColorIndex++;

        ColorData _tempColorData = ColorMixerClass.Instance.GetColor(newColorIndex);

        for (int i = 0; i < _maxObstacle; i++) {
            GameObject newObstacle = Instantiate(random_prefabObstacle, this.transform);
            newObstacle.transform.position += _positionChange * i;
            newObstacle.transform.localScale -= _scaleChange * i;
            newObstacle.name = "obstacle_" + i;
            //newObstacle.layer = 9;

            newObstacle.transform.localPosition = new Vector3(0, newObstacle.transform.localPosition.y, 0);
            newObstacle.GetComponent<Obstacle>().Set_Obstacle_Part_Color(_tempColorData, i);
            _listObstacle.Add(newObstacle.GetComponent<Obstacle>());
        }

        SetBlackMaterialColor(_tempColorData.colorCode);

        int _maxBlackSideCount = 0;
        if (_listObstacle.Count > 0) {
            _maxBlackSideCount = _listObstacle[0].GetObstacleSideCount() - 2;
        }

        for (int i = 0; i < _currentObstacleLevel; i++) {
            if (_listObstacle.Count > 0 && i < _maxBlackSideCount) {
                _listObstacle[0].Set_Obstacle_Black_Color();
            }
        }




        StartCoroutine(WaitForObstacleToLandOnPlane(false));

    }

    private void SetBlackMaterialColor(Color _tempColor) {
        _blackMat.color = _tempColor;
        _blackMat.SetColor("_BaseColor", _tempColor);
        _blackMat.SetColor("_HColor", _tempColor);
        _blackMat.SetColor("_SColor", _tempColor);

        //_blackMat.SetColor("_Emission", _tempColor);
    }




    private void Awake() {
        /*
            if (TransformBase == null) {
            TransformBase = this.transform;
        } */

        EVENT_ObstaclesColorChanged += HANDLER_ObstaclesColorChanged;
        EVENT_ObstaclesColorMatched += HANDLER_ObstaclesColorMatched;
        EVENT_Obstacles_Destroyed += HANDLER_Obstacles_Destroyed;
        EVENT_Obstacles_Created += HANDLER_Obstacles_Created;

    }

    private void Start() {

    }

    private void OnDestroy() {
        EVENT_ObstaclesColorChanged -= HANDLER_ObstaclesColorChanged;
        EVENT_ObstaclesColorMatched -= HANDLER_ObstaclesColorMatched;
        EVENT_Obstacles_Destroyed -= HANDLER_Obstacles_Destroyed;
        EVENT_Obstacles_Created -= HANDLER_Obstacles_Created;

    }

    public static void Created() { EVENT_Obstacles_Created(); }

    public static void ColorChanged() { EVENT_ObstaclesColorChanged(); }
    public static void ColorMatched() {
        EVENT_ObstaclesColorMatched();
    }

    public static void Destroyed(GameObject objectToDestroy) {
        //LevelManager.GemPiles.transform.GetChild(0).gameObject.SetActive(true);
        //LevelManager.GemPiles.transform.GetChild(0).GetComponent<IFShatter>().StartCutting(objectToDestroy);
        //SoundManager.PlayAudio(SoundManager.Get(Sounds.obstacleShattered));
        Destroy(objectToDestroy);
        EVENT_Obstacles_Destroyed();
    }


    private IEnumerator WaitForObstacleToLandOnPlane(bool alreadyCreated) {
        GameObject obstaclePiece = _listObstacle[0].gameObject;
        bool isLanded = false;

        while (!isLanded) {
            yield return new WaitForEndOfFrame();
            if (alreadyCreated) {
                if (obstaclePiece.transform.GetComponent<Rigidbody>().velocity.y == 0) {
                    isLanded = true;
                } else {
                    isLanded = false;
                }

            } else {
                obstaclePiece = _listObstacle[_listObstacle.Count - 1].gameObject;
                isLanded = GroundCheck(obstaclePiece.transform);
            }
        }

        _listObstacle[0].SettingCurrentObstacle(true);

        Weapon.Created();

        if (_listObstacle.Count >= 0) {

            foreach (Obstacle go in _listObstacle) {
                go.transform.localPosition = new Vector3(0, go.transform.localPosition.y, 0);
            }
        }
        RotationManager.StartRotating(true);

        PaintBallManager.PaintBall_LoadUnload(true);
    }

    private bool GroundCheck(Transform _trans) {
        //Debug.LogError("Ground Detetion Check");
        float distance = 1f;
        Vector3 dir = new Vector3(0, -1);
        //Debug.DrawRay(_trans.position, dir, Color.green, distance, false);
        if (Physics.Raycast(_trans.position, dir, out RaycastHit hit, distance)) {
            Debug.Log("Ground Detected : " + _trans.gameObject.name);
            return true;
        } else {
            return false;
        }

    }


}
