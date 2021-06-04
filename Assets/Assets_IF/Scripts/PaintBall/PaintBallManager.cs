using UnityEngine;
using System.Collections.Generic;
using InvisibleFiction;
using InvisibleFiction.TwistHit;


public class PaintBallManager : MonoBehaviour {

    [Header("Paint-Balls")]
    [SerializeField] private GameObject prefabPaintBall;
    [SerializeField] private Transform _parentFiredPaintBalls;
    [SerializeField] private float _paintBallShootDelay = 0.3f;
    [SerializeField] private GameObject currentPaintBall;
    private bool _isFiring = false;
    private bool _stopFiring = false;
    private bool _paintBallsLoaded = false;

    public static int PaintBallCounter = 0;


    public delegate void DEL_PaintBalls();
    public static event DEL_PaintBalls EVENT_PaintBall_Destroyed;
    public static event DEL_PaintBalls EVENT_PaintBall_ChangeColor;
    public static event DEL_PaintBalls EVENT_PaintBall_Shoot;
    public static event DEL_PaintBalls EVENT_PaintBall_Loaded;
    public static event DEL_PaintBalls EVENT_PaintBall_Unload;
    private static ColorData paintBallNewColorData;

    public void PointerDown_FireButton() {
        _stopFiring = false;
        StartShooting_PaintBalls();
    }

    public void PointerUp_FireButton() {
        _isFiring = false;
        _stopFiring = true;
        if (currentPaintBall) {
            currentPaintBall.GetComponent<PaintBall>().DestroyPaintBall();
        }

    }

    public void StartShooting_PaintBalls() {
        _isFiring = true;
    }

    public void StopShooting_PaintBalls() {
        _isFiring = false;
        foreach (Transform child in this.transform) {
            Destroy(child.gameObject);
        }
        if (!_stopFiring) {
            Invoke("StartShooting_PaintBalls", _paintBallShootDelay);
        }
    }


    // Update is called once per frame
    void Update() {
        if (_paintBallsLoaded) {
            if (_isFiring) {
                EVENT_PaintBall_Shoot();
                StopShooting_PaintBalls();
            }
        } else {
            if (currentPaintBall) {
                currentPaintBall.GetComponent<PaintBall>().DestroyPaintBall();
            }
        }

    }

    private void Awake() {
        //_animator = this.GetComponent<Animator>();
        // RayBeamPrefab = _rayBeamPrefab;
    }

    private void Start() {

        EVENT_PaintBall_Destroyed += HANDLER_PaintBall_Destroyed;
        EVENT_PaintBall_ChangeColor += HANDLER_PaintBall_ChangeColor;
        EVENT_PaintBall_Shoot += HANDLER_PaintBall_Shoot;
        EVENT_PaintBall_Loaded += HANDLER_PaintBall_Loaded;
        EVENT_PaintBall_Unload += HANDLER_PaintBall_Unload;

        ObstacleManager.EVENT_ObstaclesColorMatched += DestroyFiredPaintBalls;

        //paintBallNewColorData = ColorMixerClass.Instance.GetColor(Random.Range(1, 8));

        int newColorIndex = LevelManager.Current_Level %= 8;
        newColorIndex++;
        paintBallNewColorData = ColorMixerClass.Instance.GetColor(newColorIndex);

        PaintBallCounter = 0;
        //Create_PaintBall();
        PaintBall_ColorChange();
    }

    private void OnDestroy() {
        EVENT_PaintBall_Destroyed -= HANDLER_PaintBall_Destroyed;
        EVENT_PaintBall_ChangeColor -= HANDLER_PaintBall_ChangeColor;
        EVENT_PaintBall_Shoot -= HANDLER_PaintBall_Shoot;
        EVENT_PaintBall_Loaded -= HANDLER_PaintBall_Loaded;
        EVENT_PaintBall_Unload -= HANDLER_PaintBall_Unload;

        ObstacleManager.EVENT_ObstaclesColorMatched -= DestroyFiredPaintBalls;
    }

    private void Create_PaintBall() {
        if (currentPaintBall == null && !_stopFiring) {

            currentPaintBall = Instantiate(prefabPaintBall, this.transform);
            //currentPaintBall.transform.localPosition = Vector3.zero;
            currentPaintBall.transform.position = Weapon.FirePoint.position;
            currentPaintBall.GetComponent<Renderer>().enabled = false;
            currentPaintBall.name = "InkDrop_" + PaintBallCounter++;
            EVENT_PaintBall_ChangeColor();
        }

    }

    public void RequestToShoot_PaintBall() {
        EVENT_PaintBall_Shoot();
    }

    public static void PaintBall_ColorChange(ColorData newColorData) {
        paintBallNewColorData = newColorData;

        EVENT_PaintBall_ChangeColor();
    }

    public static void PaintBall_ColorChange() {

        int newColorIndex = LevelManager.Current_Level %= 8;
        newColorIndex++;

        //int newColorIndex = (int)IFColor.White;

        paintBallNewColorData = ColorMixerClass.Instance.GetColor(newColorIndex);

        EVENT_PaintBall_ChangeColor();
    }

    public static void PaintBall_Destroyed(GameObject objectToDestroy) {
        Destroy(objectToDestroy);

        EVENT_PaintBall_Destroyed();
    }

    public static void PaintBall_LoadUnload(bool value) {
        if (value) {
            EVENT_PaintBall_Loaded();
        } else {
            EVENT_PaintBall_Unload();
        }
    }

    public static ColorData GetColorData() {
        return paintBallNewColorData;
    }

    private void HANDLER_PaintBall_ChangeColor() {
        Debug.Log("HANDLER_PaintBall_ChangeColor Event Received");
        //this.GetComponent<ColorClass>().SetColorData(paintBallNewColorData);

        if (currentPaintBall != null) {
            currentPaintBall.GetComponent<PaintBall>().ChangeColor(paintBallNewColorData);
        }

    }

    private void HANDLER_PaintBall_Destroyed() {
        Debug.Log("HANDLER_PaintBall_Destroyed Event Received");
    }

    private void HANDLER_PaintBall_Shoot() {
        Debug.Log("HANDLER_PaintBall_Shoot Event Received");

        if (currentPaintBall == null & _paintBallsLoaded) {
            Create_PaintBall();
        }

        if (currentPaintBall != null) {
            currentPaintBall.GetComponent<PaintBall>().FirePaintBall(_parentFiredPaintBalls);
            currentPaintBall = null;
        }
        Invoke("Create_PaintBall", 0.051f);
        //Create_PaintBall();
    }

    private void HANDLER_PaintBall_Loaded() {
        Debug.Log("HANDLER_PaintBall_Loaded Event Received");
        _paintBallsLoaded = true;
        _isFiring = false;
        //_animator.SetBool("isFiring", false);
    }

    private void HANDLER_PaintBall_Unload() {
        Debug.Log("HANDLER_PaintBall_Unload Event Received");
        _paintBallsLoaded = false;
        _isFiring = false;
        DestroyFiredPaintBalls();
        foreach (Transform child in this.transform) {
            Debug.Log($"child Paint Ball Name : {child.name}");
            Destroy(child.gameObject);
        }
    }

    public void DestroyFiredPaintBalls() {
        Debug.Log("Destroying Fired Bullets");
        foreach (Transform child in _parentFiredPaintBalls) {
            Destroy(child.gameObject);
        }

    }



}