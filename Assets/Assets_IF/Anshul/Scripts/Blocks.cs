using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blocks : MonoBehaviour
{
    
    [SerializeField] private int BlockPieces;
    [SerializeField] LayerMask GroundMask;
    
    private int DestroyedPiecesCount=0;
    private GameController gameController;
    private BlockManager blockManager;
    
    void Start()
    {
        blockManager = FindObjectOfType<BlockManager>();
        gameController = FindObjectOfType<GameController>();
        GetBlockPieces();
    }

    private void GetBlockPieces()
    {
       BlockPieces = transform.childCount;
       foreach(Transform c in transform)
       {
           c.gameObject.AddComponent<Pieces>();
           c.GetComponent<MeshRenderer>().material = blockManager.BlockMaterials[blockManager.MaterialChose];
       }
    }

    // Update is called once per frame
    void Update()
    {
        if(gameController.isGameover) { return ;}
        CheckBlockDestroyed();
    }

    private void CheckBlockDestroyed()
    {
       if(DestroyedPiecesCount == BlockPieces)
       {
           blockManager.Blocks.Remove(gameObject);
           Destroy(gameObject);
       }
    }
    public void IncreaseDestroyedParts()
    {
        DestroyedPiecesCount++;
    }

    private void OnTriggerEnter(Collider other)
    {
         if(((1<<other.gameObject.layer) & GroundMask) != 0)
         {
            DoRandomCrack();
         }
       
    }

    private void DoRandomCrack()
    {
        blockManager.MaxnumCracks = Mathf.Clamp(blockManager.MaxnumCracks , 0 , BlockPieces-1);
        for(int i=0 ; i< blockManager.MaxnumCracks ; i++)
        {
            int r = UnityEngine.Random.Range(0, transform.childCount);
           if(transform.GetChild(r).GetComponent<Pieces>().ReturnDestroyedStatus())
           {
               i--;
           }
           else
           {
               transform.GetChild(r).GetComponent<Pieces>().UpdateParentDetroyedParts();
           }
        }
        
    }
}

