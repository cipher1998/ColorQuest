using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public  bool isGameover= false;
    public bool isGameStarted= true;
    public bool isLevelFinished= false;
    public bool PlayerReached = false;
    public bool StartInstantiating = false;
    public bool StopInstantiating = false;
    public bool Instantiatestatus = false;

    public bool startcameramovement= true;
    public bool stopcameramovement = false;

    [SerializeField] GameObject CelebrationLeftparticle;
    [SerializeField] GameObject CelebrationRightparticle;
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if(isLevelFinished)
        {
            CelebrationLeftparticle.SetActive(true);
            CelebrationRightparticle.SetActive(true);
        }
    }
}
