using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static int id_Object;
    public GameObject prefab;
    public int maxObjects = 25;
    public Vector3 spawnBoundariesMin, spawnBoundariesMax;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        while(GameObject.FindGameObjectsWithTag(prefab.tag).Length < maxObjects)
        {
            id_Object += 1;
            GameObject gO = Instantiate(prefab, GlobalFunctions.RndPosWithinCube(spawnBoundariesMin, spawnBoundariesMax),Quaternion.Euler(0,Random.Range(0,359),0));
            gO.name = id_Object.ToString();
            gO.transform.parent = GameObject.Find("SpawnerObjects").transform;
        }   
    }
}
