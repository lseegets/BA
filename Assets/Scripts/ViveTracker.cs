using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveTracker : MonoBehaviour
{
    public List<KeyValuePair<float, decimal>> trackingData = new();
    public List<string> controllerPos = new();
    public List<decimal> distanceToPrevPos = new();
    public List<string> cameraPos = new();
    public List<Vector3> controllerRot = new();
    public List<decimal> distanceToLastTarget = new();
    public List<float> distanceToCurrentTarget = new();
    public List<float> vectorX = new();
    public List<float> vectorY = new();
    public List<float> vectorZ = new();

    public Vector3 lastPos;
    public decimal totalDistance = 0;

    private Vector3 currentPos;
    private Vector3 rot;
    private Vector3 currentTargetPos;
    private Vector3 previousTargetPos;

    private bool goingForward;
    private decimal prevDistance;
    private decimal currentDistance;
    private float distanceToCurrTarget;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
        rot = transform.rotation.eulerAngles;

        prevDistance = (decimal)Vector3.Distance(previousTargetPos, lastPos);

        distanceToCurrTarget = Vector3.Distance(currentTargetPos, lastPos);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TrackMovement();
    }

    public void TrackMovement()
    {
        currentPos = transform.position;

        CheckDistanceToTargets();
        CalculateDistance();
        
        lastPos = currentPos;
        rot = transform.rotation.eulerAngles;
    }

    public void HandlePositionData(Vector3 currentTarget, Vector3 previousTarget)
    {
        currentTargetPos = currentTarget;
        previousTargetPos = previousTarget;
    }

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

        prevDistance = currentDistance;
        distanceToCurrTarget = Vector3.Distance(currentTargetPos, currentPos);
    }

    private void CalculateDistance()
    {
        decimal distance = (decimal)Vector3.Distance(currentPos, lastPos);

        if (goingForward)
        {
            totalDistance += distance;
        }
        else if (!goingForward)
        {
            totalDistance -= distance;
        }

        trackingData.Add(new KeyValuePair<float, decimal>(Timer.timer, totalDistance));
        controllerPos.Add(currentPos.ToString("F9"));
        distanceToPrevPos.Add(distance);
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position.ToString("F9"));
        controllerRot.Add(rot);
        distanceToLastTarget.Add(prevDistance);
        distanceToCurrentTarget.Add(distanceToCurrTarget);

        vectorX.Add(currentPos.x);
        vectorY.Add(currentPos.y);
        vectorZ.Add(currentPos.z);
    }

}
