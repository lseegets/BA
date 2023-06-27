using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public List<KeyValuePair<float, float>> trackingData = new();
   // public List<float> trackingData = new List<float>();

    public Vector3 lastPos;
    private bool moving;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TrackMovement();
        // Debug.Log("From Tracker: " + lastPos);
    }

    public void TrackMovement()
    {
        Vector3 currentPos = transform.position;
        if (lastPos != currentPos)
        {
            CalculateDistance(currentPos, lastPos);
        }
        else if (lastPos == currentPos)
        {
            moving = false;
        }
        lastPos = currentPos;
    }

    private float CalculateDistance(Vector3 currentPosition, Vector3 lastPosition)
    {
        float xDistance = Mathf.Pow(currentPosition.x - lastPosition.x, 2);
        float yDistance = Mathf.Pow(currentPosition.y - lastPosition.y, 2);
        float zDistance = Mathf.Pow(currentPosition.z - lastPosition.z, 2);

        float radicand = xDistance + yDistance + zDistance;

        float distance = Mathf.Sqrt(radicand);

        trackingData.Add(new KeyValuePair<float, float>(Timer.timer, distance));
      //  trackingData.Add(distance);
        return distance;
    }
}
