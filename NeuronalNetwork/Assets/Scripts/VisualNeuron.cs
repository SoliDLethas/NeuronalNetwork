using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NNTest;

public class VisualNeuron : MonoBehaviour
{
    public InputNeuron InputNeuron;
    public WorkingNeuron workingNeuron;
    public NeuronType neuronType;
    private Neuron neuron;
    public Material mat_Default;
    public Material mat_Activated;
    public List<GameObject> gOs_LineRenderer = new List<GameObject>();
    public VisualNetwork vn;

    // Start is called before the first frame update
    void Start()
    {
        if(neuronType == NeuronType.WorkingNeuron)
        {
            //Neuronenlinien spawnen
            foreach (Connection connection in workingNeuron.connections)
            {
                Neuron neuronOrigin = connection.getNeuron();
                GameObject gO_line = new GameObject("lr_" + neuronOrigin.x + "_" + neuronOrigin.y + "_to_" + workingNeuron.x + "_" + workingNeuron.y);
                LineRenderer lineRenderer = gO_line.AddComponent<LineRenderer>();
                lineRenderer.material = mat_Default;
                lineRenderer.widthMultiplier = 0.2f;
                lineRenderer.SetPosition(0, vn.Start_Position + new Vector3(neuronOrigin.x * vn.SpaceBetweenVisNeurons, neuronOrigin.y * vn.SpaceBetweenVisNeurons, 0));
                lineRenderer.SetPosition(1, transform.position);
                gOs_LineRenderer.Add(gO_line);
            }

            neuron = workingNeuron;
        }
        else
        {
            neuron = InputNeuron;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (neuron.getValue() >= 1f)
        {
            GetComponent<Renderer>().material = mat_Activated;

            //foreach (GameObject line in gOs_LineRenderer)
            //{
            //    line.GetComponent<LineRenderer>().material = mat_Activated;
            //}
        }
        else
        {
            GetComponent<Renderer>().material = mat_Default;

            //foreach (GameObject line in gOs_LineRenderer)
            //{
            //    line.GetComponent<LineRenderer>().material = mat_Default;
            //}
        }
    }
    public void DeleteLineRenderer()
    {
        foreach (GameObject lr in gOs_LineRenderer)
        {
            GameObject.Destroy(lr);
        }
    }
}
