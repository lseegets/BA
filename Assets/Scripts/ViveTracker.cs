using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveTracker : MonoBehaviour
{
    public List<KeyValuePair<float, float>> trackingData = new();
   // public List<Vector3> controllerPos = new();
    public List<string> controllerPos = new();
    //public List<Vector3> cameraPos = new();
    public List<string> cameraPos = new();
    public Vector3 lastPos;
    public float totalDistance = 0;

    private Vector3 currentPos;

    private bool goingForward;

    private float time;

    private float prevDistance;

    private Vector3 currentTargetPos;
    private Vector3 previousTargetPos;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;

        totalDistance = 0;

        Vector2 prevTarget = previousTargetPos;
        Vector2 lastPos2 = lastPos;
        prevDistance = Vector2.Distance(prevTarget, lastPos2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TrackMovement();
    }

    public void TrackMovement()
    {
        currentPos = transform.position;

        // if (currentPos != lastPos)
        // {
        // Debug.Log("prev: " + lastPos);
        // Debug.Log("current: " + currentPos);
        CheckDistanceToPrevTarget();
        CalculateDistance(currentPos, lastPos);
        // }
        /*else if (time > 2f) time = 0;
        else if (time < 2f)
        {
            time += Time.deltaTime;
        }*/

        //CheckPosition();
        /*if (lastPos != currentPos)
        {
            CalculateDistance(currentPos, lastPos);
        }*/

        lastPos = currentPos;
    }

    //public void HandlePositionData(bool distanceX, bool distanceY)
    public void HandlePositionData(Vector3 currentTarget, Vector3 previousTarget)
    {
        //distancePositiveX = distanceX;
        //distancePositiveY = distanceY;
        currentTargetPos = currentTarget;
        previousTargetPos = previousTarget;
    }

    private void CheckDistanceToPrevTarget()
    {
        Vector2 prevTarget = previousTargetPos;
        //Vector2 lastPos2 = lastPos;
        Vector2 currentPos2 = currentPos;

        // float prevDistance = Vector2.Distance(prevTarget, lastPos2);
        float currentDistance = Vector2.Distance(prevTarget, currentPos2);

        if (prevDistance <= currentDistance)
        {
            goingForward = true;
        }
        else if (prevDistance > currentDistance)
        {
            goingForward = false;
        }

        prevDistance = currentDistance;

        //Debug.Log("GoingForward: " + goingForward + "                distance: " + (currentDistance - prevDistance));
    }

    private void CalculateDistance(Vector3 currentPosition, Vector3 lastPosition)
    {
        //float distance = (currentPosition - lastPosition).magnitude;

        Vector2 currentPos2 = currentPos;
        Vector2 lastPos2 = lastPos;
        float distance = (currentPos2 - lastPos2).magnitude;

        float speed = (currentPos - lastPos).magnitude / Time.deltaTime;

        if (goingForward)
        {
            totalDistance += distance;
        }
        else if (!goingForward)
        {
            totalDistance -= distance;
        }

        /* if (!goingForward)
         {
             speed -= 2 * speed;
         }*/

        // totalDistance += distance;

        trackingData.Add(new KeyValuePair<float, float>(Timer.timer, totalDistance));
        controllerPos.Add(currentPos.ToString("F4"));
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position.ToString("F4"));
    }


}
