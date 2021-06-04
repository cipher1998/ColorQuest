using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    private Blocks Parent;
    private bool IsDestroyed = false;
    void Start()
    {
        Parent = transform.parent.GetComponent<Blocks>();
       
    }

    public bool ReturnDestroyedStatus()
    {
        return IsDestroyed;
    }
    public void UpdateParentDetroyedParts()
    {
       
        if(IsDestroyed)
        {
            FindObjectOfType<Player>().SetDieStatus(true);
            return;
        }
        Parent.IncreaseDestroyedParts();
        GetComponent<MeshRenderer>().material.color -= new Color(0.5f,0.5f,0.5f,1f);
        IsDestroyed = true;
           
    }

   
}
