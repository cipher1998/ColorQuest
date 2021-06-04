using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InvisibleFiction;

public class Platform : MonoBehaviour {

    [SerializeField] private Transform[] spawnPoints; // LeftPath. RightPath, Character
    [SerializeField] private Transform obstacleBase;
    [SerializeField] private Transform environment;

    [SerializeField] private Transform cameraAngle;
    [SerializeField] private Transform lightAngle;
    [SerializeField] private GameObject _gemInGameImage;

    [SerializeField] private GameObject nextPlatform;


    public Transform Base { get { return obstacleBase; } }
    public Transform CharacterBase { get { return spawnPoints[2]; } }
    public Transform CameraTransform { get { return cameraAngle; } }
    public Transform LightTransform { get { return lightAngle; } }
    public GameObject GemUI { get { return _gemInGameImage; } }


    public static Platform Current = null;



    private void Start() {
        GeneratePlatform();
    }


    public void GeneratePlatform(bool _generateNextPlatform = false) {
        if (!Current || _generateNextPlatform) {
            Current = this;
            int pathDirection = Random.Range(0, 2); // 0=> Left, 1=> Right

            if (nextPlatform != null) {
                Destroy(nextPlatform.gameObject);
            }


            nextPlatform = Instantiate(LevelManager.Platform, spawnPoints[pathDirection]);
            nextPlatform.transform.SetParent(this.transform.parent);
            nextPlatform.name = $"Platform_{PlatformManager.Counter}";
            PlatformManager.Add(nextPlatform.gameObject);

            int _environmentIndex = Random.Range(0, environment.childCount);
            environment.GetChild(_environmentIndex).gameObject.SetActive(true);

        }

    }



    public void ChangePlatform() {
        Current = nextPlatform.GetComponent<Platform>();
        Current.GeneratePlatform(true);
        //Camera.main.GetComponent<CameraController>().LerpCameraTowards(Current.CameraTransform.GetChild(0), 1f);
        CameraController.LerpTowards(Current.CameraTransform.GetChild(0), 1f);
        LightController.LerpTowards(Current.LightTransform, 1f);
        PlayerManager.CurrentCharacter.Reset();
        PlayerManager.Run(Current.CharacterBase, false);
        StartCoroutine(LoadNextLevel());

    }

    private IEnumerator LoadNextLevel() {
        yield return new WaitForSeconds(3f);
        GameManager.UpdateGameState(LevelState.LOADING);
    }

    public void WinParticle(bool _enable = true) {
        spawnPoints[0].gameObject.SetActive(_enable);
        spawnPoints[1].gameObject.SetActive(_enable);

    }



}






