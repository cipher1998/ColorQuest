using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionLightTest : MonoBehaviour {

    [SerializeField] private Transform _lookAt;


    // Update is called once per frame
    void Update() {
        transform.LookAt(_lookAt);

    }
}
