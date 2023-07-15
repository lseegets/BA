using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tracker : MonoBehaviour
{
    public List<KeyValuePair<float, float>> trackingData = new();
    public List<Vector3> controllerPos = new();
    public List<Vector3> cameraPos = new();
    public Vector3 lastPos;
    public float totalDistance = 0;

    private Vector3 currentPos;

    private bool distancePositiveX;
    private bool distancePositiveY;
    private bool goingForward;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        totalDistance = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TrackMovement();
        
        Debug.Log("current X: " + distancePositiveX + "             current Y: " + distancePositiveY);
    }

    public void TrackMovement()
    {
        currentPos = transform.position;
        CheckPosition();
        if (lastPos != currentPos)
        {
            CalculateDistance(currentPos, lastPos);
        }

        lastPos = currentPos;
    }

    public void HandlePositionData(bool distanceX, bool distanceY)
    {
        distancePositiveX = distanceX;
        distancePositiveY = distanceY;
    }

    // Problem liegt hier irgendwo bzw. mal den Debug in FixedUpdate prüfen
    private void CheckPosition()
    {
        bool goingForwardX = true;
        bool goingForwardY = true;

        if (distancePositiveX)
        {
            if (currentPos.x >= lastPos.x)
            {
                goingForwardX = true;
            }
            else if (currentPos.x < lastPos.x)
            {
                goingForwardX = false;
            }
        }
        else if (!distancePositiveX)
        {
            if (currentPos.x >= lastPos.x)
            {
                goingForwardX = false;
            }
            else if (currentPos.x < lastPos.x)
            {
                goingForwardX = true;
            }
        }

        if (distancePositiveY)
        {
            if (currentPos.y >= lastPos.y)
            {
                goingForwardY = true;
            }
            else if (currentPos.y < lastPos.y)
            {
                goingForwardY = false;
            }
        }
        else if (!distancePositiveY)
        {
            if (currentPos.y >= lastPos.y)
            {
                goingForwardY = false;
            }
            else if (currentPos.y < lastPos.y)
            {
                goingForwardY = true;
            }
        }

        // keep a look out for this
        if (!goingForwardX && !goingForwardY)
        {
            goingForward = false;
        }
        else if (goingForwardX || goingForwardY)
        {
            goingForward = true;
        }
    }

    private void CalculateDistance(Vector3 currentPosition, Vector3 lastPosition)
    {
        float distance = (currentPosition - lastPosition).magnitude;

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
        controllerPos.Add(currentPos);
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position);
    }
}
