using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInput : MonoBehaviour
{
    [SerializeField] GameObject cube;

    public static GameObject currentObject;
    public List<KeyValuePair<float, decimal>> trackingData = new();
    public List<string> cameraPos = new();

    public Material activatedMaterial;
    public Material defaultMaterial;

    private const float DestructionTime = 0.2f;

    private float countdown;
    private Plane plane;
    private ViveTracker viveTracker;
    private Vector3 currentRayPos;
    private Vector3 lastRayPos;
    private Vector3 currentTargetPos;
    private Vector3 previousTargetPos;

    public List<string> rayPos = new();
    public List<decimal> rayDistanceToLastTarget = new();
    public List<float> rayDistanceToCurrentTarget = new();
    public List<decimal> rayDistanceToPrevPos = new();

    public decimal totalRayDistance = 0;
    private decimal prevRayDistance;
    private decimal currentRayDistance;
    private float rayDistanceToCurrTarget;
    private bool goingForward;
    private decimal totalDistance = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        countdown = DestructionTime;
        viveTracker = GameObject.FindGameObjectsWithTag("Tracker")[0].GetComponent<ViveTracker>();

        lastRayPos = new Vector3(0, 0, 0);

        prevRayDistance = (decimal)Vector3.Distance(previousTargetPos, lastRayPos);

        rayDistanceToCurrTarget = Vector3.Distance(currentTargetPos, lastRayPos);
    }

    // Update is called once per frame
   /* void FixedUpdate()
    {
      if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100.0f))
      {
            Ray ray = new Ray(transform.position, transform.forward);
            float enter = 0.0f;
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                currentRayPos = hitPoint;
                SendRayData(currentRayPos, lastRayPos);
                lastRayPos = currentRayPos;
                //Debug.Log(hitPoint);
            }
            if (hit.collider.gameObject.CompareTag("Target"))
            {
                currentObject = hit.collider.gameObject;
                currentObject.GetComponent<MeshRenderer>().material = activatedMaterial;
                countdown -= Time.deltaTime;
                if (countdown < 0)
                {
                    Destroy(currentObject);
                    ResetCountdown();
                }
            }
            else
            {
                ResetCountdown();
                if (currentObject != null) currentObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
      }
    }*/

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float enter = 0.0f;
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                currentRayPos = hitPoint;
                CheckDistanceToTargets();
                CalculateDistance();
                cube.transform.position = hitPoint;
                //SendRayData(currentRayPos, lastRayPos);
                lastRayPos = currentRayPos;
                //Debug.Log(hitPoint);
            }
            if (hit.collider.gameObject.CompareTag("Target"))
            {
                currentObject = hit.collider.gameObject;
                currentObject.GetComponent<MeshRenderer>().material = activatedMaterial;
                countdown -= Time.deltaTime;
                if (countdown < 0)
                {
                    Destroy(currentObject);
                    ResetCountdown();
                }
            }
            else
            {
                ResetCountdown();
                if (currentObject != null) currentObject.GetComponent<MeshRenderer>().material = defaultMaterial;
            }
        }
        Debug.Log("Last: " + lastRayPos);
        Debug.Log("Current: " + currentRayPos);
    }

    public void GetPlaneData(Plane plane)
    {
        this.plane = plane;
      //  Debug.Log("Sent Plane Data");
    }

   /* public void SendRayData(Vector3 currentRayPos, Vector3 lastRayPos)
    {
        viveTracker.HandleRayData(currentRayPos, lastRayPos);
    }*/

    public void HandlePositionData(Vector3 currentTarget, Vector3 previousTarget)
    {
        currentTargetPos = currentTarget;
        previousTargetPos = previousTarget;
    }

    private void CheckDistanceToTargets()
    {
        currentRayDistance = (decimal)Vector3.Distance(previousTargetPos, currentRayPos);

        if (prevRayDistance <= currentRayDistance)
        {
            goingForward = true;
        }
        else if (prevRayDistance > currentRayDistance)
        {
            goingForward = false;
        }

        prevRayDistance = currentRayDistance;
        rayDistanceToCurrTarget = Vector3.Distance(currentTargetPos, currentRayPos);
    }

    private void CalculateDistance()
    {
        decimal distance = (decimal)Vector3.Distance(currentRayPos, lastRayPos);

        //  distanceToPrevTarget = (currentPos - previousTargetPos).magnitude;

        if (goingForward)
        {
            totalDistance += distance;
        }
        else if (!goingForward)
        {
            totalDistance -= distance;
        }

        trackingData.Add(new KeyValuePair<float, decimal>(Timer.timer, totalDistance));
        rayPos.Add(currentRayPos.ToString("F9"));
        rayDistanceToPrevPos.Add(distance);
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position.ToString("F9"));
        // distanceToLastTarget.Add(distanceToPrevTarget.ToString("F4"));
        rayDistanceToLastTarget.Add(prevRayDistance);
        rayDistanceToCurrentTarget.Add(rayDistanceToCurrTarget);
    }

    private void ResetCountdown()
    {
        countdown = DestructionTime;
    }
}
