
using System.Collections.Generic;
using UnityEngine;


/*
    Note: This normally displays crossover, but in this case it will just display a neural network
*/

public class DisplayCrossover : MonoBehaviour
{
    public DisplayNetwork displayNet;
    public Manager manager;
    public int index = 0;
    private bool crossover = false;
    public List<PlayNet> genomes;

    public JumpScript jumpScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V) && displayNet.displayingNetwork == false)
        {
            manager.HideCams(true);
            genomes = new List<PlayNet>();
            genomes = manager.playList;
            displayNet.DisplayGenome(genomes[index]);
            crossover = true;
        }

        if(crossover == true)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                manager.HideCams(false);
                crossover = false;
                displayNet.UnDisplayGenome();
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow) && index > 0)
            {
                index -= 1;
                displayNet.UnDisplayGenome();
                displayNet.DisplayGenome(genomes[index]);
                Debug.LogError("Current Neural Net: " + index);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && index < genomes.Count)
            {
                index += 1;
                displayNet.UnDisplayGenome();
                displayNet.DisplayGenome(genomes[index]);
                Debug.LogError("Current Neural Net: " + index);
            }
        }

        if (genomes[index].playerDead == true)
        {
            // jump to another screen
            if(index < (manager.playList.Count-1))
            {
                index += 1;
            }
            else
            {
                index = 0;
            }
            JumpIfDead(crossover);
        }

    }

    private void JumpIfDead(bool displaying)
    {
        // Jumps to an alive screen
        jumpScript.DoMove(index);
        if (displaying)
        {
            displayNet.UnDisplayGenome();
            displayNet.DisplayGenome(genomes[index]);
        }
    }
}
