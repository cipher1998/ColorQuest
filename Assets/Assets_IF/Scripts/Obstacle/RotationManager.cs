using UnityEngine;
using TMPro;

public class RotationManager : MonoBehaviour {
    [SerializeField] private Transform objectToRotate;
    [SerializeField] private RotationPreset currRotPreset;
    [SerializeField] private RotationPreset[] rotPresets;
    [SerializeField] private static bool enableRotation;
    public float _currRotTimeDuration = 1f; // Decrese over time or Level
    public float _currRotSpeed = 1f, _currRot_LevelMultiplier = 1;
    private int _currRotIndex = 0; // increase over Time or Level

    private Quaternion targetRotation;

    private float _newRotSpeed = 0, startTime;

    void Start() {
        _currRotIndex = -1;
        //int presetIndex = Random.Range(0, 20);
        _currRot_LevelMultiplier = 0.6f + (float)LevelManager.Current_Level / 20;
        int presetIndex = (int)LevelManager.Current_Level % 20;
        while (presetIndex > rotPresets.Length - 1) {
            presetIndex -= rotPresets.Length;
        }

        SetCurrentRotationPreset(presetIndex);
        GetNextRotation();

        targetRotation = objectToRotate.transform.rotation;
    }

    private void FixedUpdate() {
        if (enableRotation) {
            RotateObject();
            ChangeRotationSpeed();
        }
    }

    private void GetNextRotation() {
        _currRotIndex++;
        if (_currRotIndex > 9) { _currRotIndex = 0; }
        //Debug.Log("RotationManager => Getting Next Rotation : " + _currRotIndex);

        //_newRotSpeed = currRotPreset.GetRotation_Speed(_currRotIndex);  // 0.05f * levelIndex
        _newRotSpeed = currRotPreset.GetRotation_Speed(_currRotIndex) * _currRot_LevelMultiplier;
        _currRotTimeDuration = currRotPreset.GetRotation_TimeDuration(_currRotIndex);
        startTime = Time.time;
    }


    private void ChangeRotationSpeed() {
        if (_currRotTimeDuration < Time.time - startTime) {
            GetNextRotation();
        } else {
            _currRotSpeed = Mathf.Lerp(_currRotSpeed, _newRotSpeed, Time.deltaTime);
        }

    }

    private void RotateObject() {
        targetRotation *= Quaternion.AngleAxis(_currRotSpeed, Vector3.up);
        objectToRotate.transform.rotation = Quaternion.Slerp(objectToRotate.transform.rotation, targetRotation, 1);
    }

    public static void StartRotating(bool value) {
        enableRotation = value;
    }

    public void SetCurrentRotationPreset(int index) {
        //Update_labelRotation("Rotation Preset # " + index);
        Debug.Log("RotationManager => Using Rotation Preset # " + index);
        currRotPreset = rotPresets[index];
    }

}