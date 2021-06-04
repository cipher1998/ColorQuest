using UnityEngine;
using UnityEngine.UI;
using InvisibleFiction;
using InvisibleFiction.TwistHit;

public class ColorClass : MonoBehaviour {

    [SerializeField] private ColorData colorData;
    [SerializeField] private Color currentColor;
    [SerializeField] private float colorSpeed = 0.1f; // 0.0003484906 good value

    private float startTime;
    private bool updatingToNewColor = false;
    public bool ColorUpdated { get { return updatingToNewColor; } }

    private void Start() {
        //SetColorData();
        startTime = Time.time;
    }

    private void Update() {
        if (updatingToNewColor) {
            ChangingColor();
        }

    }

    public void Editor_ChangeSpeed(Slider colorSpeedSlider) {
        colorSpeed = colorSpeedSlider.value;
    }


    public ColorData GetColorData() {
        return this.colorData;
    }

    public void SetColorData() {
        //Debug.Log("Setting ColorData : " + colorData.colorName);        
        currentColor = colorData.colorCode;
        GetComponent<Renderer>().material.color = currentColor;
    }

    public void SetColorData(ColorData newColorData, int _index = 0) {
        colorData = newColorData;

        _index %= colorData.colorRange.Length;

        currentColor = colorData.colorRange[_index];
        Debug.Log($"Setting ColorData : {colorData.colorName}, {_index}, {currentColor} on {this.gameObject.name}");
        // GetComponent<Renderer>().material.color = currentColor;
        // GetComponent<Renderer>().material.SetColor("Color", currentColor);
        // if (!GetComponent<Renderer>().material.IsKeywordEnabled("_EMISSION")) {
        //     GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
        // }
        // GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColor);

        GetComponent<Renderer>().material.color = currentColor;
        GetComponent<Renderer>().material.SetColor("_BaseColor", currentColor);
        GetComponent<Renderer>().material.SetColor("_Emission", currentColor);

        GetComponent<Renderer>().material.SetColor("_HColor", currentColor);
        GetComponent<Renderer>().material.SetColor("_SColor", currentColor);

    }

    public void ChangeColorData(ColorData newColorData) {
        Debug.Log("Changing ColorData to : " + newColorData.colorName);
        colorData = newColorData;
        startTime = Time.time;
        updatingToNewColor = true;
    }

    public void ChangingColor() {

        if (currentColor == colorData.colorCode) {
            updatingToNewColor = false;
            //GetComponent<Renderer>().material.SetColor("_EmissionColor", currentColor);
            GetComponent<Renderer>().material = colorData.colorMat;
            Debug.Log("Finish Updating Color to : " + colorData.colorName);

        } else {
            float t = (Time.time - startTime) * colorSpeed;
            currentColor = Color.Lerp(currentColor, colorData.colorCode, t);
            GetComponent<Renderer>().material.Lerp(GetComponent<Renderer>().material, colorData.colorMat, t);
        }
    }


}
