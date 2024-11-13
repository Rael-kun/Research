using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpScript : MonoBehaviour
{
    private bool jumpMode = false;
    private string number;
    private bool doMove = false;

    public DisplayCrossover dc;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if(jumpMode == false)
            {
                number = "";
                doMove = true;
                jumpMode = true;
            }
            else
            {
                jumpMode = false;
            }
        }
        if (jumpMode)
        {
            // take input
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                number += "0";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                number += "1";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                number += "2";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                number += "3";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                number += "4";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                number += "5";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                number += "6";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                number += "7";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                number += "8";
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                number += "9";
            }
        }
        else
        {
            if (doMove)
            {
                // send to location after entering number
                int num = System.Int32.Parse(number);
                DoMove(num);
            }
        }
    }

    public void DoMove(int index)
    {
        gameObject.transform.position = new Vector3(index * 1000, 7, -10);
        dc.index = index;
        doMove = false;
    }

}
