using UnityEngine;
using System.Collections.Generic;
using InvisibleFiction;
using InvisibleFiction.TwistHit;
[CreateAssetMenu(fileName = "RotationPreset", menuName = "Color TwistHit/RotationPreset", order = 0)]


public class RotationPreset : ScriptableObject {
    [Header("x: for Seconds, y: for Speed")]
    public Vector2[] rotData = new Vector2[10];


    public float GetRotation_TimeDuration(int index) {
        return rotData[index].x;
    }

    public float GetRotation_Speed(int index) {
        return rotData[index].y;
    }


}


