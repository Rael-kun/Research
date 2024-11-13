using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject leftPos;
    public GameObject rightPos;
    public GameObject enemy;
    public GameObject coin;
    public GameObject player;
    public PlayNet playNet;
    public PlayerMove playerMove;

    public float multiplier = 1;
    private float threshold = 3;
    private float leftBound;
    private float rightBound;
    private float inbetween;
    private Vector3 spawnPos;
    // Start is called before the first frame update
    void Start()
    {
        leftBound = leftPos.transform.position.x + 2;
        rightBound = rightPos.transform.position.x - 2;
        inbetween = rightBound - leftBound;

        if(playNet.displayObject == false)
        {
            Spawn("Enemy");
            Spawn("Coin");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Spawn(string type)
    {
        threshold = 3 * multiplier;
        float dist = 0;
        while(dist < threshold && dist > -threshold)
        {
            // Calculate position
            float rand = Random.Range(0f, 1f);
            if(type == "Coin")
            {
                spawnPos = new Vector3(leftPos.transform.position.x + (rand * inbetween) + 2, leftPos.transform.position.y + (4*multiplier), leftPos.transform.position.z);
            }
            else
            {
                spawnPos = new Vector3(leftPos.transform.position.x + (rand * inbetween) + 2, leftPos.transform.position.y, leftPos.transform.position.z);
            }
            if(player != null)
            {
                dist = player.transform.position.x - spawnPos.x;

            }
        }

        // Spawn proper object
        if (type == "Enemy")
        {
            GameObject theEnemy = Instantiate(enemy, gameObject.transform);
            theEnemy.transform.position = spawnPos;
            if(playerMove != null)
            {
                playerMove.theEnemy = theEnemy;
            }
        }
        else if(type == "Coin")
        {
            GameObject theCoin = Instantiate(coin, gameObject.transform);
            theCoin.transform.position = spawnPos;
            if(playerMove != null)
            {
                playerMove.theCoin = theCoin;
            }
        }
                
    }


}
