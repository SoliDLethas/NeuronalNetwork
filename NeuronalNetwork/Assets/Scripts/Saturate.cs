using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saturate : MonoBehaviour
{
    public float saturation_Value = 50f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public interface ISaturate
    {
        void Saturate(float saturation_Value);
    }

    void OnCollisionEnter(Collision col)
    {
        Creature creature = col.gameObject.GetComponent<Creature>();

        if (creature != null)
        {
            creature.Saturate(saturation_Value);
            Destroy(gameObject);
        }
    }

    
}
