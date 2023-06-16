using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    private Vector3 lastPos;
    private bool moving;
    private Rigidbody rb;

    private float[] trackingData = new float[] {1.2f, 2.3f, 3.4f};

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       // TrackMovement();
        Debug.Log(rb.velocity.magnitude);
    }

    private void TrackMovement()
    {
        Vector3 currentPos = transform.position;
        if (lastPos != currentPos)
        {
            moving = true;
       //     SaveData.Save(trackingData);
        }
        else if (lastPos == currentPos)
        {
            moving = false;
        }
        lastPos = currentPos;
    }
}
