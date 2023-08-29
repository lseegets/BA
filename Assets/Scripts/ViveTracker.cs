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
    public List<string> distanceToLastTarget = new();
    public List<string> distanceToCurrentTarget = new();

    public Vector3 lastPos;
    public float totalDistance = 0;

    private Vector3 currentPos;

    private bool goingForward;

    private float prevDistance;
    private float currentDistance;
    private decimal prevDistanceDecimal;
    private decimal currentDistanceDecimal;

    //private float distanceToPrevTarget;
    private decimal distanceToPrevTarget;

    private float distanceToCurrTarget;

    private Vector3 currentTargetPos;
    private Vector3 previousTargetPos;

    private int currentTargetCount;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;

        totalDistance = 0;

        prevDistance = Vector3.Distance(previousTargetPos, lastPos);
        prevDistanceDecimal = (decimal)prevDistance;

        distanceToCurrTarget = Vector3.Distance(currentTargetPos, lastPos);

        currentTargetCount = transform.parent.Find("Camera").GetComponent<TargetSpawn>().currentTargetCount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TrackMovement();
        Debug.Log(currentTargetPos);
        Debug.Log(previousTargetPos);
    }

    public void TrackMovement()
    {
        currentPos = transform.position;

        CheckDistanceToTargets();
        CalculateDistance();
        
        lastPos = currentPos;
    }

    public void HandlePositionData(Vector3 currentTarget, Vector3 previousTarget)
    {
        currentTargetPos = currentTarget;
        previousTargetPos = previousTarget;
    }

    // distances printen & vergleichen
    private void CheckDistanceToTargets()
    {
        currentDistance = Vector3.Distance(previousTargetPos, currentPos);
        currentDistanceDecimal = (decimal)currentDistance;

        if (prevDistanceDecimal <= currentDistanceDecimal)
        {
            goingForward = true;
        }
        else if (prevDistanceDecimal > currentDistanceDecimal)
        {
            goingForward = false;
        }

        //distanceToPrevTarget = prevDistanceDecimal;
        prevDistance = currentDistance;
        prevDistanceDecimal = (decimal)prevDistance;
        distanceToCurrTarget = Vector3.Distance(currentTargetPos, currentPos);
    }

    private void CalculateDistance()
    {
        float distance = (currentPos - lastPos).magnitude;

      //  distanceToPrevTarget = (currentPos - previousTargetPos).magnitude;

        if (goingForward)
        {
            totalDistance += distance;
        }
        else if (!goingForward)
        {
            totalDistance -= distance;
        }

        trackingData.Add(new KeyValuePair<float, float>(Timer.timer, /*distanceToPrevTarget));*/ totalDistance));
        controllerPos.Add(currentPos.ToString("F4"));
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position.ToString("F4"));
       // distanceToLastTarget.Add(distanceToPrevTarget.ToString("F4"));
        distanceToLastTarget.Add(prevDistance.ToString("F4"));
        distanceToCurrentTarget.Add(distanceToCurrTarget.ToString("F4"));
    }


}
