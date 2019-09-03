using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NNDynamic;
public class VisualNetworkDynamic : MonoBehaviour
{
    public GameObject prefabNeuron;
    public Vector3 Start_Position = new Vector3(25, 25, 50);

    //Vector3 spaceMin = new Vector3(0, 0, 0);
    //Vector3 spaceMax = new Vector3(60, 60, 60);

    GameObject creature;

    List<GameObject> vis_neurons = new List<GameObject>();

    List<InputNeuron> inputNeurons = new List<InputNeuron>();
    List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
    List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();

    float lastTimeDrawing;
    readonly float durationUntilDrawing = 1f;

    private GlobalData globalData;

    // Start is called before the first frame update
    void Start()
    {
        globalData = GameObject.Find("GameManager").GetComponent<GlobalData>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Time.realtimeSinceStartup - lastTimeDrawing) > durationUntilDrawing)
        {
            try
            {
                creature = globalData.oldestCreature_gO;

                if (creature != null)
                {
                    inputNeurons = creature.GetComponent<DynamicNetwork>().nn.GetInputNeurons();
                    hiddenNeurons = creature.GetComponent<DynamicNetwork>().nn.GetHiddenNeurons();
                    outputNeurons = creature.GetComponent<DynamicNetwork>().nn.GetOutputNeurons();

                    ResetVisualNetwork();
                    SpawnVisualNetwork();
                }
            }
            catch (System.Exception)
            {

                throw;
            }

            lastTimeDrawing = Time.realtimeSinceStartup;
        }
    }

    public void ResetVisualNetwork()
    {
        DeleteVisNeurons();
        vis_neurons = new List<GameObject>();
    }

    public void SpawnVisualNetwork()
    {
        foreach (InputNeuron inputNeuron in inputNeurons)
        {
            //Neuronenkugeln spawnen
            Vector3 spawnPosition = Start_Position + Vector3.Scale(inputNeuron.position, globalData.SpaceBetweenNeurons);
            GameObject vis_Neuron = Instantiate(prefabNeuron, spawnPosition, Quaternion.identity);
            vis_Neuron.GetComponent<VisualNeuronDynamic>().neuron = inputNeuron;
            vis_Neuron.GetComponent<VisualNeuronDynamic>().vn = this;
            vis_Neuron.transform.parent = GameObject.Find("vis_Neurons").transform;
            vis_neurons.Add(vis_Neuron);
        }

        foreach (WorkingNeuron hiddenNeuron in hiddenNeurons)
        {
            //Neuronenkugeln spawnen
            Vector3 spawnPosition = Start_Position + Vector3.Scale(hiddenNeuron.position, globalData.SpaceBetweenNeurons);
            GameObject vis_Neuron = Instantiate(prefabNeuron, spawnPosition, Quaternion.identity);
            vis_Neuron.GetComponent<VisualNeuronDynamic>().neuron = hiddenNeuron;
            vis_Neuron.GetComponent<VisualNeuronDynamic>().vn = this;
            vis_Neuron.transform.parent = GameObject.Find("vis_Neurons").transform;
            vis_neurons.Add(vis_Neuron);
        }

        foreach (WorkingNeuron outputNeuron in outputNeurons)
        {
            //Neuronenkugeln spawnen
            Vector3 spawnPosition = Start_Position + Vector3.Scale(outputNeuron.position, globalData.SpaceBetweenNeurons);
            GameObject vis_Neuron = Instantiate(prefabNeuron, spawnPosition, Quaternion.identity);
            vis_Neuron.GetComponent<VisualNeuronDynamic>().neuron = outputNeuron;
            vis_Neuron.GetComponent<VisualNeuronDynamic>().vn = this;
            vis_Neuron.transform.parent = GameObject.Find("vis_Neurons").transform;
            vis_neurons.Add(vis_Neuron);
        }
    }

    private void DeleteVisNeurons()
    {
        foreach (GameObject vis_Neuron in vis_neurons)
        {
            vis_Neuron.GetComponent<VisualNeuronDynamic>().DeleteLineRenderer();
            Destroy(vis_Neuron);
        }
    }
}
