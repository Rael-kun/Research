using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // multi camera
    public bool multiCamera = false;
    public GameObject multiCanvas;
    public GameObject multiCameraObj;
    public Spawner spawner;


    private bool isTraining = false;
    public int populationSize = 50;
    public int currentAlive;
    private int generationNumber = 0;
    public int[] layers = new int[] { 4, 4, 4, 3 }; // 17 total and 3 output
    private List<NeuralNet> nets;
    [HideInInspector] public List<PlayNet> playList = null;
    public GameObject playNetPrefab;
    public float timeForGeneration;

    public List<RenderTexture> renderers;
    private int cameraCount = 9;

    public List<GameObject> cameras;
    public GameObject specialCanvas;
    public DisplayCrossover dc;
    
    void Timer()
    {
        isTraining = false;
    }

    private void Start()
    {
        if (multiCamera)
        {
            multiCanvas.SetActive(true);
            // out of view
            multiCameraObj.transform.position = new Vector3(multiCameraObj.transform.position.x, multiCameraObj.transform.position.y + 50, multiCameraObj.transform.position.z);
        }
        else
        {
            //multiCameraObj.GetComponent<Camera>().orthographicSize = 7;
            // shift view down
            multiCameraObj.transform.position = new Vector3(multiCameraObj.transform.position.x, multiCameraObj.transform.position.y + 7, multiCameraObj.transform.position.z);
            playNetPrefab.transform.localScale = new Vector3(3.1f, 3.1f, 3.1f);
            spawner.multiplier = 3.1f;
            multiCanvas.SetActive(false);
        }
    }

    void Update()
    {
        if(isTraining == false)
        {
            // If first generation, create nets for the first time
            if(generationNumber == 0)
            {
                InitPlayNeuralNetworks();
            }
            else // Just replace the nets
            {
                SortNets();
                for(int i = 0; i < populationSize / 5; i++)
                {
                    // reset low scoring nets
                    nets[i] = new NeuralNet(layers);
                    // mutate average scoring nets
                    nets[i + (populationSize / 5)].Mutate();
                    nets[i + (2*populationSize / 5)].Mutate();
                    nets[i + (3 * populationSize / 5)].Mutate();
                    // keep best scoring nets
                }

                for(int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);
                }
            }

            generationNumber++;
            Debug.LogError("Generation: " + generationNumber);
            isTraining = true;
            Invoke("Timer", timeForGeneration);
            CreatePlayers();

        }
        else
        {
            // wait until all are dead
            if(currentAlive <= 0)
            {
                isTraining = false;
            }
        }
    }

    private void CreatePlayers()
    {
        // reset camera list
        cameras = new List<GameObject>();
        //cameras.Add(playNetPrefab.gameObject.GetComponentInChildren<Camera>().gameObject);

        // kill old gamestates
        if(playList != null)
        {
            for(int i = 0; i < playList.Count; i++)
            {
                GameObject.Destroy(playList[i].gameObject);
            }
        }

        playList = new List<PlayNet>();
        cameraCount = 9;

        // create new gamestates
        for(int i = 0; i < populationSize; i++)
        {
            currentAlive += 1;
            // instantiate here, set script to balnet
            PlayNet thePlay = Instantiate(playNetPrefab).GetComponent<PlayNet>();
            thePlay.gameObject.transform.position = new Vector2(i * 1000, 0);
            thePlay.displayObject = false;
            thePlay.playerDead = false;

            if(cameraCount > 0)
            {
                thePlay.gameObject.GetComponentInChildren<Camera>().targetTexture = renderers[9 - cameraCount];
                cameraCount -= 1;

            }
            cameras.Add(thePlay.gameObject.GetComponentInChildren<Camera>().gameObject);

            // call neuralnet init here
            thePlay.Init(nets[i]);
            // add to list for destruction
            playList.Add(thePlay);
        }

        dc.genomes = playList;
    }

    void InitPlayNeuralNetworks()
    {
        // population must be a multiple of 5, setting to 20 if it isn't
        if(populationSize % 5 != 0)
        {
            Debug.LogError("Override: Requirement that you use a multiple of 5 for population size Setting pupulation to 20.");
            populationSize = 20;
        }

        nets = new List<NeuralNet>();

        for(int i = 0; i < populationSize; i++)
        {
            NeuralNet net = new NeuralNet(layers);
            net.Mutate();
            nets.Add(net);
        }
    }

    void SortNets()
    {
        // Using Bubble Sort!
        int i, j;
        NeuralNet temp;
        bool swapped;
        for (i = 0; i < nets.Count - 1; i++)
        {
            swapped = false;
            for (j = 0; j < nets.Count - i - 1; j++)
            {
                if (nets[j].GetFitness() > nets[j + 1].GetFitness())
                {
                    temp = nets[j];
                    nets[j] = nets[j + 1];
                    nets[j + 1] = temp;
                    swapped = true;
                }
            }
            if (swapped == false)
                break;
        }
    }

    public void HideCams(bool hide)
    {
        if(hide)
        {
            // turn off the other cameras
            foreach(GameObject camera in cameras)
            {
                camera.SetActive(false);
            }
            if(multiCamera == true)
            {
                specialCanvas.SetActive(false);
            }
        }
        else
        {
            // turn on the cameras
            foreach (GameObject camera in cameras)
            {
                camera.SetActive(true);
            }
            if (multiCamera == true)
            {
                specialCanvas.SetActive(true);
            }
        }
    }


}
