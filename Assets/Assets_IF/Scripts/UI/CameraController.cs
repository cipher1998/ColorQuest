using UnityEngine;
using InvisibleFiction;


public class CameraController : MonoBehaviour {

    private bool _isLerpingTransform = false;
    private float _distLeft = 0.01f;
    private Transform _nextTransform;

    public static CameraController Instance;


    private void Start() {
        Instance = this;
    }

    private void Update() {
        if (_isLerpingTransform) {
            transform.LerpTransform(_nextTransform, Time.deltaTime);

            if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(transform.rotation, _nextTransform.rotation)), 1.0f)) {
                if (_distLeft == 0.01f) {
                    float _dist = Vector3.Distance(transform.position, _nextTransform.position);

                    if (_dist <= _distLeft) {
                        Debug.Log("Camera Reached Destination");
                        _isLerpingTransform = false;
                        //GameManager.UpdateGameState(LevelState.LOADING);
                    }
                } else {
                    Debug.Log("Camera Reached Rotation");
                    _isLerpingTransform = false;
                    if (_nextTransform.childCount > 0) {
                        LerpTowards(_nextTransform.GetChild(0));
                    } else {
                        //GameManager.UpdateGameState(LevelState.LOADING);
                    }

                }

            }

        }

    }

    public static void LerpTowards(Transform _trans, float _dist = 0.01f) {
        if (Instance != null) {
            Instance._distLeft = _dist;
            Instance._nextTransform = _trans;
            Instance._isLerpingTransform = true;
            Debug.Log("Camera Lerp Started");
        }


    }


}