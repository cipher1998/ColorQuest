using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UITwean : MonoBehaviour {


    [SerializeField] private float _leanTIme = 0.25f;

    [SerializeField] private Vector2 _initialPos;
    [SerializeField] private Vector2 _enablePos;


    private void OnEnable() {
        this.GetComponent<RectTransform>().DOAnchorPos(_enablePos, _leanTIme);
    }

    public void DisableTwean(bool _disableOnFinish = false) {
        this.GetComponent<RectTransform>().DOAnchorPos(_initialPos, _leanTIme).OnComplete(() => gameObject.SetActive(_disableOnFinish));
    }


}