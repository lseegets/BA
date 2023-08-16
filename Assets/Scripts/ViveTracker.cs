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

    private float prevDistance;

    private float distanceToPrevTarget;

    private Vector3 currentTargetPos;
    private Vector3 previousTargetPos;

    private int currentTargetCount;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;

        totalDistance = 0;

        prevDistance = Vector3.Distance(previousTargetPos, lastPos);

        currentTargetCount = transform.parent.Find("Camera").GetComponent<TargetSpawn>().currentTargetCount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TrackMovement();
    }

    public void TrackMovement()
    {
        currentPos = transform.position;

        CheckDistanceToPrevTarget();
        CalculateDistance();
        
        lastPos = currentPos;
    }

    public void HandlePositionData(Vector3 currentTarget, Vector3 previousTarget)
    {
        currentTargetPos = currentTarget;
        previousTargetPos = previousTarget;
    }

    // distances printen & vergleichen
    private void CheckDistanceToPrevTarget()
    {
        float currentDistance = Vector3.Distance(previousTargetPos, currentPos);

        if (prevDistance <= currentDistance)
        {
            goingForward = true;
        }
        else if (prevDistance > currentDistance)
        {
            goingForward = false;
        }

        prevDistance = currentDistance;
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
    }


}
