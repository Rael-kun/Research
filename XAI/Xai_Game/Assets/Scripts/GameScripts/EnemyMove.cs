using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float moveSpeed = 5;
    public float distFromWall = 0;
    public float rayDistance = 4000f;
    public LayerMask targetLayer;
    private Vector2 theVec = Vector2.left;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.velocity = new Vector2(-moveSpeed, rb2d.velocity.y); // move toward player  first
        if(gameObject.transform.eulerAngles.z == 0)
        {
            theVec = Vector2.left;
        }
        else
        {
            theVec = Vector2.right;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, theVec, Mathf.Infinity, targetLayer);

        if (hit.collider != null)
        {
            distFromWall = hit.distance / 60; // somewhat normalize it
            Debug.DrawRay(transform.position, theVec * hit.distance, Color.green);
        }

    }
}
