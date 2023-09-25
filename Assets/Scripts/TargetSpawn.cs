using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class TargetSpawn : MonoBehaviour
{
    public int playerId;

    public Vector3 previousTargetPos;
    public Vector3 normal;
    public Vector3 currentTargetPos;
    public enum Mode
    {
        testRun,
        serious
    }
    public Mode mode = new();

    [SerializeField] GameObject target;
    [SerializeField] Text display;

    private const float radiusPlayerDome = 5f;
    private const float radiusTargetSafeSpace = 2f;
    private const float startElevation = 0.25f * Mathf.PI;
    private const float startPolar = 0.5f * Mathf.PI;
    private const int testRunTargetCount = 10;
    private const int seriousTargetCount = 51;

    private GameObject currentTarget;
    private Vector3 cameraPos;
    private Plane plane;

    private CSVWriter csvWriter;
    private Plotter plotter;
    private ViveTracker viveTracker;
    private LaserInput laserInput;

    private int currentTargetCount = 0;
    private int maxTargetCount;
    private float totalTime = 0;

    private WeightManager.WeightLevel weightLevel;

    // Start is called before the first frame update
    void Start()
    {
        switch (mode)
        {
            case Mode.testRun:
                maxTargetCount = testRunTargetCount;
                break;
            case Mode.serious:
                maxTargetCount = seriousTargetCount;
                break;
        }

        plane = new Plane(Vector3.Cross(cameraPos, currentTargetPos - previousTargetPos), currentTargetPos);
       // laserInput = GameObject.FindGameObjectsWithTag("Dumbbell")[0].GetComponentInChildren<LaserInput>();
        viveTracker = transform.parent.Find("ViveTracker").GetComponent<ViveTracker>();
        SpawnFirstTarget();
        csvWriter = new CSVWriter(playerId);
        display.text = "Point at the target to start";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Timer.keepTiming) Timer.UpdateTimer();
        //if (viveTracker == null) viveTracker = transform.parent.Find("ViveTracker").GetComponent<ViveTracker>();
        if (laserInput == null) laserInput = GameObject.FindGameObjectsWithTag("Dumbbell")[0].GetComponentInChildren<LaserInput>();
    }

    public void OnTargetDestroyed()
    {
        currentTargetCount++;
        PlayerData playerData = new PlayerData(playerId, currentTargetCount, weightLevel, Timer.timer, laserInput.maxSpeed);
        totalTime += Timer.timer;
        Timer.StopTimer();
        plotter = new Plotter(playerId, currentTargetCount);
        plotter.WriteCSV(viveTracker.trackingData, viveTracker.distanceToPrevPos, viveTracker.controllerPos, viveTracker.controllerRot, viveTracker.cameraPos, viveTracker.trackerDistanceToHmd, viveTracker.distanceToLastTarget, viveTracker.distanceToCurrentTarget, previousTargetPos.ToString("F9"), currentTargetPos.ToString("F9"), viveTracker.vectorX, viveTracker.vectorY, viveTracker.vectorZ);
        plotter.WriteRayCSV(laserInput.trackingData, laserInput.rayDistanceToPrevPos, laserInput.rayPos, laserInput.cameraPos, laserInput.rayDistanceToLastTarget, laserInput.rayDistanceToCurrentTarget, previousTargetPos.ToString("F9"), currentTargetPos.ToString("F9"), laserInput.startReactionTime, laserInput.startReactionTimeDistance, laserInput.trackingData2, laserInput.rayDistanceToPrevPos2, laserInput.rayPos2, laserInput.rayDistanceToLastTarget2, laserInput.rayDistanceToCurrentTarget2, laserInput.vectorX, laserInput.vectorY, laserInput.vectorZ);
        ClearTrackingData();
        laserInput.totalDistance = 0;
        laserInput.totalDistance2 = 0;
        viveTracker.totalDistance = 0;
        csvWriter.WriteCSV(playerData);
        SendPositionData();
        previousTargetPos = currentTargetPos;

        if (currentTargetCount < maxTargetCount)
        {
            display.text = "";
            SpawnNextTarget();
        }
        else if (currentTargetCount == maxTargetCount) display.text = "DONE! \n" + "Total Time: " + totalTime.ToString("n2") + " seconds \n\n\n" +
        "What level of exertion did you experience on a scale of \n\n 6 (no exertion/relaxed) to 20 (maximal exertion)?";
    }

    public void SendPositionData()
    {
        viveTracker.HandlePositionData(currentTargetPos, previousTargetPos);
        laserInput.HandlePositionData(currentTargetPos, previousTargetPos);
    }

    public void GetWeightData(WeightManager.WeightLevel level)
    {
        weightLevel = level;
    }
    private Vector3 ComputeTargetCenter()
    {
        float distance = Mathf.Abs(Vector3.Distance(currentTargetPos, cameraPos));

        float scalingOnC1C2 = 0.5f + (radiusPlayerDome * radiusPlayerDome - radiusTargetSafeSpace * radiusTargetSafeSpace) / (2f * distance * distance);

        Vector3 intersectionCircleCenter = cameraPos + scalingOnC1C2 * (currentTargetPos - cameraPos); // Center of the cirlce of the intersection between radiusPlayerDome and radiusTargetSafeSpace

        float betweenResult = radiusPlayerDome * radiusPlayerDome - scalingOnC1C2 * scalingOnC1C2 * distance * distance;

        float intersectionCircleRadius = Mathf.Sqrt(Mathf.Abs(betweenResult));

        Vector3 n_i = (currentTargetPos - cameraPos) / distance;

        Vector3 x_vector = new Vector3(1, 0, 0);
        Vector3 y_vector = new Vector3(0, 1, 0);

        Vector3 t_i = Vector3.Normalize(Vector3.Cross(x_vector, n_i));
        if (t_i.x == 0 && t_i.y == 0 && t_i.z == 0)
        {
            t_i = Vector3.Normalize(Vector3.Cross(y_vector, n_i));
        }

        Vector3 b_i = Vector3.Cross(t_i, n_i);

        float randomAngle = UnityEngine.Random.Range(0.0f, 1.0f) * 2.0f * Mathf.PI;

        Vector3 nextTargetCenter = intersectionCircleCenter + intersectionCircleRadius * (t_i * Mathf.Cos(randomAngle) + b_i * Mathf.Sin(randomAngle));

        while (nextTargetCenter.y < 0 || nextTargetCenter.z < 2 || nextTargetCenter.y > 3.4)
        {
            randomAngle = UnityEngine.Random.Range(0.0f, 1.0f) * 2.0f * Mathf.PI;
            nextTargetCenter = intersectionCircleCenter + intersectionCircleRadius * (t_i * Mathf.Cos(randomAngle) + b_i * Mathf.Sin(randomAngle));
        }

        return nextTargetCenter;
    }

    private void SphericalToCartesian(float radius, float elevation, float polar, out Vector3 outCart)
    {
        float a = radius * Mathf.Cos(elevation);
        outCart.x = a * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.z = a * Mathf.Sin(polar);
    }

    private void ClearTrackingData()
    {
        viveTracker.trackingData.Clear();
        viveTracker.controllerPos.Clear();
        viveTracker.controllerRot.Clear();
        viveTracker.cameraPos.Clear();
        viveTracker.trackerDistanceToHmd.Clear();
        viveTracker.distanceToPrevPos.Clear();
        viveTracker.vectorX.Clear();
        viveTracker.vectorY.Clear();
        viveTracker.vectorZ.Clear();
        viveTracker.distanceToLastTarget.Clear();
        viveTracker.distanceToCurrentTarget.Clear();

        laserInput.trackingData.Clear();
        laserInput.rayPos.Clear();
        laserInput.cameraPos.Clear();
        laserInput.rayDistanceToPrevPos.Clear();
        laserInput.rayDistanceToLastTarget.Clear();
        laserInput.rayDistanceToCurrentTarget.Clear();
        laserInput.vectorX.Clear();
        laserInput.vectorY.Clear();
        laserInput.vectorZ.Clear();
        laserInput.reactionTime = 0;
        laserInput.startReactionTime = 0;
        laserInput.startReactionTimeDistance = 0;
        laserInput.reactionTimeDistance = 0;
        laserInput.trackedReactionTime = false;
        laserInput.trackedReactionTimeDistance = false;
        laserInput.movementStarted = false;
        laserInput.movementStartedDistance = false;
        laserInput.maxSpeed = 0;
        laserInput.frames = 0;

        laserInput.trackingData2.Clear();
        laserInput.rayPos2.Clear();
        laserInput.rayDistanceToPrevPos2.Clear();
        laserInput.rayDistanceToLastTarget2.Clear();
        laserInput.rayDistanceToCurrentTarget2.Clear();
    }

    private void SpawnFirstTarget()
    {
        currentTarget = Instantiate(target);
        Vector3 firstTargetVector;
        SphericalToCartesian(radiusPlayerDome, startElevation, startPolar, out firstTargetVector);
        currentTarget.transform.position = currentTarget.transform.position + firstTargetVector;
        currentTargetPos = currentTarget.transform.position;
        previousTargetPos = currentTargetPos;

        cameraPos = transform.position;

        Timer.StartTimer();
    }

    private void SpawnNextTarget()
    {
        Vector3 targetCenter = ComputeTargetCenter();
        currentTarget = Instantiate(target);
        currentTarget.transform.position = targetCenter;
        cameraPos = transform.position;
        currentTargetPos = currentTarget.transform.position;
        CreatePlane();
        SendPositionData();

        Timer.StartTimer();
    }

    private void CreatePlane()
    {
        normal = Vector3.Cross(currentTargetPos - previousTargetPos, cameraPos).normalized;
        plane.SetNormalAndPosition(normal, currentTargetPos);
        SendPlaneData(plane);
       // OnDrawGizmos();
    }

    public void SendPlaneData(Plane plane)
    {
        laserInput.GetPlaneData(plane);
    }

    /*void OnDrawGizmos()
    {
        //plane = new Plane(normal, currentTargetPos);

        // Draw our three input points in world space.
        // b and c are drawn as lollipops from the preceding point,
        // so that you can see the clockwise winding direction.

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(currentTargetPos, 0.1f);

        Gizmos.color = Color.gray;
        Gizmos.DrawLine(currentTargetPos, previousTargetPos);
        Gizmos.DrawWireSphere(previousTargetPos, 0.1f);

        Gizmos.color = Color.black;
        Gizmos.DrawLine(previousTargetPos, normal);
        Gizmos.DrawWireSphere(normal, 0.1f);

        // Draw this object's position, 
        // as a lollipop sticking out from our plane,
        // blue-green if in front (in the positive half-space),
        // and red if behind (negative half-space).           
        Gizmos.color = plane.GetSide(transform.position) ? Color.cyan : Color.red;
        Gizmos.DrawLine(plane.ClosestPointOnPlane(transform.position), transform.position);
        Gizmos.DrawWireSphere(transform.position, 0.2f);

        // Draw plane normal.
        Gizmos.color = Color.yellow;
        var center = (currentTargetPos + previousTargetPos + normal) / 3f;
        Gizmos.DrawLine(center, center + plane.normal);

        // Draw planar grid.
        Gizmos.color = Color.blue;
        var matrix = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(center, Quaternion.LookRotation(plane.normal), Vector3.one);
        for (int i = -10; i <= 10; i++)
        {
            Gizmos.DrawLine(new Vector3(i, -10, 0), new Vector3(i, 10, 0));
            Gizmos.DrawLine(new Vector3(-10, i, 0), new Vector3(10, i, 0));
        }
        Gizmos.matrix = matrix;
    }*/
}
