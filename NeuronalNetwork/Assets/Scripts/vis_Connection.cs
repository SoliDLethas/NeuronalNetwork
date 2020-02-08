using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NNDynamic;

public class vis_Connection : MonoBehaviour
{
    public Connection connection;
    public float weight;
    public float activationWithinDurationAverageValue;
    public LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        weight = connection.Weight;
        activationWithinDurationAverageValue = connection.activationWithinDurationAverageValue;
        //Min 0.05f
        //Max 0.5f
        //Mathf.Abs(connection.Weight) / 2
        lineRenderer.widthMultiplier = Mathf.Abs(connection.Weight) / 2;
    }
}
