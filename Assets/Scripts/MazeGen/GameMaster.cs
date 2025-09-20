using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    ClusterMazeGen generator;
    [SerializeField] int mazeWidth = 7; 
    [SerializeField] int mazeHeight = 7;
    [SerializeField] int seed = 0;
    [SerializeField] bool useRandomSeed = true;


    // Start is called before the first frame update
    void Start()
    {
        if (useRandomSeed)
        {
            seed = Random.Range(0, 10000);
            Debug.Log("Using random seed: " + seed);
        }
        else
        {
            Debug.Log("Using fixed seed: " + seed);
        }
        Random.InitState(seed);
        generator = new ClusterMazeGen();
        generator.InitMaze(mazeWidth, mazeHeight);

    }


}
