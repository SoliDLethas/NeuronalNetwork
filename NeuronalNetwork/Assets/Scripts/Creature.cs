using System.Collections.Generic;
using UnityEngine;
using NNDynamic;

public class Creature : MonoBehaviour, I_MoveObject, I_RaycastHit, IExchangeNeuronData, Saturate.ISaturate
{
    public List<bool> rayCastResults = new List<bool>();

    public float food_Value = 100f;
    public float age;

    public bool moveForward;
    public bool moveBackward;
    public bool turnLeft;
    public bool turnRight;

    bool isChild = false;

    public Material mat_blue;
    public Material mat_white;
    public Material mat_yellow;

    public Creature parent;

    private GlobalData globalData;

    // Start is called before the first frame update
    void Start()
    {
        globalData = GameObject.Find("GameManager").GetComponent<GlobalData>();

        RaycastDirection raycastDirection = GetComponent<RaycastDirection>();

        //Beschreibt den Versatz der "Augen" zum Creature Mittelpunkt
        raycastDirection.offsetStartPoint = new Vector3(0, 0, 0.6f);

        //Gibt die Richtung der Sensoren vor
        raycastDirection.directions.Add(new Vector3(0, -0.25f, 1));
        raycastDirection.directions.Add(new Vector3(0, 0, 1));
        raycastDirection.directions.Add(new Vector3(-0.5f, 0, 1));
        raycastDirection.directions.Add(new Vector3(0.5f, 0, 1));
    }

    // Update is called once per frame
    void Update()
    {
        Starvation();

        GiveBirth();

        CalculateAge();
    }

    private void CalculateAge()
    {
        age += Time.deltaTime;
        
        if (globalData.oldestCreature_Age < age && globalData.overrideOldestCreature)
        {
            globalData.oldestCreature_Age = age;
            globalData.oldestCreature_Name = gameObject.name;
            globalData.oldestCreature_gO = gameObject;

            GetComponent<Renderer>().material = mat_blue;
        }
        else if(isChild)
        {
            GetComponent<Renderer>().material = mat_yellow;
        }
        else
        {
            GetComponent<Renderer>().material = mat_white;
        }
    }

    private void GiveBirth()
    {
        if (food_Value >= (globalData.bornRate * GameObject.FindGameObjectsWithTag("Creature").Length))
        {
            GameObject gO_Child = Instantiate(globalData.prefabCreature, transform.position + new Vector3(3, 3, 3), Quaternion.Euler(0, Random.Range(0, 359), 0));
            Spawner.id_Object += 1;
            gO_Child.name = gameObject.name + "|" + Spawner.id_Object;
            gO_Child.transform.parent = GameObject.Find("Childs").transform;

            Creature creature_Child = gO_Child.GetComponent<Creature>();
            creature_Child.isChild = true;
            creature_Child.parent = this;

            DynamicNetwork network_Child = gO_Child.GetComponent<DynamicNetwork>();
            DynamicNetwork network_Parent = GetComponent<DynamicNetwork>();

            network_Child.parentNetwork = network_Parent.nn;

            food_Value -= 50;
        }
    }

    private void Starvation()
    {
        food_Value -= 10f * Time.deltaTime;

        if (food_Value <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void OnRayCastHit(List<RayCastHitResult> RayCastHitResults)
    {
        rayCastResults = new List<bool>();

        foreach (RayCastHitResult rayCastHitResult in RayCastHitResults)
        {
            if(rayCastHitResult.gameObject == null)
            {
                rayCastResults.Add(false);
                rayCastResults.Add(false);
                rayCastResults.Add(false);
            }
            else if (rayCastHitResult.gameObject.tag == "Terrain")
            {
                rayCastResults.Add(true);
                rayCastResults.Add(false);
                rayCastResults.Add(false);
            }
            else if (rayCastHitResult.gameObject.tag == "Creature")
            {
                rayCastResults.Add(false);
                rayCastResults.Add(true);
                rayCastResults.Add(false);
            }
            else if (rayCastHitResult.gameObject.tag == "Food")
            {
                rayCastResults.Add(false);
                rayCastResults.Add(false);
                rayCastResults.Add(true);
            }
        }
    }

    public void UpdateInputNeurons(List<InputNeuron> neurons)
    {
        try
        {
            for (int i = 0; i < rayCastResults.Count; i++)
            {
                if (rayCastResults[i]) { neurons[i].SetValue(1.1f); } else { neurons[i].SetValue(0f); }
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    public void ReadOutputNeurons(List<WorkingNeuron> neurons)
    {
        if (neurons[0].GetValue() >= 1f) { moveForward = true; } else { moveForward = false; }

        if (neurons[1].GetValue() >= 1f) { moveBackward = true; } else { moveBackward = false; }

        if (neurons[2].GetValue() >= 1f) { turnLeft = true; } else { turnLeft = false; }

        if (neurons[3].GetValue() >= 1f) { turnRight = true; } else { turnRight = false; }
    }

    public void MoveObject(ref Vector3 direction, ref Vector3 rotation)
    {
        //Direction
        if (moveForward && moveBackward == false)
        {
            direction = new Vector3(0, 0, 1);
        }
        else if (moveForward == false && moveBackward)
        {
            direction = new Vector3(0, 0, -1);
        }
        else
        {
            direction = new Vector3(0, 0, 0);
        }

        //Rotation
        if (turnLeft && turnRight == false)
        {
            rotation = new Vector3(0, -1, 0);
        }
        else if (turnLeft == false && turnRight)
        {
            rotation = new Vector3(0, 1, 0);
        }
        else
        {
            rotation = new Vector3(0, 0, 0);
        }
    }

    public void Saturate(float saturation_Value)
    {
        food_Value += saturation_Value;
    }
}
