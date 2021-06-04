using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Vector3 StartPos;
    [SerializeField] Vector3 StartRot;

    [SerializeField] Vector3 FinalPos;
    [SerializeField] Vector3 FinalRot;
    Quaternion startrot_;
    Quaternion finalrot_;
    private GameController gameController;


    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
         startrot_ = Quaternion.Euler(StartRot);
         finalrot_ = Quaternion.Euler(FinalRot);
    }
    // Update is called once per frame
    void Update()
    {
      if(gameController.stopcameramovement)
      {
          return ;
      }   
      else if(gameController.startcameramovement)
      {
         
        if( Vector3.Distance(transform.position , FinalPos) < 0.5f) { gameController.stopcameramovement  =true;}
        transform.position = Vector3.Slerp(transform.position , FinalPos , Time.deltaTime *2);
          
        transform.rotation = Quaternion.Lerp(transform.rotation , finalrot_ , Time.deltaTime *2);
        
      }
    }
}
