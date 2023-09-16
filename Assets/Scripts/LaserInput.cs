using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInput : MonoBehaviour
{
    [SerializeField] GameObject cube;

    public static GameObject currentObject;
    public List<KeyValuePair<float, decimal>> trackingData = new();
    public List<string> cameraPos = new();

    public List<float> vectorX = new();
    public List<float> vectorY = new();
    public List<float> vectorZ = new();

    public List<string> rayPos = new();
    public List<decimal> rayDistanceToLastTarget = new();
    public List<float> rayDistanceToCurrentTarget = new();
    public List<decimal> rayDistanceToPrevPos = new();
    public decimal totalDistance = 0;
    public decimal totalDistance2 = 0;

    public Material activatedMaterial;
    public Material defaultMaterial;

    private const float DestructionTime = 0.2f;

    private float countdown;
    private Plane plane;
    private Vector3 currentRayPos;
    private Vector2 currentRayPos2;
    private Vector3 lastRayPos;
    private Vector2 lastRayPos2;
    private Vector3 currentTargetPos;
    private Vector2 currentTargetPos2;
    private Vector3 previousTargetPos;    
    private Vector2 previousTargetPos2;    

    private decimal prevRayDistance;
    private decimal prevRayDistance2;
    private decimal currentRayDistance;
    private decimal currentRayDistance2;
    private float rayDistanceToCurrTarget;
    private float rayDistanceToCurrTarget2;
    private bool goingForward;
    private bool goingForward2;

    public List<KeyValuePair<float, decimal>> trackingData2 = new();
    public List<string> rayPos2 = new();
    public List<decimal> rayDistanceToPrevPos2 = new();
    public List<decimal> rayDistanceToLastTarget2 = new();
    public List<float> rayDistanceToCurrentTarget2 = new();

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        countdown = DestructionTime;

        lastRayPos = new Vector3(0, 0, 0);
        lastRayPos2 = new Vector2(0, 0);

        prevRayDistance = (decimal)Vector3.Distance(previousTargetPos, lastRayPos);
        prevRayDistance2 = (decimal)Vector2.Distance(previousTargetPos2, lastRayPos2);

        rayDistanceToCurrTarget = Vector3.Distance(currentTargetPos, lastRayPos);
        rayDistanceToCurrTarget2 = Vector2.Distance(currentTargetPos2, lastRayPos2);
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
                Vector2 hitPoint2 = ray.GetPoint(enter);
                currentRayPos = hitPoint;
                currentRayPos2 = hitPoint2;
                CheckDistanceToTargets();
                CalculateDistance();
               // cube.transform.position = hitPoint;
                //SendRayData(currentRayPos, lastRayPos);
                lastRayPos = currentRayPos;
                lastRayPos2 = currentRayPos2;
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

        currentTargetPos2 = currentTarget;
        previousTargetPos2 = previousTarget;
    }

    private void CheckDistanceToTargets()
    {
        currentRayDistance = (decimal)Vector3.Distance(previousTargetPos, currentRayPos);
        currentRayDistance2 = (decimal)Vector2.Distance(previousTargetPos2, currentRayPos2);

        if (prevRayDistance <= currentRayDistance)
        {
            goingForward = true;
        }
        else if (prevRayDistance > currentRayDistance)
        {
            goingForward = false;
        }

        if (prevRayDistance2 <= currentRayDistance2)
        {
            goingForward2 = true;
        }
        else if (prevRayDistance2 > currentRayDistance2)
        {
            goingForward2 = false;
        }

        prevRayDistance = currentRayDistance;
        prevRayDistance2 = currentRayDistance2;
        rayDistanceToCurrTarget = Vector3.Distance(currentTargetPos, currentRayPos);
        rayDistanceToCurrTarget2 = Vector2.Distance(currentTargetPos2, currentRayPos2);
    }

    private void CalculateDistance()
    {
        decimal distance = (decimal)Vector3.Distance(currentRayPos, lastRayPos);
        decimal distance2 = (decimal)Vector2.Distance(currentRayPos2, lastRayPos2);

        //  distanceToPrevTarget = (currentPos - previousTargetPos).magnitude;

        if (goingForward)
        {
            totalDistance += distance;
        }
        else if (!goingForward)
        {
            totalDistance -= distance;
        }

        if (goingForward2)
        {
            totalDistance2 += distance2;
        }
        else if (!goingForward2)
        {
            totalDistance2 -= distance2;
        }

        trackingData.Add(new KeyValuePair<float, decimal>(Timer.timer, totalDistance));
        rayPos.Add(currentRayPos.ToString("F9"));
        rayDistanceToPrevPos.Add(distance);
        cameraPos.Add(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform.position.ToString("F9"));
        // distanceToLastTarget.Add(distanceToPrevTarget.ToString("F4"));
        rayDistanceToLastTarget.Add(prevRayDistance);
        rayDistanceToCurrentTarget.Add(rayDistanceToCurrTarget);

        trackingData2.Add(new KeyValuePair<float, decimal>(Timer.timer, totalDistance2));
        rayPos2.Add(currentRayPos2.ToString("F9"));
        rayDistanceToPrevPos2.Add(distance2);
        // distanceToLastTarget.Add(distanceToPrevTarget.ToString("F4"));
        rayDistanceToLastTarget2.Add(prevRayDistance2);
        rayDistanceToCurrentTarget2.Add(rayDistanceToCurrTarget2);

        vectorX.Add(currentRayPos.x);
        vectorY.Add(currentRayPos.y);
        vectorZ.Add(currentRayPos.z);
    }

    private void ResetCountdown()
    {
        countdown = DestructionTime;
    }
}
