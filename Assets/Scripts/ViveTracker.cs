using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveTracker : MonoBehaviour
{
    public List<KeyValuePair<float, decimal>> trackingData = new();
    public List<string> controllerPos = new();

    public List<decimal> distanceToPrevPos = new();

    public List<string> cameraPos = new();
    //public List<Quaternion> controllerRot = new();
    public List<Vector3> controllerRot = new();

    public List<decimal> distanceToLastTarget = new();
    public List<float> distanceToCurrentTarget = new();

    public List<float> vectorX = new();
    public List<float> vectorY = new();
    public List<float> vectorZ = new();

    //public List<KeyValuePair<float, float>> trackingData2 = new();
    public List<KeyValuePair<float, decimal>> trackingData2 = new();
    public List<string> controllerPos2 = new();

    //public List<string> distanceToPrevPos2 = new();
    public List<decimal> distanceToPrevPos2 = new();

    //public List<string> distanceToLastTarget2 = new();
    public List<decimal> distanceToLastTarget2 = new();
   // public List<string> distanceToCurrentTarget2 = new();
    public List<float> distanceToCurrentTarget2 = new();

    public List<float> vectorX2 = new();
    public List<float> vectorY2 = new();
    /// 

    public Vector3 lastPos;
    public Vector2 lastPos2;

    //public float totalDistance = 0;
    public decimal totalDistance = 0;
    public decimal totalDistance2 = 0;

    private Vector3 currentPos;
    private Vector2 currentPos2;

    //private Quaternion rot;
    private Vector3 rot;

    private bool goingForward;
    private bool goingForward2;

    private decimal prevDistance;
    private float prevDistance2;
    private decimal currentDistance;
    private float currentDistance2;
    private decimal prevDistanceDecimal2;
    private decimal currentDistanceDecimal2;

    //private float distanceToPrevTarget;
    private decimal distanceToPrevTarget;

    private float distanceToCurrTarget;
    private float distanceToCurrTarget2;

    private Vector3 currentTargetPos;
    private Vector2 currentTargetPos2;
    private Vector3 previousTargetPos;
    private Vector2 previousTargetPos2;

    private int currentTargetCount;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        lastPos2 = transform.position;
        rot = transform.rotation.eulerAngles;

       // totalDistance = 0;
       // totalDistance2 = 0;

        prevDistance = (decimal)Vector3.Distance(previousTargetPos, lastPos);
        prevDistance2 = Vector2.Distance(previousTargetPos2, lastPos2);
        prevDistanceDecimal2 = (decimal)prevDistance2;

        distanceToCurrTarget = Vector3.Distance(currentTargetPos, lastPos);
        distanceToCurrTarget2 = Vector2.Distance(currentTargetPos2, lastPos2);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TrackMovement();
    }

    public void TrackMovement()
    {
        currentPos = transform.position;
        currentPos2 = transform.position;

        CheckDistanceToTargets();
        CalculateDistance();
        CheckDistanceToTargets2D();
        CalculateDistance2D();
        
        lastPos = currentPos;
        lastPos2 = currentPos2;
        rot = transform.rotation.eulerAngles;
    }

    public void HandlePositionData(Vector3 currentTarget, Vector3 previousTarget)
    {
        currentTargetPos = currentTarget;
        previousTargetPos = previousTarget;
        
        currentTargetPos2 = currentTarget;
        previousTargetPos2 = previousTarget;
    }

    /*public void HandleRayData(Vector3 currentRayPos, Vector3 lastRayPos)
    {
        this.currentRayPos = currentRayPos;
        this.lastRayPos = lastRayPos;
    }*/

    private void CheckDistanceToTargets()
    {
        currentDistance = (decimal)Vector3.Distance(previousTargetPos, currentPos);

        if (prevDistance <= currentDistance)
        {
            goingForward = true;
        }
        else if (prevDistance > currentDistance)
        {
            goingForward = false;
        }

        //distanceToPrevTarget = prevDistanceDecimal;
        prevDistance = currentDistance;
        distanceToCurrTarget = Vector3.Distance(currentTargetPos, currentPos);
    }

    private void CheckDistanceToTargets2D()
    {
        currentDistance2 = Vector2.Distance(previousTargetPos2, currentPos2);
        currentDistanceDecimal2 = (decimal)currentDistance2;

        if (prevDistanceDecimal2 <= currentDistanceDecimal2)
        {
            goingForward2 = true;
        }
        else if (prevDistanceDecimal2 > currentDistanceDecimal2)
        {
            goingForward2 = false;
        }

        //distanceToPrevTarget = prevDistanceDecimal;
        prevDistance2 = currentDistance2;
        prevDistanceDecimal2 = currentDistanceDecimal2;
        distanceToCurrTarget2 = Vector2.Distance(currentTargetPos2, currentPos2);
    }

    private void CalculateDistance2D()
    {
        float distance2 = Vector2.Distance(currentPos2, lastPos2); //(currentPos - lastPos).magnitude;

        decimal distanceDecimal2 = (decimal)distance2;

        //  distanceToPrevTarget = (currentPos - previousTargetPos).magnitude;

        /*if (goingForward)
        {
            totalDistance += distance;
        }
        else if (!goingForward)
        {
            totalDistance -= distance;
        }*/

        if (goingForward2)
        {
            totalDistance2 += distanceDecimal2;
        }
        else if (!goingForward2)
        {
            totalDistance2 -= distanceDecimal2;
        }

        //trackingData.Add(new KeyValuePair<float, float>(Timer.timer, /*distanceToPrevTarget));*/ totalDistance));
        trackingData2.Add(new KeyValuePair<float, decimal>(Timer.timer, /*distanceToPrevTarget));*/ totalDistance2));
        controllerPos2.Add(currentPos2.ToString("F9"));
        distanceToPrevPos2.Add(distanceDecimal2);
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position.ToString("F9"));
        controllerRot.Add(rot);
        // distanceToLastTarget.Add(distanceToPrevTarget.ToString("F4"));
        distanceToLastTarget2.Add(prevDistanceDecimal2);
        distanceToCurrentTarget2.Add(distanceToCurrTarget2);

        vectorX2.Add(currentPos.x);
        vectorY2.Add(currentPos.y);
    }

    private void CalculateDistance()
    {
        decimal distance = (decimal)Vector3.Distance(currentPos, lastPos); //(currentPos - lastPos).magnitude;

      //  distanceToPrevTarget = (currentPos - previousTargetPos).magnitude;

        if (goingForward)
        {
            totalDistance += distance;
        }
        else if (!goingForward)
        {
            totalDistance -= distance;
        }

        //trackingData.Add(new KeyValuePair<float, float>(Timer.timer, /*distanceToPrevTarget));*/ totalDistance));
        trackingData.Add(new KeyValuePair<float, decimal>(Timer.timer, /*distanceToPrevTarget));*/ totalDistance));
        controllerPos.Add(currentPos.ToString("F9"));
        distanceToPrevPos.Add(distance);
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position.ToString("F9"));
        controllerRot.Add(rot);
        // distanceToLastTarget.Add(distanceToPrevTarget.ToString("F4"));
        distanceToLastTarget.Add(prevDistance);
        distanceToCurrentTarget.Add(distanceToCurrTarget);

        vectorX.Add(currentPos.x);
        vectorY.Add(currentPos.y);
        vectorZ.Add(currentPos.z);
    }

}
