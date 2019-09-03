using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public I_MoveObject context;
    public float speed_transform = 8;
    public float speed_rotate = 10;

    public Vector3 direction = Vector3.zero;
    public Vector3 rotation = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        context = GetComponent<Creature>();
    }

    // Update is called once per frame
    void Update()
    {
        context.MoveObject(ref direction, ref rotation);
        // Move the object forward along its z axis 1 unit/second.
        transform.Translate(direction * speed_transform * Time.deltaTime,Space.Self);

        transform.Rotate(rotation * speed_rotate * Time.deltaTime,Space.Self);
    }
}

public interface I_MoveObject
{
    void MoveObject(ref Vector3 direction, ref Vector3 rotation);
}
