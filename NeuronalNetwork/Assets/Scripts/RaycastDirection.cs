using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastDirection : MonoBehaviour
{
    public float maxRange = 10;
    public List<Vector3> directions = new List<Vector3>();
    public List<RayCastHitResult> hits = new List<RayCastHitResult>();
    public I_RaycastHit context;
    public Vector3 offsetStartPoint = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        context = GetComponent<Creature>();
    }

    // Update is called once per frame
    void Update()
    {
        hits = new List<RayCastHitResult>();

        foreach (Vector3 direction in directions)
        {
            // Bit shift the index of the layer (8) to get a bit mask
            //int layerMask = 1 << 8;

            // This would cast rays only against colliders in layer 8.
            // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
            //layerMask = ~layerMask;

            RaycastHit rayCastHit;
            // Does the ray intersect any objects excluding the player layer
            //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
            if (Physics.Raycast(transform.position +  transform.TransformDirection(offsetStartPoint), transform.TransformDirection(direction), out rayCastHit, maxRange))
            {
                Debug.DrawRay(transform.position +  transform.TransformDirection(offsetStartPoint), transform.TransformDirection(direction) * rayCastHit.distance, Color.red);
                hits.Add(new RayCastHitResult(directions.IndexOf(direction),rayCastHit.collider.gameObject));
            }
            else
            {
                Debug.DrawRay(transform.position +  transform.TransformDirection(offsetStartPoint), transform.TransformDirection(direction) * maxRange, Color.white);
                hits.Add(new RayCastHitResult(directions.IndexOf(direction), null));
            }
        }

        if (hits.Count > 0)
        {
            context.OnRayCastHit(hits); 
        }
    }
}

public interface I_RaycastHit
{
    void OnRayCastHit(List<RayCastHitResult> hitObjectResults);
}

public class RayCastHitResult
{
    public int indexDirection;
    public GameObject gameObject;

    public RayCastHitResult(int indexDirection, GameObject gameObject)
    {
        this.indexDirection = indexDirection;
        this.gameObject = gameObject;
    }
}
