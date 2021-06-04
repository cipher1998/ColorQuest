using UnityEngine;
using System.Collections;


public class IFCut : MonoBehaviour {

    public Material _insideMaterial;


    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Obstacle") {
            Cut_Object(other.gameObject);
        }

    }


    private void Cut_Object(GameObject _objectToCut) {
        GameObject[] _pieces = MeshManipulation.MeshCut.Cut(_objectToCut, transform.position, transform.right, _insideMaterial);

        foreach (GameObject _piece in _pieces) {
            _piece.AddComponent<Rigidbody>().ResetCenterOfMass();
            _piece.AddComponent<MeshCollider>().convex = true;
        }

        Destroy(_objectToCut);


    }




}