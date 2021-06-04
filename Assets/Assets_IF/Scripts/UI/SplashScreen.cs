using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SplashScreen : MonoBehaviour {

    [SerializeField] private string sceneToLoadName;
    [SerializeField] private float sceneLoadWaitTime = 2f;
    private AsyncOperation asyncOperation;

    private bool loadingInProgress = false;

    [System.Obsolete]
    private void Awake() {
        GameManager.InitializeGameManager();
    }


    void Start() {
        //Start loading the Scene asynchronously and output the progress bar
        StartCoroutine(LoadSceneInBackground());

    }

    private void Update() {
        if (loadingInProgress) {
            Debug.Log("Loading progress: " + (asyncOperation.progress * 100) + "%");
            if (!asyncOperation.isDone && asyncOperation.progress >= 0.9f) {
                asyncOperation.allowSceneActivation = true;
            }
        }
    }

    private IEnumerator LoadSceneInBackground() {
        yield return new WaitForSeconds(sceneLoadWaitTime);
        asyncOperation = SceneManager.LoadSceneAsync(sceneToLoadName);
        asyncOperation.allowSceneActivation = false;
        loadingInProgress = true;
        /*
            while (!asyncOperation.isDone) {
            if (asyncOperation.progress >= 0.9f) {
                asyncOperation.allowSceneActivation = true;
            }
            yield return new WaitForEndOfFrame();

        } */
    }


}
