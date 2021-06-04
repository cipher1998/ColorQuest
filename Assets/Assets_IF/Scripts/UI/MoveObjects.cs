using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InvisibleFiction;

public class MoveObjects : MonoBehaviour {
    private Vector3 destination;
    private GameObject _totalGem, newGem = null;
    [SerializeField] private float moveSpeed = 0.05f;

    [SerializeField] private List<GameObject> _listGems;
    [SerializeField] private bool _destroyAfterMoved = true;

    private bool startMoving;

    private void Start() {
        if (_destroyAfterMoved) {
            Debug.Log("Destroying Extra Gems from GemPiles");
            while (_listGems.Count > LevelManager.LevelRewardGems) {
                Destroy(_listGems[0]);
                _listGems.RemoveAt(0);
            }
        }

    }

    private void OnEnable() {
        if (!_destroyAfterMoved) {
            Debug.Log("Reseting Pos for adReward-2x Button Image");
            this.gameObject.GetComponent<RectTransform>().Set(0, 0, 0, 0);
        }
    }

    private void OnDestroy() {
        if (newGem != null) {
            Destroy(newGem);
        }

        startMoving = false;
    }

    void FixedUpdate() {
        if (startMoving) {
            newGem.transform.position = Vector3.MoveTowards(newGem.transform.position, destination, moveSpeed);
            //Debug.Log(newGem.transform.position);
            //moveSpeed += 0.11f;
            if (Vector3.Distance(newGem.transform.position, destination) < 0.001f) {
                Debug.Log("Gem Reached to the holder");
                if (_listGems.Count > 1) {
                    if (_destroyAfterMoved) {
                        Destroy(newGem);
                    }
                    _listGems.RemoveAt(0);
                    newGem = _listGems[0];
                    LevelManager.AddGems(1);
                } else {
                    _totalGem.SetActive(true);
                    this.gameObject.SetActive(false);
                    startMoving = false;

                    if (_destroyAfterMoved) {
                        LevelManager.AddGems(1, true);
                        Destroy(newGem);
                    } else {
                        LevelManager.AddGems(LevelManager.LevelRewardGems);

                    }

                }

            }
        }
    }

    public IEnumerator GemCollected(GameObject _destinationObject) {
        //_totalGem = UIManager.Instance.GemUI();
        _totalGem = _destinationObject;
        destination = _totalGem.transform.position;
        Debug.Log($"Gem started Moving towards : {destination}");
        yield return new WaitForSeconds(0.1f);

        //newGem = this.gameObject;
        newGem = _listGems[0];

        foreach (Transform child in this.transform) {
            if (child.GetComponent<Rigidbody>()) {
                Destroy(child.GetComponent<Rigidbody>());
            }
        }

        startMoving = true;
    }




}
