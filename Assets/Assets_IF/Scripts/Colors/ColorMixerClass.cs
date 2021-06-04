using UnityEngine;
using InvisibleFiction;
using InvisibleFiction.TwistHit;

public class ColorMixerClass : MonoBehaviour {

    [SerializeField] private ColorData colorBlack;
    [SerializeField] private ColorData colorBlue;
    [SerializeField] private ColorData colorBrown;
    [SerializeField] private ColorData colorGreen;
    [SerializeField] private ColorData colorOrange;
    [SerializeField] private ColorData colorPurple;
    [SerializeField] private ColorData colorRed;
    [SerializeField] private ColorData colorWhite;
    [SerializeField] private ColorData colorYellow;

    public static ColorMixerClass Instance;

    private void Awake() {
        Instance = this;
    }

    public ColorData GetColor(int index) {

        IFColor newColor = (IFColor)index;
        //Debug.Log("Requested Color : " + newColor);
        ColorData newMixedColor = colorWhite;
        switch (newColor) {
            case IFColor.Red:
                newMixedColor = colorRed;
                break;
            case IFColor.Yellow:
                newMixedColor = colorYellow;
                break;
            case IFColor.Blue:
                newMixedColor = colorBlue;
                break;
            case IFColor.Green:
                newMixedColor = colorGreen;
                break;
            case IFColor.Purple:
                newMixedColor = colorPurple;
                break;
            case IFColor.Orange:
                newMixedColor = colorOrange;
                break;
            case IFColor.Brown:
                newMixedColor = colorBrown;
                break;
            case IFColor.White:
                newMixedColor = colorWhite;
                break;
            case IFColor.Black:
                newMixedColor = colorBlack;
                break;
        }

        return newMixedColor;
    }

    public int GetColorIndex(ColorData newColorData) {
        return (int)newColorData.colorName;
    }


    public void MixNewColor(GameObject objectToChangeColor, ColorData newColorData) {
        IFColor color1 = objectToChangeColor.GetComponent<ColorClass>().GetColorData().colorName;
        IFColor color2 = newColorData.colorName;

        ColorData newMixedColor = colorWhite;
        if (color2 == IFColor.White) {
            newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData();
        } else if (color2 == IFColor.Black) {
            newMixedColor = colorBlack;
            LevelFailed();

        } else {
            switch (color1) {
                case IFColor.Red:
                    if (color2 == IFColor.Yellow) { newMixedColor = colorOrange; } else
                    if (color2 == IFColor.Blue) { newMixedColor = colorPurple; } else
                    if (color2 == IFColor.Green) { newMixedColor = colorBrown; } else { newMixedColor = newColorData; }
                    break;
                case IFColor.Yellow:
                    if (color2 == IFColor.Red) { newMixedColor = colorOrange; } else
                    if (color2 == IFColor.Blue) { newMixedColor = colorGreen; } else
                    if (color2 == IFColor.Purple) { newMixedColor = colorBrown; } else { newMixedColor = newColorData; }
                    break;
                case IFColor.Blue:
                    if (color2 == IFColor.Red) { newMixedColor = colorPurple; } else
                    if (color2 == IFColor.Yellow) { newMixedColor = colorGreen; } else
                    if (color2 == IFColor.Orange) { newMixedColor = colorBrown; } else { newMixedColor = newColorData; }
                    break;
                case IFColor.Green:
                    if (color2 == IFColor.Green) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else
                    if (color2 == IFColor.Yellow) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else
                    if (color2 == IFColor.Blue) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else { newMixedColor = colorBrown; }
                    break;
                case IFColor.Purple:
                    if (color2 == IFColor.Purple) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else
                    if (color2 == IFColor.Red) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else
                    if (color2 == IFColor.Blue) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else { newMixedColor = colorBrown; }
                    break;
                case IFColor.Orange:
                    if (color2 == IFColor.Orange) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else
                    if (color2 == IFColor.Red) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else
                    if (color2 == IFColor.Yellow) { newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData(); } else { newMixedColor = colorBrown; }
                    break;
                case IFColor.Brown:
                    newMixedColor = objectToChangeColor.GetComponent<ColorClass>().GetColorData();
                    break;
                case IFColor.White:
                    newMixedColor = newColorData;
                    break;
                case IFColor.Black:
                    newMixedColor = colorBlack;
                    LevelFailed();
                    break;
            }
        }

        objectToChangeColor.GetComponent<ColorClass>().ChangeColorData(newMixedColor);
    }

    public static void LevelFailed() {
        GameManager.UpdateGameState(LevelState.FAILED);
    }



}
