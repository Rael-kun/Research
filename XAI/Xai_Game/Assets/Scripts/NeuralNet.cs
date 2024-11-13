using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Unity.VisualScripting.Dependencies.NCalc;

public class NeuralNet : IComparable<NeuralNet>
{
    private int[] layers;
    private float[][] neurons;
    public float[][][] weights;
    private float fitness;

    //scale nodes added
    private float[][][] weightImportance;
    public float[][] neuronImportance;

    public NeuralNet(int[] layers)
    {
        this.layers = new int[layers.Length];
        for(int i = 0; i< layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeights();
        InitScaleNodes();
    }

    public NeuralNet(NeuralNet copyNetwork)
    {
        this.layers = new int[copyNetwork.layers.Length];
        for (int i = 0; i < copyNetwork.layers.Length; i++)
        {
            this.layers[i] = copyNetwork.layers[i];
        }

        InitNeurons();
        InitWeights();
        CopyWeights(copyNetwork.weights);
        InitScaleNodes();
    }

    private void CopyWeights(float[][][] copyWeights)
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }
    }

    private void InitNeurons()
    {
        // Neuron Initialization
        List<float[]> neuronsList = new List<float[]>();

        for(int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]); // add each layer to neuron list
        }

        neurons = neuronsList.ToArray();
    }

    private void InitWeights()
    {
        // Weight Initialization
        List<float[][]> weightsList = new List<float[][]>();

        for (int i = 1; i < layers.Length; i++)
        {
            // go through each layer
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer]; // neuron weights

                // set weights randomly between -0.5 and 0.5
                for(int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    // give random weights
                    neuronWeights[k] = (float)UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray();
    }

    public float[] FeedFoward(float[] inputs)
    {
        for(int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        for(int i = 1; i < layers.Length; i++)
        {
            for(int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;
                for(int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                // Tanh function just used to get value between -1 and 1
                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[neurons.Length - 1];
    }

    public void Mutate()
    {
        for(int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];

                    // mutate weight step
                    float randomNumber = (float)UnityEngine.Random.Range(0, 1000);

                    if(randomNumber <= 2f)
                    {
                        weight *= -1;
                    }
                    else if(randomNumber <= 4f)
                    {
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                    else if(randomNumber <= 6f)
                    {
                        weight *= (UnityEngine.Random.Range(0f, 1f) + 1);
                    }
                    else if(randomNumber <= 8f)
                    {
                        weight *= UnityEngine.Random.Range(0f, 1f);
                    }

                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float fit)
    {
        fitness += fit;
    }
    public void SetFitness(float fit)
    {
        fitness = fit;
    }
    public float GetFitness()
    {
        return fitness;
    }

    public int CompareTo(NeuralNet other)
    {
        if(other == null)
        {
            return 1;
        }
        if (fitness > other.fitness)
        {
            return 1;
        }
        else if(fitness < other.fitness)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public void InitScaleNodes()
    {
        // Weight Initialization
        List<float[][]> scaleList = new List<float[][]>();

        for (int i = 1; i < layers.Length; i++)
        {
            // go through each layer
            List<float[]> layerScaleList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronScaleWeights = new float[neuronsInPreviousLayer]; // neuron weights

                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    // give random weights
                    neuronScaleWeights[k] = 0.0f;
                }

                layerScaleList.Add(neuronScaleWeights);
            }

            scaleList.Add(layerScaleList.ToArray());
        }

        weightImportance = scaleList.ToArray();


        // Neuron Initialization
        List<float[]> neuronsList = new List<float[]>();

        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]); // add each layer to neuron list
        }

        neuronImportance = neuronsList.ToArray();
    }

    public void FeedScaleNodes(int outputViewing)
    {
        for(int i = 0; i < neuronImportance[layers.Length - 1].Length; i++)
        {
            // give the outputViewing an importance of 1, the rest 0
            if(i == outputViewing)
            {
                neuronImportance[layers.Length - 1][i] = 1;
            }
            else
            {
                neuronImportance[layers.Length - 1][i] = 0;
            }

            // if outputViewing = -1, give each an equal score of importance
            if(outputViewing == -1)
            {
                neuronImportance[layers.Length - 1][i] = (1.0f / (float)neuronImportance[layers.Length - 1].Length);
            }
        }

        // explanation of what's going on (showing formulas):
        // Inputs: A = [A1, A2, A3, ..., An]
        // Output: B = Sum(A)
        // A_abs = Sum of absolute value of all A's
        // If B >= 0:
        //      Importance = Ai / A_abs
        // else:
        //      Importance = Ai / -A_abs
        //
        // Vector of Node Importance (Gained from last time) = [x1, x2, x3, ..., xn] <-- Importances from output layer
        // 
        // Importance_TopNode *= x1, Importance_NextNode *= x2, ... Importance_BottomNode *= xn
        //  for i in range Inputs:
        //      Input[i] = Importance_TopNode[i] + Importance_NextNode[i] + ... + Importance_BottomNode[i]
        //
        //  Magnitude = Sum of absolute value of all Inputs
        //
        //  for i in range Inputs:
        //      Input[i] /= Magnitude


        // Let's say a nodes has weights 0.9, -0.9, 0.2, -0.7, and -0.3 going in, to sum up to -0.8.
        // Since the output is negative (thus, a negative action is happening as a result) we should say that negative
        // weights are important to the decision, and positive weights are holding back the decision. Since the nodes are of size 1,
        // I want to scale them up if they are important, and down if they are stopping the decision. Assuming the minimum size is
        // 0.25 and the max size is 4, the algorithm is as follows:

        // Separate the negatives and positives
        // Find value totals of both
        // Scale negatives and positives by their value totals
        // The values that are supporting the decision have the equation of: size = 1 + (scaled_value * 3)
        // The values against the decision have the equation of: size = 1 - (scaled_value * 3)/4

        // For the above example, we would get:
        // Positives: [0.9, 0.2] Negatives:[-0.9, -0.7, -0.3]
        // Value totals: Positive = 1.1, Negative = -1.9
        // Scaled_Positives: [0.82, 0.18] Scaled_Negatives: [0.47, 0.37, 0.16]
        // Supporting values: [2.41, 2.11, 1.47]
        // Non-suporting values: [0.39, 0.87]



        for (int i = layers.Length - 2; i >= 0; i--) // start at second to last layer
        {
            for (int j = 0; j < neurons[i + 1].Length; j++) // cycle through each node in the next layer
            {
                float absSum = 0f;
                for (int k = 0; k < neurons[i].Length; k++) // cycle through the nodes in the current layer
                {
                    // find initial importance
                    //weightImportance[i][j][k] is a connection between the current layer, with weight from node in next layer to current node
                    weightImportance[i][j][k] = weights[i][j][k] * neurons[i][k]; // grabs a vector of all values pertaining to the next node
                    absSum += Mathf.Abs(weightImportance[i][j][k]); // is absolute sum discussed above
                    neuronImportance[i][k] = 0; // reset neuron importance in the current layer
                }
                // now that absSum is found:
                for (int k = 0; k < neurons[i].Length; k++) // cycle once more through all nodes in the current layer
                {
                    // this bit repleaces weight importance with scaled importance: it divides by absSum to make it normalized,
                    // then it multiplies by neuron importance to get the actual values
                    weightImportance[i][j][k] = (weightImportance[i][j][k] / absSum) * neuronImportance[i + 1][j];

                }

            }
            // once you have cycled through all of the nodes in the next layer, you can find magnitude then re-normalize
            // Note that weightImportance[i][j] is the importance vector for the j-th node

            // Also NOTE: The below loops are reversed order from the previous loops; this is because we are applying importance
            // for each node k in the current layer
            float magnitude = 0f;

            for (int k = 0; k < neurons[i].Length; k++) // cycle through each node in the CURRENT layer
            {
                for (int j = 0; j < neurons[i + 1].Length; j++) // cycle through the nodes in the NEXT layer
                {
                    neuronImportance[i][k] += weightImportance[i][j][k];

                }

                magnitude += Mathf.Abs(neuronImportance[i][k]);
                
            }

            for (int k = 0; k < neurons[i].Length; k++) // cycle through each node in the CURRENT layer
            {
                // normalize by magnitude
                neuronImportance[i][k] = neuronImportance[i][k] / magnitude;
            }

        }
        //ScaleNodesHere();

    }

    private void ScaleNodesHere()
    {
        // uses neruon importance to get a scaling for a node ( not used yet )

        // equations: 1 + (scaled_value * 3) if positive, 1 - (scaled_value * 3)/4 if negative 
        for (int layer = 0; layer < neuronImportance.Length; layer++)
        {
            for (int node = 0; node < neuronImportance[layer].Length; node++)
            {
                float size = 0;
                if(neuronImportance[layer][node] < 0)
                {
                    // negative importance, size is between 0.25 and 1
                    size = 1 - (neuronImportance[layer][node] * 3 / 4);
                }
                else
                {
                    // positive importance, size is between 1 and 4
                    size = 1 + (neuronImportance[layer][node] * 3);
                }


                
            }
        }
    }


}
