using UnityEngine;
using System.Collections;
using InvisibleFiction;

public class Character : MonoBehaviour {

    [SerializeField] private CharacterData data;
    public string Name { get { return data._characterName; } }
    public int UnlockPrice { get { return data._characterUnlockPrice; } }
    public bool IsUnlocked { get { return data._characterUnlocked; } }
    public GameObject Prefab { get { return this.gameObject; } }


    private bool _startMoving = false;
    private bool _startRotating = false;
    private bool _isDying = false;
    private bool _victoryRun = false;

    private float runSpeed = 5f;
    private Vector3 targetPos;

    public Transform targetObjLookAt;
    public int lookatSpeed = 5;

    Animator _animator;


    private void Start() {
        targetObjLookAt = Camera.main.transform;
        _animator = gameObject.GetComponent<Animator>();
    }



    private void FixedUpdate() {

        if (_startMoving) {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, runSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.1f) {
                _startMoving = false;
                _animator.SetBool("Run", false);

                if (_victoryRun) {
                    _startRotating = true;
                    _animator.SetBool("Victory", true);
                    StartCoroutine(LevelManager.GemPiles.GetComponent<MoveObjects>().GemCollected(Platform.Current.GemUI));
                    //LevelManager.AddGems(LevelManager.LevelRewardGems);
                } else {

                }

            }
        }

        if (_startRotating) {
            Quaternion targetRotation = Quaternion.LookRotation(targetObjLookAt.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, lookatSpeed * Time.deltaTime);

            if (Mathf.Approximately(Mathf.Abs(Quaternion.Dot(transform.rotation, targetRotation)), 1.0f)) {
                _startRotating = false;

                if (_victoryRun) {
                    _victoryRun = false;
                    UIManager.Instance.LevelClearPanel_Open();
                } else {
                    _startMoving = true;
                }
            }
        }

        if (_isDying) {

            if (!this.GetComponent<Animator>().enabled) {
                _isDying = false;
                UIManager.Instance.LevelFailedPanel_Open();
            }
        }

    }

    public void Unlock(bool isFreeUnlock = false) {
        if (!isFreeUnlock) {
            LevelManager.AddGems(-UnlockPrice);
        }
        data.UnlockCharacter(isFreeUnlock);
    }

    public void Lock() {
        data.ResetAndLockCharacter();
    }

    public Character Selected() {
        return this;
    }

    public void StartMoving(Transform transTarget, bool victoryRun = false) {

        _victoryRun = victoryRun;
        targetPos = transTarget.position;

        if (_victoryRun) {
            targetObjLookAt = Camera.main.transform;
            StartCoroutine(StartVictoryRun());
        } else {
            targetObjLookAt = transTarget;
            _startRotating = true;
            LevelManager.FetchLevelData();
            Debug.Log($"Current Pos : {transform.position} , => To Next Position : {targetPos}");
            targetPos.y = transform.position.y;
            //targetPos.y = 0f;
            _animator.SetBool("Run", true);
        }


    }

    private IEnumerator StartVictoryRun() {

        yield return new WaitForSeconds(1);
        _startMoving = true;
        //_animator.SetBool("Run", true);
        Debug.Log($"Current Pos : {transform.position} , => To Next Position : {targetPos}");
        targetPos.y = transform.position.y;
        //targetPos.y = 0f;
        _animator.SetBool("Run", true);
    }


    public void Die() {
        _isDying = true;
        _animator.ResetTrigger("Die");
        _animator.SetTrigger("Die");
        //Debug.LogError("Started Character Dying Animation");
    }

    public void Reset() {
        Debug.Log("Reseting Current Player Character variables");
        _isDying = false;
        _startMoving = false;
        _startRotating = false;
        _victoryRun = false;
        _animator = gameObject.GetComponent<Animator>();
        _animator.enabled = true;
        _animator.ResetTrigger("Die");
        if (_animator.GetBool("Run")) {
            _animator.SetBool("Run", false);
        }
        if (_animator.GetBool("Victory")) {
            _animator.SetBool("Victory", false);
        }




    }

}