using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NNDynamic;
public class VisualNeuronDynamic : MonoBehaviour
{
    public float lastSum = 0;
    public float neuronValue = 0;
    public Neuron neuron;
    public Material mat_Default;
    public Material mat_Activated;
    public List<GameObject> gOs_LineRenderer = new List<GameObject>();
    public VisualNetworkDynamic vn;

    private GlobalData globalData;

    // Start is called before the first frame update
    void Start()
    {
        globalData = GameObject.Find("GameManager").GetComponent<GlobalData>();

        if (neuron.GetType() == typeof(WorkingNeuron))
        {
            WorkingNeuron workingNeuron = (WorkingNeuron)neuron;

            neuronValue = workingNeuron.value;
            lastSum = workingNeuron.lastSum;

            //Neuronenlinien spawnen
            foreach (Connection connection in workingNeuron.connections)
            {
                Neuron neuronOrigin = connection.Neuron;
                GameObject gO_line = new GameObject("lr_" + neuronOrigin.position.x + "_" + neuronOrigin.position.y + "_" + neuronOrigin.position.z + "_to_" + workingNeuron.position.x + "_" + workingNeuron.position.y + "_" + workingNeuron.position.z);
                gO_line.transform.parent = GameObject.Find("lineRenderer").transform;
                LineRenderer lineRenderer = gO_line.AddComponent<LineRenderer>();
                lineRenderer.material = mat_Default;
                gO_line.AddComponent<vis_Connection>().connection = connection;
                lineRenderer.widthMultiplier = 0.05f;
                lineRenderer.SetPosition(0, vn.Start_Position + Vector3.Scale(neuronOrigin.position,globalData.SpaceBetweenNeurons));
                lineRenderer.SetPosition(1, transform.position);
                lineRenderer.generateLightingData = true;
                gOs_LineRenderer.Add(gO_line);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (neuron.activated)
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

        //foreach (GameObject lr in gOs_LineRenderer)
        //{
        //    Connection connection = lr.GetComponent<vis_Connection>().connection;
        //    LineRenderer lineRenderer = lr.GetComponent<LineRenderer>();
        //    //Min 0.05f
        //    //Max 0.5f
        //    //Mathf.Abs(connection.Weight) / 2
        //    lineRenderer.widthMultiplier = Mathf.Abs(connection.Weight) / 2;
        //}
    }
    public void DeleteLineRenderer()
    {
        foreach (GameObject lr in gOs_LineRenderer)
        {
            GameObject.Destroy(lr);
        }
    }
}
