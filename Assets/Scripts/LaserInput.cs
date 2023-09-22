using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LaserInput : MonoBehaviour
{
    [SerializeField] GameObject cube;
    [SerializeField] TMPro.TextMeshProUGUI speedTest;
    [SerializeField] public Material activatedMaterial;
    [SerializeField] Material defaultMaterial;

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

    public float reactionTime;
    public float reactionTimeDistance;

    // public Material activatedMaterial;
    // public Material defaultMaterial;

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

    private float maxReactionTime = 0.6f;
    private float currentReactionTime = 0;
    public float startReactionTime = 0;
    public float startReactionTimeDistance = 0;
    private float maxReactionSpeed = 0.6f;
    private decimal maxReactionDistance = 0.1m;
    private decimal currentReactionDistance = 0;

    public List<KeyValuePair<float, decimal>> trackingData2 = new();
    public List<string> rayPos2 = new();
    public List<decimal> rayDistanceToPrevPos2 = new();
    public List<decimal> rayDistanceToLastTarget2 = new();
    public List<float> rayDistanceToCurrentTarget2 = new();

    public bool trackedReactionTime = false;
    public bool movementStarted = false;
    public bool movementStartedDistance = false;
    public bool trackedReactionTimeDistance = false;
    public float maxSpeed = 0;
    public int frames = 0;
    private int maxFrames = 30;
    private decimal distanceThreshold = 0.009m; //try 0.01 again
    private int tolerance = 2;
    private int currentTolerance = 0;

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

    void FixedUpdate()
    {
        if (!trackedReactionTime) reactionTime += Time.deltaTime;
        if (!trackedReactionTimeDistance) reactionTimeDistance += Time.deltaTime;

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
              //  CalculateReactionTime();
                
                lastRayPos = currentRayPos;
                lastRayPos2 = currentRayPos2;
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
    }

    public void HandlePositionData(Vector3 currentTarget, Vector3 previousTarget)
    {
        currentTargetPos = currentTarget;
        previousTargetPos = previousTarget;

        currentTargetPos2 = currentTarget;
        previousTargetPos2 = previousTarget;
    }

    private void CalculateReactionTime()
    {
        float speed = Vector3.Distance(lastRayPos, currentRayPos) / Time.deltaTime;

        if (speed > maxSpeed) maxSpeed = speed;

        speedTest.text = "SPEED: " + speed.ToString("F2");

        if (!trackedReactionTime)
        {
            if (speed >= maxReactionSpeed)
            {
                if (!movementStarted)
                {
                    startReactionTime = reactionTime;
                    movementStarted = true;
                }
                currentReactionTime += Time.deltaTime;
                if (currentReactionTime >= maxReactionTime)
                {
                    trackedReactionTime = true;
                    //reactionTime = startReactionTime;
                }
            }
            else
            {
                currentReactionTime = 0;
                startReactionTime = 0;
                movementStarted = false;
            }
        }

        /*if (!trackedReactionTimeDistance)
        {
            currentReactionDistance += (decimal)Vector3.Distance(lastRayPos, currentRayPos);
            if (currentReactionDistance >= maxReactionDistance)
            {
                trackedReactionTimeDistance = true;
            }
        }*/
        if (!trackedReactionTimeDistance)
        {
            if (frames < maxFrames)
            {
                if ((decimal)Vector3.Distance(lastRayPos, currentRayPos) >= distanceThreshold)
                {
                    frames++;
                    if (!movementStartedDistance)
                    {
                        startReactionTimeDistance = reactionTimeDistance;
                        movementStartedDistance = true;
                    }
                }
                else
                {
                    if (currentTolerance < tolerance)
                    {
                        currentTolerance++;
                        frames++;
                        if (!movementStartedDistance)
                        {
                            startReactionTimeDistance = reactionTimeDistance;
                            movementStartedDistance = true;
                        }
                    }
                    else if (currentTolerance >= tolerance)
                    {
                        frames = 0;
                        movementStartedDistance = false;
                        startReactionTimeDistance = 0;
                        currentTolerance = 0;
                    }
                }
            }
            else if (frames >= maxFrames)
            {
                trackedReactionTimeDistance = true;
                currentTolerance = 0;
            }
        }

    }

    private void CheckDistanceToTargets()
    {
        currentRayDistance = (decimal)Vector3.Distance(previousTargetPos, currentRayPos);
        currentRayDistance2 = (decimal)Vector2.Distance(previousTargetPos2, currentRayPos2);

        if (prevRayDistance <= currentRayDistance)
        {
            goingForward = true;
            //CalculateReactionTime();
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

        if (goingForward)
        {
            CalculateReactionTime();
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
        rayDistanceToLastTarget.Add(prevRayDistance);
        rayDistanceToCurrentTarget.Add(rayDistanceToCurrTarget);

        trackingData2.Add(new KeyValuePair<float, decimal>(Timer.timer, totalDistance2));
        rayPos2.Add(currentRayPos2.ToString("F9"));
        rayDistanceToPrevPos2.Add(distance2);
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
