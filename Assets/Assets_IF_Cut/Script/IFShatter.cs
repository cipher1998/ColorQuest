using UnityEngine;
using System.Collections;

public class IFShatter : MonoBehaviour {
    [SerializeField] private Transform _parent;
    [SerializeField] private Material _insideMaterial;
    [SerializeField] private float _loadTime = 0.15f;
    [SerializeField] private int _cutLayer = 2;
    [SerializeField]
    private bool _initialized = false;

    private GameObject _currObstacle;

    private Vector3 _initialScale, _maxScale, _initialPos;


    private void OnEnable() {
        /*
        _initialPos = this.transform.localPosition;
        _initialScale = this.transform.localScale;
        _maxScale = new Vector3(4.55f, transform.localScale.y, 4.5f);
        this.GetComponent<Collider>().enabled = true;
        StartCoroutine(ScaleOverSeconds(this.gameObject, _maxScale, 0.5f, true));
         */
        if (_initialized) {
            this.transform.localPosition = _initialPos;
            this.transform.localScale = _initialScale;
        }

    }

    private void Start() {
        //_initialPos = this.transform.localPosition;
        _initialPos = new Vector3(0, 0.05f, 0);
        _initialScale = new Vector3(0.5f, 0.4f, 0.5f);
        _maxScale = new Vector3(4.5f, transform.localScale.y, 4.5f);
        _initialized = true;
    }


    public void StartCutting(GameObject _currObstacle) {
        this._currObstacle = _currObstacle;
        this.transform.localPosition = _initialPos;
        this.transform.localScale = _initialScale;
        _currObstacle.transform.SetParent(_parent);
        StartCoroutine(ScaleOverSeconds(this.gameObject, _maxScale, 0.5f, true));
    }



    public IEnumerator ScaleOverSeconds(GameObject objectToScale, Vector3 scaleTo, float seconds, bool _resetPos) {
        float elapsedTime = 0;
        Vector3 startingScale = objectToScale.transform.localScale;
        Debug.Log("Started Shattering GameObject");
        while (elapsedTime < seconds) {

            objectToScale.transform.localScale = Vector3.Lerp(startingScale, scaleTo, (elapsedTime / seconds));
            //objectToScale.transform.localPosition = Vector3.Lerp(objectToScale.transform.localPosition, _initialPos, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        if (_resetPos) {
            StartCoroutine(ScaleOverSeconds(this.gameObject, _initialScale, 0.5f, false));
        } else {
            this.transform.localPosition = _initialPos;
            StartCoroutine(DisableAfterSeconds(0.5f));
        }


    }

    public IEnumerator DisableAfterSeconds(float seconds) {
        ObstacleManager.Destroyed(_currObstacle);

        yield return new WaitForSeconds(seconds);
        foreach (Transform _child in _parent) {
            Destroy(_child.gameObject);
        }

        this.gameObject.SetActive(false);

    }


    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag.Contains("Obstacle")) {
            GameObject obstaclepart = other.contacts[0].otherCollider.gameObject;
            Debug.Log($"{this.gameObject.name} Collided with {other.gameObject.name} => {obstaclepart.name} ,  Tag : {obstaclepart.tag}");

            if (!_insideMaterial) {
                _insideMaterial = other.gameObject.GetComponent<Renderer>().material;
            }
            if (obstaclepart.gameObject.CompareTag("Obstacle_Black")) {
                StartCoroutine(Shatter_Object(obstaclepart, _cutLayer));
            }
        }

    }
    private IEnumerator Shatter_Object(GameObject _objectToCut, int _cutLayer) {
        yield return new WaitForSeconds(Random.Range(_loadTime, _loadTime * 2f));
        _objectToCut.layer = 14;
        GameObject[] _pieces = MeshManipulation.MeshCut.Cut(_objectToCut, _objectToCut.GetComponent<Collider>().bounds.center, Get_CutAngle(_objectToCut, _cutLayer), _insideMaterial);

        foreach (GameObject _piece in _pieces) {
            _piece.layer = 14;
            _piece.AddComponent<Rigidbody>().ResetCenterOfMass();
            _piece.AddComponent<MeshCollider>().convex = true;
            _piece.transform.SetParent(_parent);
            _piece.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-40f, 40f), 0, Random.Range(-40f, 40f)) * Random.Range(45f, 65f));
            if (_cutLayer > 0) {
                StartCoroutine(Shatter_Object(_piece, _cutLayer - 1)); ;
            }

        }

        Destroy(_objectToCut);

    }

    private Vector3 Get_CutAngle(GameObject _objectToCut, int _cutLayer) {

        Quaternion _rotEuler = Quaternion.Euler(Random.Range(-40f, 40f), Random.Range(-40f, 40f), Random.Range(-40f, 40f));
        Vector3[] _faces = { _objectToCut.transform.forward, _objectToCut.transform.right, _objectToCut.transform.up };
        return _rotEuler * _faces[_cutLayer % 3];
    }

}

