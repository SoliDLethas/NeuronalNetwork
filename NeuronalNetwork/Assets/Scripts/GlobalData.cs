using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlobalData : MonoBehaviour
{
    public Text txt_food_Values;
    public Text txt_bestCreature;

    public GameObject prefabCreature;

    public float oldestCreature_Age;
    public string oldestCreature_Name;
    public GameObject oldestCreature_gO;

    public bool overrideOldestCreature = true;

    public int bornRate = 3;

    public Vector3 SpaceBetweenNeurons = new Vector3(10,5,0);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        txt_food_Values.text = "";

        foreach (GameObject gObject in GameObject.FindGameObjectsWithTag("Creature"))
        {
            txt_food_Values.text += "Id : " + gObject.name + "  Food_Value : " + gObject.GetComponent<Creature>().food_Value.ToString("F0") + "\n";
        }

        txt_bestCreature.text = "";
        txt_bestCreature.text += "Oldest Age : " + oldestCreature_Age.ToString("F0") + "\n";
        txt_bestCreature.text += "Id : " + oldestCreature_Name + "\n";
        txt_bestCreature.text += "BirthCap : " + bornRate * GameObject.FindGameObjectsWithTag("Creature").Length + "\n";
     

        
    }

}
