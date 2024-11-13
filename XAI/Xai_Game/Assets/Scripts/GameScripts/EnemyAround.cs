using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAround : MonoBehaviour
{
    public EnemyMove enemyMove;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(col.gameObject.tag == "Wall")
        {
            if(enemyMove.gameObject.transform.eulerAngles.z != 0)
            {
                enemyMove.gameObject.transform.eulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                enemyMove.gameObject.transform.eulerAngles = new Vector3(0, 0, 180f);
            }
            
            enemyMove.moveSpeed = -enemyMove.moveSpeed;
        }
    }
}
