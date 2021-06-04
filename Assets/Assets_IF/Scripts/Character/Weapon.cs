using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InvisibleFiction;

public class Weapon : MonoBehaviour {
    [SerializeField] private List<GameObject> _listWeaponPart;
    [SerializeField] private Transform _firePoint;

    private bool _isCreated = false;

    public static Transform FirePoint { get; set; }

    public delegate void DEL_Weapons();
    public static event DEL_Weapons EVENT_Weapon_Created;
    public static event DEL_Weapons EVENT_Weapon_ColorChanged;
    public static event DEL_Weapons EVENT_Weapon_Changed;
    public static event DEL_Weapons EVENT_Weapon_Destroyed;

    private void HANDLER_Weapon_Created() {
        if (_isCreated) {
            Debug.Log("HANDLER_Weapon_Created Event Received");
            FirePoint = this._firePoint;
        }
    }

    private void HANDLER_Weapon_Destroyed() {
        if (_isCreated) {
            Debug.LogError("HANDLER_Weapon_Destroyed Event Received");
        }
    }

    private void HANDLER_Weapon_Changed() {
        if (_isCreated) {
            if (this._firePoint != null) {
                Debug.Log("HANDLER_Weapon_Changed Event Received");
                FirePoint = this._firePoint;
                EVENT_Weapon_ColorChanged();
            }
        }
    }

    private void HANDLER_Weapon_ColorChanged() {
        if (_isCreated) {
            Debug.Log("HANDLER_Weapon_ColorChanged Event Received");

            // Changing Weapon Color    
            if (GetComponent<Renderer>()) {
                GetComponent<Renderer>().material.color = PaintBallManager.GetColorData().colorCode;
            }
            // Changing Weapon Particle Color
            foreach (GameObject weaponPart in _listWeaponPart) {
                var psMain = weaponPart.GetComponent<ParticleSystem>().main;
                psMain.startColor = PaintBallManager.GetColorData().colorCode;
            }
        }
    }

    private void Awake() {
        EVENT_Weapon_Created += HANDLER_Weapon_Created;
        EVENT_Weapon_Changed += HANDLER_Weapon_Changed;
        EVENT_Weapon_ColorChanged += HANDLER_Weapon_ColorChanged;
        EVENT_Weapon_Destroyed += HANDLER_Weapon_Destroyed;

    }

    private void Start() {
        _isCreated = true;
    }

    private void OnDestroy() {

        EVENT_Weapon_Created -= HANDLER_Weapon_Created;
        EVENT_Weapon_Changed -= HANDLER_Weapon_Changed;
        EVENT_Weapon_ColorChanged -= HANDLER_Weapon_ColorChanged;
        EVENT_Weapon_Destroyed -= HANDLER_Weapon_Destroyed;
    }

    public static void ColorChanged() {
        EVENT_Weapon_ColorChanged();
    }

    public static void Created() {
        EVENT_Weapon_Created();
        EVENT_Weapon_ColorChanged();
    }

    public static void Changed() {
        if (FirePoint != null) {
            //EVENT_Weapon_Changed();
        }

    }

}