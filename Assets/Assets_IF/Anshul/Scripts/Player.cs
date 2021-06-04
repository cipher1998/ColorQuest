using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    
    [SerializeField] Transform WeaponPos;
    [SerializeField] Transform InitialStartpoint;
    [SerializeField] Transform FinalStartpoint;
    [SerializeField] LayerMask layerMask;

    
    private Animator animator;
    private bool IsDied= false;
    private GameController gameController;
    void Start()
    {
        animator =  GetComponent<Animator>();
        gameController = FindObjectOfType<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameController.isGameover) { return ;}
        if(GetDieStatus())
        {
            animator.SetTrigger("Die");
            FindObjectOfType<GameController>().isGameover = true;
            return ;
        }
        else if(gameController.isLevelFinished)
        {
            PlayerOnLevelFinished();
           
            return ;
        }
        else if(gameController.Instantiatestatus)
        {
            TakeInput();
            return; 
        }
        else if(gameController.PlayerReached)
        {
            
            return;
        }
        else  if(gameController.isGameStarted)
        {
           Movetopoint(transform.position , InitialStartpoint.position);
            return ;
        }
    }

    private void PlayerOnLevelFinished()
    {
         FinalStartpoint.position = new Vector3(FinalStartpoint.position.x, transform.position.y , FinalStartpoint.position.z);
        if(Movetopoint(transform.position , FinalStartpoint.position))
        {
            transform.rotation = Quaternion.Euler(0f,180f,0f);
            animator.SetBool("Victory", true);
            
        }
    }


    private bool Movetopoint( Vector3 initialpos , Vector3 finalpos)
    {
        if(Vector3.Distance(initialpos ,finalpos) < 0.5f)
        {
            gameController.PlayerReached = true;
            animator.SetBool("Run", false);

            return true ;
        }
         //transform.position = Vector3.Lerp(transform.position , InitialStartpoint.position , Time.deltaTime * 1);
         transform.position = Vector3.MoveTowards(initialpos , finalpos , Time.deltaTime*5);
         animator.SetBool("Run", true);
         return false;
    }

    public void SetDieStatus(bool status)
    {
        IsDied = status;
        
    }
    public bool GetDieStatus()
    {
        return IsDied;
    }
    private void TakeInput()
    {
       if(Input.GetMouseButtonDown(0))
       {   
           Shoot();
       }
    }

    private void Shoot()
    {
       Debug.DrawLine(WeaponPos.position , transform.forward * 100 , Color.red , 2f , false);
       RaycastHit hit;
      if( Physics.Raycast(WeaponPos.position , transform.forward , out hit , 20f , layerMask))
      {
            animator.SetTrigger("Melee Right Attack 01");
            hit.collider.gameObject.GetComponent<Pieces>().UpdateParentDetroyedParts();
      }
    
      
    }
}
