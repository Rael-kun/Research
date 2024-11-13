using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class DisplayNode
{
    // note, although nodes are not sorted based on id in nodeGenes, they are sorted based on id for this (id is just index in the list)
    public List<int> inputsForNode;
    public int id;
    public float size;
    public GameObject nodeObj;

    public DisplayNode(int ident)
    {
        inputsForNode = new List<int>();
        id = ident;
        nodeObj = null;
        size = 1f;
    }

}

public class DisplayWeight
{
    public float weight;
    public GameObject weightObj;
    public Image weightImage;

    public DisplayWeight(float weightIn)
    {
        weight = weightIn;
        weightObj = null;
        weightImage = null;
    }
}

public class DisplayNetwork : MonoBehaviour
{
    public Manager manager;
    public GameObject canvas;
    public GameObject nodeList;
    public GameObject conList;
    public GameObject node;
    public GameObject connection;
    [HideInInspector] public bool displayingNetwork = false;
    private float savedTimeScale;
    private PlayNet netToShow;

    private int theNet = 0;
    private int select = -1;

    List<List<DisplayNode>> displayList = new List<List<DisplayNode>>();
    List<List<List<DisplayWeight>>> weightList = new List<List<List<DisplayWeight>>>();
    List<GameObject> displayObjects = new List<GameObject>();
    private List<GameObject> displayCons = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        canvas.transform.localScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (displayingNetwork == true)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if(select == -1)
                {
                    select = 0;
                }
                else if(select == 0)
                {
                    select = 1;
                }
                else
                {
                    select = -1;
                }
            }
            UpdateEverything(netToShow);
        }
        // Update is different then Fixed_Update, because update still runs when timescale is 0
        if(Time.timeScale == 0 && Input.GetKeyDown(KeyCode.Z) && displayingNetwork == true)
        {
            UnDisplayGenome();
        }
        if(Time.timeScale == 0 && Input.GetKeyDown(KeyCode.C) && displayingNetwork == true)
        {
            //  display connections in debug.logerror
            ClearLog();
            Debug.Log("Start Displaying Network " + theNet);
            // Note: Weights are a 3d array, with dimensions (1:Layers  2:Node looking at
            // 3: All nodes connecting to *2* in the previous layer)
            // Not implemented yet
            Debug.Log("Finish Displaying Network " + theNet);
        }
    }


    // Quick code to clear logs
    public void ClearLog()
    {
        /*
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
        */
    }

    public void DisplayGenome(PlayNet net)
    {
        if(displayingNetwork == false)
        {
            displayingNetwork = true;
            theNet += 1;
            netToShow = net;
            // Now build network
            BuildNetwork(net);
            // Now display canvas
            canvas.transform.localScale = new Vector3(1, 1, 1);
            // At the end of this, after displaying the genome, freeze time
            savedTimeScale = Time.timeScale;
            //Time.timeScale = 0;
        }
        
    }

    public void UnDisplayGenome()
    {
        // first, hide the canvas
        canvas.transform.localScale = new Vector3(0, 0, 0);
        // then reset arrays
        ResetDisplay();
        // then reset time
        if (savedTimeScale == 0)
        {
            //Time.timeScale = 1;
        }
        else
        {
            //Time.timeScale = savedTimeScale;
        }
        displayingNetwork = false;
    }

    private void ResetDisplay()
    {
        for(int i = 0; i < displayList.Count; i++)
        {
            for(int j = 0; j < displayList[i].Count; j++)
            {
                Destroy(displayList[i][j].nodeObj);
            }
        }
        for(int i = 0; i < displayCons.Count; i++)
        {
            Destroy(displayCons[i]);
        }
        displayList = new List<List<DisplayNode>>();
        weightList = new List<List<List<DisplayWeight>>>();
        displayCons = new List<GameObject>();
        displayObjects = new List<GameObject>();
    }
    private void BuildNetwork(PlayNet net)
    {
        // first figure out nodes and layers
        BuildNodes(net);
        // then figure out connections
        BuildConnections(net);
        // then do innovation display
    }



    private void BuildNodes(PlayNet net)
    {
        int id = 0;
        DisplayNode nodeToAdd = null;

        // Cycle through each layer
        for (int i = 0; i < manager.layers.Length; i++)
        {
            // Make a new list of nodes
            List<DisplayNode> listNodes = new List<DisplayNode>();
            for (int j = 0; j < manager.layers[i]; j++)
            {
                // add node
                nodeToAdd = new DisplayNode(id);
                id += 1;
                // then, add connections to node
                if (i > 0)
                {
                    // for each node in previous layer
                    for (int k = 0; k < displayList[i-1].Count; k++)
                    {
                        nodeToAdd.inputsForNode.Add(displayList[i - 1][k].id);
                    }
                }
                // finally, add to list
                listNodes.Add(nodeToAdd);
            }
            // Add layer to list
            displayList.Add(listNodes);
        }

        // Now, spawn nodes!
        SpawnNodes();

    }


    private void SpawnNodes()
    {
        // take off 10% of available screen
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        float widthStart = canvasWidth * 0.05f; // start on left side
        float widthAvailable = canvasWidth * 0.9f; // total space after 10% taken off
        float heightStart = canvasHeight;//canvasHeight * 0.95f;// start at top and move down
        float heightAvailable = canvasHeight;// * 0.9f;
        // actual width and height to set postion
        float width; 
        float height;

        // adjusted amount to add after each new node or layer
        float adjustedHeight;
        float adjustedWidth = (widthAvailable) / (displayList.Count - 1); // 0 starts on left, layerList.Count starts on right

        for(int i = 0; i < displayList.Count; i++)
        {
            // cycle through all layers, starting at 10% width and adding adjusted width
            width = widthStart + (i * adjustedWidth);
            if(displayList[i].Count > 1) // avoids divide by zero error
            {
                adjustedHeight = (heightAvailable) / (displayList[i].Count + 1);
            }
            else
            {
                // puts in center screen
                adjustedHeight = canvasHeight / 2;//0;
            }

            for (int j = 0; j < displayList[i].Count; j++)
            {
                // cycle through all nodes, starting at 10% height and adding adjusted height
                height = heightStart - ((j+1) * adjustedHeight);

                GameObject newNode = Instantiate(node, nodeList.transform);
                displayList[i][j].nodeObj = newNode;
                displayObjects.Add(newNode);
                newNode.transform.localPosition = new Vector3(width - (canvasWidth / 2), height - (canvasHeight / 2), 0);
                // display id
                newNode.GetComponentInChildren<Text>().text = displayList[i][j].id.ToString();
            }

        }
    }

    private void BuildConnections(PlayNet net)
    {
        DisplayWeight weightToAdd = null;

        GameObject inputNode;
        GameObject outputNode;

        // Starting at the first layer:
        for(int i = 1; i < displayList.Count; i++)
        {

            List<List<DisplayWeight>> layerWeights = new List<List<DisplayWeight>>();
            // For each node in the layer:
            for (int j = 0; j < displayList[i].Count; j++)
            {

                List<DisplayWeight> listWeights = new List<DisplayWeight>();
                // Add connection for each input node in list
                for (int k = 0; k < displayList[i][j].inputsForNode.Count; k++)
                {
                    // Get id of input node and output node
                    int inputId = displayList[i][j].inputsForNode[k];
                    int outputId = displayList[i][j].id;
                    // Note: They are created in order, so it is fine to grab using tempId as an index.
                    // If they are not created in order, displayObjects will not have the correct gameobject
                    inputNode = displayObjects[inputId];
                    outputNode = displayObjects[outputId];

                    // Get weight; remember that weights use a 3 dimensional array where the first
                    // dimension is the layers of weights, the second dimension is outputNode of the 
                    // weight, and the third dimension is the input node of the weight
                    // Note: This line only works since we are using a fully connected network,
                    // otherwise k would be incorrect
                    float weight = net.net.weights[i - 1][j][k];

                    weightToAdd = new DisplayWeight(weight);
                    weightToAdd.weightObj = SpawnConnections(inputNode, outputNode, weight);
                    weightToAdd.weightImage = weightToAdd.weightObj.GetComponent<Image>();

                    listWeights.Add(weightToAdd);
                }

                layerWeights.Add(listWeights);
            }

            weightList.Add(layerWeights);
        }
    }
    private GameObject SpawnConnections(GameObject inputNode, GameObject outputNode, float weight)
    {
        // before anything, spawn connection and add it to proper list
        GameObject theConnection = Instantiate(connection, conList.transform);
        displayCons.Add(theConnection);

        // first, find center
        Vector2 lineBetween = new Vector2(outputNode.transform.localPosition.x - inputNode.transform.localPosition.x, outputNode.transform.localPosition.y - inputNode.transform.localPosition.y);
        Vector2 center = new Vector2(inputNode.transform.localPosition.x + (lineBetween.x / 2), inputNode.transform.localPosition.y + (lineBetween.y / 2));
        theConnection.transform.localPosition = center;

        // second, find rotation
        float angle = Mathf.Atan2(lineBetween.y, lineBetween.x) * Mathf.Rad2Deg;
        theConnection.transform.localEulerAngles = new Vector3(0, 0, angle);

        // third, find length
        float length = Mathf.Sqrt((lineBetween.x * lineBetween.x) + (lineBetween.y * lineBetween.y));
        theConnection.transform.localScale = new Vector3(length, 1, 1);

        // fourth, color based on weight
        // 1 is red, 0 is grey, -1 is blue
        weight = (weight + 1) / 2; // weight is now between 0 and 1
        float red = weight * 255;
        float green;
        float blue = 255 - red;
        if (blue > red)
        {
            green = red;
        }
        else
        {
            green = blue;
        }
        theConnection.GetComponent<Image>().color = new Color32((byte)red, (byte)green, (byte)blue, 255);

        return theConnection;
    }

    public void UpdateEverything(PlayNet net)
    {
        net.net.FeedScaleNodes(select); // equal importance
        UpdateNodes(net);
        //UpdateWeights(net);
    }

    private void UpdateWeights(PlayNet net)
    {
        // for each layer
        for (int i = 1; i < displayList.Count; i++)
        {
            // for each node in current layer
            for (int j = 0; j < displayList[i].Count; j++)
            {
                // for each node in previous layer
                for (int k = 0; k < displayList[i][j].inputsForNode.Count; k++)
                {
                    float weight = net.net.weights[i - 1][j][k];

                    weight = (weight + 1) / 2; // weight is now between 0 and 1
                    float red = weight * 255;
                    float green;
                    float blue = 255 - red;
                    if (blue > red)
                    {
                        green = red;
                    }
                    else
                    {
                        green = blue;
                    }

                    weightList[i - 1][j][k].weightObj.GetComponent<Image>().color = new Color32((byte)red, (byte)green, (byte)blue, 255);
                }
            }

        }
    }

    private void UpdateNodes(PlayNet net)
    {
        float size = 0f;
        int id = 0;
        for (int i = 0; i < manager.layers.Length; i++)
        {
            for (int j = 0; j < manager.layers[i]; j++)
            {
                size = net.net.neuronImportance[i][j];
                Debug.Log(size + " " + id);
                id += 1;
                if (size < 0)
                {
                    // negative importance, size is between 0.5 and 1
                    size = 1 + (size * 0.5f);
                    //size = 1 - (-size * 3 / 4);
                }
                else
                {
                    // positive importance, size is between 1 and 2
                    //size = 1 + (size * 3);
                    size = 1 + size;
                }

                displayList[i][j].size = size;
                displayList[i][j].nodeObj.transform.localScale = new Vector3(size, size, size);
            }
        }
    }
}
