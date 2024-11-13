using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCol : MonoBehaviour
{
    // Start is called before the first frame update
    public Spawner spawner;
    public PlayNet playNet;
    public Manager manager;

    public float deathScore = -10f;
    public float coinScore = 10f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            // Give bad score
            playNet.net.AddFitness(deathScore);
            playNet.playerDead = true;
            manager.currentAlive -= 1;
            // Kill player
            Destroy(gameObject.GetComponentInParent<PlayerMove>().gameObject);
        }
        if(col.gameObject.tag == "Coin")
        {
            // Give good score
            playNet.net.AddFitness(coinScore);
            // Create new coin
            spawner.Spawn("Coin");

            // Destroy Old Coin
            Destroy(col.gameObject);
        }
    }
}
