using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InvisibleFiction;

public class PaintBall : MonoBehaviour {
    private float _ballSpeed = 5f;
    [SerializeField] private List<GameObject> _list_partcileChild;
    [SerializeField] private TrailRenderer _trail;
    private bool _fired = false, hitToObject = false;
    public float Speed { get { return _ballSpeed; } }

    private void Start() {
        GameManager.EVENT_LEVELFAILED += DestroyPaintBall;
        GameManager.EVENT_LEVELCLEARED += DestroyPaintBall;
    }


    private void OnDestroy() {
        GameManager.EVENT_LEVELFAILED -= DestroyPaintBall;
        GameManager.EVENT_LEVELCLEARED -= DestroyPaintBall;
    }



    public void FirePaintBall(Transform _parentFiredPaintBalls) {
        Debug.Log(this.gameObject.name + " Ball Fired !!!");
        this._fired = true;
        this.transform.SetParent(_parentFiredPaintBalls);

        //transform.LookAt(Obstacle.CurrentObstacleTransform.position);
        //this.GetComponent<Rigidbody>().AddForce(Obstacle.CurrentObstacleTransform.position - transform.position);
        //this.GetComponent<Rigidbody>().velocity = (Obstacle.CurrentObstacleTransform.position - transform.position) * Speed;

        transform.LookAt(LevelManager.GemStacks.position);
        this.GetComponent<Rigidbody>().AddForce(LevelManager.GemStacks.position - transform.position);
        this.GetComponent<Rigidbody>().velocity = (LevelManager.GemStacks.position - transform.position) * Speed;
    }

    private void OnCollisionEnter(Collision other) {
        //Debug.Log(this.gameObject.name + " Collided with " + other.gameObject.name + " ,  Tag : " + other.gameObject.tag);
        this.GetComponent<Collider>().enabled = false;
        if (!hitToObject) {
            hitToObject = true;

            if (other.gameObject.CompareTag("Obstacle")) {
                GameObject obstaclepart = other.contacts[0].otherCollider.gameObject;
                Debug.Log($"{this.gameObject.name} Collided with {other.gameObject.name} => {obstaclepart.name} ,  Tag : {obstaclepart.tag}");


                if (obstaclepart.gameObject.CompareTag("Obstacle_Black")) {
                    SoundManager.PlayAudio(SoundManager.Get(Sounds.obstacleHitWrong));
                    ColorMixerClass.LevelFailed();
                } else {
                    LevelManager.AddScore();
                    Obstacle.SetObstacle_Black(obstaclepart.GetComponent<ColorClass>());
                    //ColorMixerClass.Instance.MixNewColor(obstaclepart, this.GetComponent<ColorClass>().GetColorData());
                    SoundManager.PlayAudio(SoundManager.Get(Sounds.obstacleHit));
                    if (_list_partcileChild[0].gameObject != null) {
                        GameObject particleGameObject = _list_partcileChild[0].gameObject;
                        particleGameObject.transform.parent = obstaclepart.transform;
                        particleGameObject.SetActive(true);
                    }

                    Obstacle.ColorChanged(other.gameObject.GetComponent<Obstacle>());
                }

                StartCoroutine(DestroyInkBall());

            } else if (other.gameObject.CompareTag("Obstacle_Black")) {
                GameObject obstaclepart = other.contacts[0].otherCollider.gameObject;
                Debug.Log($"{this.gameObject.name} Collided with {other.gameObject.name} => {obstaclepart.name} ,  Tag : {obstaclepart.tag}");
                ColorMixerClass.Instance.MixNewColor(obstaclepart, this.GetComponent<ColorClass>().GetColorData());

                SoundManager.PlayAudio(SoundManager.Get(Sounds.obstacleHitWrong));
                Destroy(obstaclepart.gameObject, 1f);

                Obstacle.ColorChanged(other.gameObject.GetComponent<Obstacle>());
                StartCoroutine(DestroyInkBall());
            }

        } else {
            Debug.Log("Single PaintBall Collided with more than one Obstacle, neglacting Collision");
        }
    }

    private IEnumerator DestroyInkBall(float delayedTime = 0.0f) {
        if (transform.childCount > 0 && delayedTime != 0) {
            List<Transform> tList = new List<Transform>();

            foreach (Transform t in transform.GetChild(1).transform) {
                tList.Add(t);
            }

            while (transform.GetChild(1).localScale.x > 0) {
                yield return new WaitForSeconds(0.01f);
                transform.GetChild(1).localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                for (int i = 0; i < tList.Count; i++) {
                    tList[i].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                }
            }
        }
        yield return new WaitForSeconds(delayedTime);
        PaintBallManager.PaintBall_Destroyed(this.gameObject);
    }

    public void ChangeColor(ColorData paintBallNewColorData) {
        this.GetComponent<ColorClass>().SetColorData(paintBallNewColorData);
        foreach (GameObject particleGameObject in _list_partcileChild) {
            //   Particle in PaintBall
            var psMain = particleGameObject.GetComponent<ParticleSystem>().main;
            psMain.startColor = new ParticleSystem.MinMaxGradient(paintBallNewColorData.colorCode);

        }

        if (_trail != null) {
            _trail.startColor = paintBallNewColorData.colorCode;
            _trail.endColor = paintBallNewColorData.colorCode;
        }

    }

    public void DestroyPaintBall() {
        if (gameObject != null) {
            //Debug.Log($"Distroyed GameObject : {this.gameObject.name}");
            Destroy(gameObject);
        }
    }


}


