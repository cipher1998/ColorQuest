using UnityEngine;
using UnityEngine.UI;
using InvisibleFiction;

public class AdButtonTimer : MonoBehaviour {

    private float timeToDisplay = 10f;
    [SerializeField] private Image _image;
    [SerializeField] private Button _btn;
    private float _timer;
    private bool _timerStarted = false;


    public void StarTimer(bool value) {
        if (value) {
            Debug.Log($"Ad Button Timer Started ");
            _timer = timeToDisplay;
            _timerStarted = true;
            _btn.interactable = true;
            _btn.ResetMaterial(false);
            _image.fillAmount = 1;

        } else {
            _timer = 0;
            _timerStarted = true;
        }


    }


    void Update() {
        if (_timerStarted) {
            if (_timer > 0) {
                _timer -= Time.deltaTime;
                //Debug.Log($"Time : {_timer}");
                _image.fillAmount = _timer / timeToDisplay;
            } else {
                Debug.Log($"Button {this.gameObject.name} Disabled");
                _timerStarted = false;
                _btn.interactable = false;
                _btn.ResetMaterial();
                _image.fillAmount = 1;
            }
        }

    }

}
