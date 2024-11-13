using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayNet : MonoBehaviour
{
    private bool initialized = false;
    public bool displayObject = true;
    [HideInInspector]public NeuralNet net;
    public PlayerMove pm;
    public bool playerDead = false;

    public float enemyDist;
    public float coinDist;
    public float enemyMove;
    public float enemyFromWall;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(displayObject == false && pm.humanMove == false)
        {
            if (initialized == true)
            {
                // 4 inputs: enemy dist, coin dist, enemymove, enemyfromwall

                
                // add fitness when doing good things

                //////////Debug.LogError("Fitness" + net.GetFitness());
                float[] inputs = new float[4];
                inputs[0] = enemyDist;
                inputs[1] = coinDist;
                inputs[2] = enemyMove;
                inputs[3] = enemyFromWall;

                float[] output = net.FeedFoward(inputs);
                // use output to do stuff
                // move player based on output[0]
                pm.Move(output[0]);
                // jump player based on output[1]
                if (output[1] > 0)
                {
                    // hard penalty, shouldn't be in but I want to speed up progress
                    if (inputs[0] > 1 || inputs[0] < -1 || inputs[1] > 1 || inputs[1] < -1)
                    {
                        // if far away, penalize jumping (distance of 15, or about half the stage)
                        net.AddFitness(-0.1f);
                    }
                    pm.Jump();
                }

            }
        }
    }

    public void Init(NeuralNet net)
    {
        this.net = net;
        initialized = true;
    }
}
