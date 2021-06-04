using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    [Header("Spawning Objects")]
    [SerializeField] Transform Spawnpoint;
     [SerializeField] GameObject[] Block;
     [SerializeField]public Material[] BlockMaterials;
     [SerializeField] int PrefabChose;
      [SerializeField] public int MaterialChose;
    [SerializeField] int ObjectCounts;
    [SerializeField] float delay;
    public List<GameObject> Blocks = new List<GameObject>();
   

    [Header("RotationPara")]

    [SerializeField] bool DynamicRotation;
    [SerializeField] bool DynamicSpeedRotation;
    [SerializeField] float currRotSpeed;
     [SerializeField] float _Minrotspeed;
      [SerializeField] float _Maxrotspeed;
     [SerializeField] float direction =1;

    [Header("BlockProperties")]
    [SerializeField] public int MaxnumCracks;
    private Quaternion  targetRotation;
    private GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        transform.position = Spawnpoint.transform.position;
        if(PrefabChose==-1) PrefabChose = UnityEngine.Random.Range(0 , Block.Length);
        if(MaterialChose==-1) MaterialChose = UnityEngine.Random.Range(0 , BlockMaterials.Length);
        
       
    }

    private void Update()
    {
        if(gameController.isGameover) { return ;}
        else if(gameController.Instantiatestatus)
        {
             DoRotation();
             CheckLevelFinished();
             return ;
        }
        else if(gameController.StopInstantiating)
        {

        }
        else if(gameController.StartInstantiating)
        {
            StartCoroutine(SpawnBlocks());
            GetRotationPreset();
            gameController.StopInstantiating= true;
           
        }
        else if(gameController.PlayerReached)
        {
           gameController.StartInstantiating = true;
        }
    }

    private void CheckLevelFinished()
    {
        if(Blocks.Count <= 0)
        {
            gameController.isLevelFinished = true;
        }
    }

    private void DoRotation()
    {
        if(gameController.Instantiatestatus)
        {
          
            RotateGameobjects();
        }
    }

    private void GetRotationPreset()
    {
        StartCoroutine(ChangeRotPreset());
    }

    IEnumerator ChangeRotPreset()
    {
        while(true)
        {
            if (DynamicRotation)
            {
                float r = UnityEngine.Random.Range(-1, 1);
                if (r == 0) { r = 1; }
                direction = r;
            }
            if (DynamicSpeedRotation)
            {
                currRotSpeed = UnityEngine.Random.Range(_Minrotspeed, _Maxrotspeed);
            }

            yield return new WaitForSeconds(UnityEngine.Random.Range(4, 8));
        }

    }

    private void RotateGameobjects()
    {
        
       transform.Rotate( 0f, Time.deltaTime * direction* currRotSpeed,0f);
    }

    IEnumerator SpawnBlocks()
   {
      
       for(int i= 0 ; i< ObjectCounts ; i++)
       {
            SpawnBlockAtpoint();
            yield return new WaitForSeconds(delay);
       }
       gameController.Instantiatestatus = true;
    
   }

    private void SpawnBlockAtpoint()
    {
        GameObject g =  Instantiate(Block[PrefabChose] , Spawnpoint);
        
        g.transform.parent = transform;
        Blocks.Add(g);
    }
}
