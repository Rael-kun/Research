using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float moveSpeed = 15;
    public float jumpSpeed = 25;
    public DetectBelow db;
    private Rigidbody2D rb2d;
    public bool humanMove;

    public GameObject theEnemy;
    public GameObject theCoin;
    public PlayNet myPlayNet;

    public Manager manager;
    private int jumpCounter = 100;
    private float jumpTimer = 10f;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
        else
        {
            jumpCounter = 100;
            jumpTimer = 10;
        }

        // update playnet
        if(theEnemy != null)
        {
            myPlayNet.enemyDist = (gameObject.transform.position.x - theEnemy.transform.position.x) / 30; // normalize
            myPlayNet.coinDist = (gameObject.transform.position.x - theCoin.transform.position.x) / 30; // normalize
            if (theEnemy.GetComponent<EnemyMove>().moveSpeed > 0)
            {
                // moving left
                myPlayNet.enemyMove = -1;
            }
            else
            {
                // moving right
                myPlayNet.enemyMove = 1;
            }
            //Debug.LogError("yeah" + theEnemy.GetComponent<EnemyMove>().distFromWall);
            myPlayNet.enemyFromWall = theEnemy.GetComponent<EnemyMove>().distFromWall;
        }
        

        // move?

        float hori = Input.GetAxis("Horizontal");
        if(humanMove)
        {
            Move(hori);
        }
        

        if (Input.GetKey(KeyCode.UpArrow) && humanMove)
        {
            Jump();
        }

    }

    public void Move(float input)
    {
        if(rb2d != null)
        {
            rb2d.velocity = new Vector2(moveSpeed * input, rb2d.velocity.y);
        }
    }
    public void Jump()
    {
        if(db != null && db.isGrounded) 
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            jumpCounter -= 1;
            if(jumpCounter <= 0)
            {
                // if used 5 jumps in the past 10 seconds, penalty
                //myPlayNet.net.AddFitness(-10f);
                //manager.currentAlive -= 1;
                //myPlayNet.playerDead = true;
                //Destroy(gameObject);
            }
        }
    }
}
