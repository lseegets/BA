using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class LaserInput : MonoBehaviour
{
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

    public float reactionTime = 0;
    public float reactionTimeDistance = 0;
    public float startReactionTime = 0;
    public float startReactionTimeDistance = 0;
    public bool trackedReactionTime = false;
    public bool movementStarted = false;
    public bool movementStartedDistance = false;
    public bool trackedReactionTimeDistance = false;
    public bool isOvershoot = false;
    public int frames = 0;
    public int timesTouched = 0;

    private const float DestructionTime = 0.2f;
    private const float maxReactionDistance = 0.04f;
    private const int maxFrames = 30; //20
    private const int tolerance = 2;
    private const decimal distanceThreshold = 0.01m;

    private Plane plane;
    private Vector3 currentRayPos;
    private Vector3 lastRayPos;
    private Vector3 currentTargetPos;
    private Vector3 previousTargetPos;
    private Vector3 reactionStartPos;

    private decimal prevRayDistance;
    private decimal currentRayDistance;
    private float countdown;
    private float rayDistanceToCurrTarget;
    private int currentTolerance = 0;
    private bool goingForward;

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        countdown = DestructionTime;

        lastRayPos = new Vector3(-0.000000155f, 3.535534000f, 3.535534000f);

        prevRayDistance = (decimal)Vector3.Distance(previousTargetPos, lastRayPos);

        rayDistanceToCurrTarget = Vector3.Distance(currentTargetPos, lastRayPos);
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
                currentRayPos = hitPoint;
                if (!movementStarted)
                {
                    reactionStartPos = hitPoint;
                    movementStarted = true;
                }
                CheckDistanceToTargets();
                CalculateDistance();
              //  CalculateReactionTime();
                
                lastRayPos = currentRayPos;
            }
            if (hit.collider.gameObject.CompareTag("Target"))
            {
                timesTouched++;
                if (timesTouched >= 2) isOvershoot = true;
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
    }

    private void CalculateReactionTime()
    {
        if (!trackedReactionTime)
        {
            if ((Vector3.Distance(reactionStartPos, currentTargetPos) - Vector3.Distance(currentRayPos, currentTargetPos) >= maxReactionDistance))
            {
                trackedReactionTime = true;
            }
        }

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
                else if ((decimal)Vector3.Distance(lastRayPos, currentRayPos) < distanceThreshold)
                {
                    if (currentTolerance <= tolerance)
                    {
                        currentTolerance++;
                        frames++;
                    }
                    else if (currentTolerance > tolerance)
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

        if (prevRayDistance <= currentRayDistance)
        {
            goingForward = true;
            //CalculateReactionTime();
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

        if (goingForward)
        {
            CalculateReactionTime();
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
        rayDistanceToLastTarget.Add(prevRayDistance);
        rayDistanceToCurrentTarget.Add(rayDistanceToCurrTarget);

        vectorX.Add(currentRayPos.x);
        vectorY.Add(currentRayPos.y);
        vectorZ.Add(currentRayPos.z);
    }

    private void ResetCountdown()
    {
        countdown = DestructionTime;
    }
}
