using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBelow : MonoBehaviour
{
    public bool isGrounded = true;
    public Spawner spawner;
    public PlayNet playNet;
    public float killEnemyScore = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Ground")
        {
            // hit ground
            isGrounded = true;

        }
        else if(col.gameObject.tag == "Enemy")
        {
            // hit enemy
            // update score
            playNet.net.AddFitness(killEnemyScore);
            // create new enemy
            spawner.Spawn("Enemy");
            // kill enemy
            Destroy(col.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.tag == "Ground")
        {
            isGrounded = false;
        }
    }
}
