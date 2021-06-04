using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InvisibleFiction;


public class PlatformManager : MonoBehaviour {
    [SerializeField] private GameObject _mainPlatform;
    [SerializeField] private List<GameObject> _listPlatforms = new List<GameObject>();
    [SerializeField] public int _counter;
    public static PlatformManager Instance;

    public static int Counter { get { return Instance._counter; } }


    private void Awake() {
        Instance = this;
    }

    public static void Reset() {
        Debug.Log("Reseting Platform List and Counter");

        foreach (var _platform in Instance._listPlatforms) {
            Destroy(_platform.gameObject);
        }
        Instance._listPlatforms.Clear();
        Instance._listPlatforms = new List<GameObject>();
        Instance._counter = 1;
        LevelManager.DestroyGemPiles();
        Platform.Current = Instance._mainPlatform.GetComponent<Platform>();
        Platform.Current.GeneratePlatform(true);
        CameraController.LerpTowards(Platform.Current.CameraTransform.GetChild(0), 1f);
        LightController.LerpTowards(Platform.Current.LightTransform, 1f);
        //Platform.Current = null;

    }

    public static void Add(GameObject _newPlatform) {
        Instance._listPlatforms.Add(_newPlatform);
        Instance._counter++;

        if (Instance._counter > 4) {
            Debug.Log("Destroy Platform from list at 0");
            Destroy(Instance._listPlatforms[0].gameObject);
            Instance._listPlatforms.RemoveAt(0);
        }
    }


}


