using UnityEngine;
using InvisibleFiction;
using InvisibleFiction.TwistHit;

[CreateAssetMenu(fileName = "ColorData", menuName = "Color TwistHit/ColorData", order = 0)]
public class ColorData : ScriptableObject {
    public IFColor colorName;
    public Material colorMat;
    public Color colorCode;
    public Color[] colorRange = new Color[6];

}