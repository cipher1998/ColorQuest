using UnityEngine;
using System.Collections.Generic;
using InvisibleFiction;
using InvisibleFiction.TwistHit;

public class Obstacle : MonoBehaviour {

    [SerializeField] private ColorClass[] _listObstacleColorData;

    [SerializeField] private string _obstacleColorString;
    [SerializeField] private int[] _obstacleColorCode;
    [SerializeField] private bool _colorMatch = false;

    [SerializeField] private int _obstacleCurrentColor = (int)IFColor.White;

    public static Transform CurrentObstacleTransform;
    private bool _refreshRequired = true, _initialized = false;

    private void Awake() {

        _listObstacleColorData = this.GetComponentsInChildren<ColorClass>();
        _obstacleColorCode = new int[_listObstacleColorData.Length];
    }

    private void Start() {
        ObstacleManager.EVENT_ObstaclesColorChanged += this.HANDLER_ObstaclesColorChanged;
        ObstacleManager.EVENT_ObstaclesColorMatched += this.HANDLER_ObstaclesColorMatched;

        _initialized = false;
        ObstacleManager.ColorChanged();

    }

    private void OnDestroy() {
        ObstacleManager.EVENT_ObstaclesColorChanged -= this.HANDLER_ObstaclesColorChanged;
        ObstacleManager.EVENT_ObstaclesColorMatched -= this.HANDLER_ObstaclesColorMatched;
    }

    private void RefreshObstacleColorSting() {
        Debug.Log($"Refreshing Obstacle Color String of {this.gameObject.name}");
        _obstacleColorString = "";
        if (!this._colorMatch) {
            _colorMatch = true;
            _obstacleCurrentColor = (int)IFColor.White;
            bool _hasWhiteBlock = false;

            for (int i = 0; i < _listObstacleColorData.Length; i++) {
                if (!_initialized) {
                    if (_listObstacleColorData[i].GetColorData().colorName == IFColor.Black) {
                        _obstacleColorCode[i] = (int)IFColor.Black;
                    } else {
                        _obstacleColorCode[i] = (int)IFColor.White;
                    }

                } else {
                    //Debug.Log($"{_listObstacleColorData[i].name} :=> {_listObstacleColorData[i].GetColorData().colorName}");
                    if (_listObstacleColorData[i].ColorUpdated) {
                        _obstacleColorCode[i] = (int)_listObstacleColorData[i].GetColorData().colorName;
                    }

                }


                if ((_obstacleCurrentColor == (int)IFColor.White
                    || _obstacleCurrentColor == (int)IFColor.Black)
                    && _obstacleCurrentColor != _obstacleColorCode[i]) {
                    _obstacleCurrentColor = _obstacleColorCode[i];
                }
                if (i > 0) {
                    _obstacleColorString += ",";
                }

                if (_obstacleColorCode[i] == (int)IFColor.White) {
                    _hasWhiteBlock = true;
                } else if (_obstacleColorCode[i] != (int)IFColor.Black) {
                    if (_obstacleColorCode[i] != _obstacleCurrentColor) {
                        _colorMatch = false;
                    }
                }

                _obstacleColorString += _obstacleColorCode[i].ToString();
            }

            if (!_initialized) { _initialized = true; }

            if (_colorMatch && !_hasWhiteBlock && _obstacleCurrentColor != (int)IFColor.White) {
                Debug.Log($"Obstacle Curr Color : {_obstacleCurrentColor}  of {this.gameObject.name}");
                ObstacleManager.ColorMatched();
            } else {
                _colorMatch = false;
            }


        }

    }

    private void HANDLER_ObstaclesColorChanged() {
        if (this._refreshRequired) {
            RefreshObstacleColorSting();
            this.RefreshRequired(false);
        }

    }

    private void HANDLER_ObstaclesColorMatched() {
        if (this._colorMatch) {
            Debug.Log($"Ring Color Match : {ColorMixerClass.Instance.GetColor(_obstacleCurrentColor)}, ({_obstacleCurrentColor})");
            SoundManager.PlayAudio(SoundManager.Get(Sounds.obstacleShattered));
            LevelManager.GemPiles.transform.GetChild(0).gameObject.SetActive(true);
            LevelManager.GemPiles.transform.GetChild(0).GetComponent<IFShatter>().StartCutting(this.gameObject);
        }
    }


    public static void ColorChanged(Obstacle updatedObstacle) {
        updatedObstacle.RefreshRequired(true);
        ObstacleManager.ColorChanged();
    }

    public void RefreshRequired(bool value) {
        _refreshRequired = value;
    }

    public int GetObstacleSideCount() {
        return _obstacleColorCode.Length;
    }

    public void Set_Obstacle_Black_Color() {

        int index_ObstaclePart = Random.Range(0, _obstacleColorCode.Length);
        SetObstacle_Black(_listObstacleColorData[index_ObstaclePart]);
        RefreshObstacleColorSting();
        Obstacle.ColorChanged(this.GetComponent<Obstacle>());

    }

    public static void SetObstacle_Black(ColorClass _obstacleHit) {
        if (!_obstacleHit.gameObject.CompareTag("Obstacle_Black")) {
            _obstacleHit.ChangeColorData(ColorMixerClass.Instance.GetColor((int)IFColor.Black));
            _obstacleHit.gameObject.tag = "Obstacle_Black";
            _obstacleHit.GetComponent<Renderer>().material = ColorMixerClass.Instance.GetColor((int)IFColor.Black).colorMat;
            //_obstacleHit.GetComponent<Renderer>().material.color = currentColor;
            //_obstacleHit.GetComponent<Renderer>().material.SetColor("_BaseColor", currentColor);
            //_obstacleHit.GetComponent<Renderer>().material.SetColor("_Emission", currentColor);
        }
    }



    public void SettingCurrentObstacle(bool value = true) {
        Debug.Log("Setting OBSTACLE Tag to :" + this.gameObject.name);
        this.gameObject.tag = "Obstacle";
        foreach (ColorClass _obstaclePart in _listObstacleColorData) {
            if (!_obstaclePart.gameObject.CompareTag("Obstacle_Black")) {
                _obstaclePart.gameObject.tag = "Obstacle";
            }

        }

        CurrentObstacleTransform = this.transform;
        Debug.Log($"Currernt Obstacle Transform Position : {CurrentObstacleTransform.position}");
        LevelManager.GenerateGemPiles();
    }


    public void Set_Obstacle_Part_Color(ColorData _temp, int _index) {
        Debug.Log($"Setting Obstacle Color Part :  {_temp.colorName} , {_index}");
        for (int i = 0; i < _listObstacleColorData.Length; i++) {
            _listObstacleColorData[i].SetColorData(_temp, _index);
        }

    }

}