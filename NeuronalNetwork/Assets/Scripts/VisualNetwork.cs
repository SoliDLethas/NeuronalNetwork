using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NNTest;
public class VisualNetwork : MonoBehaviour
{
    public GameObject gO_Neuron;
    public int SpaceBetweenVisNeurons = 5;
    public Vector3 Start_Position = new Vector3(0, 10, 10);

    public NNet nw;
    private List<GameObject> vis_neurons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetVisualNetwork()
    {
        DeleteVisNeurons();
        vis_neurons = new List<GameObject>();
    }

    public void SpawnVisualNetwork()
    {
        foreach (InputNeuron inputNeuron in nw.nn.inputNeurons)
        {
            //Neuronenkugeln spawnen
            Vector3 spawnPosition = Start_Position + new Vector3(inputNeuron.x * SpaceBetweenVisNeurons, inputNeuron.y * SpaceBetweenVisNeurons, 0);
            GameObject vis_Neuron = Instantiate(gO_Neuron, spawnPosition, Quaternion.identity);
            vis_Neuron.GetComponent<VisualNeuron>().InputNeuron = inputNeuron;
            vis_Neuron.GetComponent<VisualNeuron>().neuronType = NeuronType.InputNeuron;
            vis_Neuron.GetComponent<VisualNeuron>().vn = this;
            vis_neurons.Add(vis_Neuron);
        }

        foreach (WorkingNeuron workingNeuron in nw.nn.getWorkingNeurons())
        {
            //Neuronenkugeln spawnen
            Vector3 spawnPosition = Start_Position + new Vector3(workingNeuron.x * SpaceBetweenVisNeurons, workingNeuron.y * SpaceBetweenVisNeurons, 0);
            GameObject vis_Neuron = Instantiate(gO_Neuron, spawnPosition, Quaternion.identity);
            vis_Neuron.GetComponent<VisualNeuron>().workingNeuron = workingNeuron;
            vis_Neuron.GetComponent<VisualNeuron>().neuronType = NeuronType.WorkingNeuron;
            vis_Neuron.GetComponent<VisualNeuron>().vn = this;
            vis_neurons.Add(vis_Neuron);
        }
    }

    private void DeleteVisNeurons()
    {
        foreach (GameObject vis_Neuron in vis_neurons)
        {
            vis_Neuron.GetComponent<VisualNeuron>().DeleteLineRenderer();
            GameObject.Destroy(vis_Neuron);
        }
    }
}
