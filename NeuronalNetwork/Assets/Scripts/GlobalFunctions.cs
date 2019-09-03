using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalFunctions : MonoBehaviour
{
    public static Vector3 RndPosWithinCube(Vector3 min, Vector3 max)
    {
        float x, y, z;
        x = Random.Range(min.x, max.x);
        y = Random.Range(min.y, max.y);
        z = Random.Range(min.z, max.z);

        return new Vector3(x, y, z);
    }

}
