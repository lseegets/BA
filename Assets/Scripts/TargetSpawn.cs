using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class TargetSpawn : MonoBehaviour
{
    private const float radiusPlayerDome = 5f;
    private const float radiusTargetSafeSpace = 2f;
    const float startElevation = 0.25f * Mathf.PI;
    const float startPolar = 0.5f * Mathf.PI;

    public int maxTargetCount;
    public int playerId;
    public string weightLevel;

    //!
    public Vector3 previousTargetPos;
    // !!
    public Vector3 normal;
    public Vector3 currentTargetPos;

    [SerializeField] GameObject target, planePrefab;
    [SerializeField] Text display;

    private GameObject currentTarget;
    private GameObject currentPlane;
    
    private Vector3 cameraPos;
    private Plane plane;

    private int currentTargetCount = 0;

    private float totalTime = 0;

    private CSVWriter csvWriter;
    private Plotter plotter;
    private Tracker tracker;
    private ViveTracker viveTracker;
    private LaserInput laserInput;
    private GameObject weight;

    float distanceToPlayer;
    float distanceToPrevTarget;

   

    // Start is called before the first frame update
    void Start()
    {
        plane = new Plane(Vector3.Cross(cameraPos, currentTargetPos - previousTargetPos), currentTargetPos);
        laserInput = GameObject.FindGameObjectsWithTag("Dumbbell")[0].GetComponentInChildren<LaserInput>();
        SpawnFirstTarget();
        csvWriter = new CSVWriter(playerId);
        //weight = GameObject.FindGameObjectsWithTag("Dumbbell")[0];
        //tracker = weight.transform.Find("TrackPoint").GetComponent<Tracker>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Timer.keepTiming) Timer.UpdateTimer();
        if (weight == null) weight = GameObject.FindGameObjectsWithTag("Dumbbell")[0];
        //if (tracker == null) tracker = weight.transform.Find("TrackPoint").GetComponent<Tracker>();
        // if (viveTracker == null) viveTracker = GameObject.Find("CameraRig.ViveTracker").GetComponent<ViveTracker>();
        if (viveTracker == null) viveTracker = transform.parent.Find("ViveTracker").GetComponent<ViveTracker>();
        Debug.Log("CURRENT TARGET: " + currentTargetPos);
    }

    public Vector3 ComputeTargetCenter()
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
        distanceToPlayer = Mathf.Abs(Vector3.Distance(nextTargetCenter, cameraPos));
        distanceToPrevTarget = Mathf.Abs(Vector3.Distance(nextTargetCenter, currentTargetPos));

        return nextTargetCenter;
    }

    public static void SphericalToCartesian(float radius, float elevation, float polar, out Vector3 outCart)
    {
        float a = radius * Mathf.Cos(elevation);
        outCart.x = a * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.z = a * Mathf.Sin(polar);
    }

    public void OnTargetDestroyed()
    {
        currentTargetCount++;
        PlayerData playerData = new PlayerData(playerId, weightLevel, Timer.timer);
        totalTime += Timer.timer;
        Timer.StopTimer();
        plotter = new Plotter(playerId, currentTargetCount);
        plotter.WriteCSV(viveTracker.trackingData, viveTracker.distanceToPrevPos, viveTracker.controllerPos, viveTracker.controllerRot, viveTracker.cameraPos, viveTracker.distanceToLastTarget, viveTracker.distanceToCurrentTarget, previousTargetPos, currentTargetPos, viveTracker.vectorX, viveTracker.vectorY, viveTracker.vectorZ);
       // plotter.WriteCSV2(viveTracker.trackingData2, viveTracker.distanceToPrevPos2, viveTracker.controllerPos2, viveTracker.controllerRot, viveTracker.cameraPos, viveTracker.distanceToLastTarget2, viveTracker.distanceToCurrentTarget2, previousTargetPos, currentTargetPos, viveTracker.vectorX2, viveTracker.vectorY2);
        plotter.WriteRayCSV(laserInput.trackingData, laserInput.rayDistanceToPrevPos, laserInput.rayPos, laserInput.cameraPos, laserInput.rayDistanceToLastTarget, laserInput.rayDistanceToCurrentTarget, previousTargetPos, currentTargetPos);
        // Differentiate(viveTracker.trackingData);
        //  Differentiate2(viveTracker.trackingData);
        ClearTrackingData();
        //ClearTrackingData2();
        ClearRayTrackingData();
        viveTracker.totalDistance = 0;
        viveTracker.totalDistance2 = 0;
        csvWriter.WriteCSV(playerData);
        SendPositionData();
        previousTargetPos = currentTargetPos;

        if (currentTargetCount < maxTargetCount) SpawnNextTarget();
        else if (currentTargetCount == maxTargetCount) display.text = "DONE \n" + "Total Time: " + totalTime.ToString("n2") + " seconds";
    }

    public void SendPositionData()
    {
        viveTracker.HandlePositionData(currentTargetPos, previousTargetPos);
        laserInput.HandlePositionData(currentTargetPos, previousTargetPos);
    }

    private void ClearTrackingData()
    {
        viveTracker.trackingData.Clear();
        viveTracker.controllerPos.Clear();
        viveTracker.controllerRot.Clear();
        viveTracker.cameraPos.Clear();
        viveTracker.distanceToPrevPos.Clear();
        viveTracker.vectorX.Clear();
        viveTracker.vectorY.Clear();
        viveTracker.vectorZ.Clear();
        viveTracker.distanceToLastTarget.Clear();
        viveTracker.distanceToCurrentTarget.Clear();
    }

    private void ClearTrackingData2()
    {
        viveTracker.trackingData2.Clear();
        viveTracker.controllerPos2.Clear();
        viveTracker.controllerRot.Clear();
        viveTracker.distanceToPrevPos2.Clear();
        viveTracker.vectorX2.Clear();
        viveTracker.vectorY2.Clear();
        viveTracker.distanceToLastTarget2.Clear();
        viveTracker.distanceToCurrentTarget2.Clear();
    }

    private void ClearRayTrackingData()
    {
        laserInput.trackingData.Clear();
        laserInput.rayPos.Clear();
        laserInput.cameraPos.Clear();
        laserInput.rayDistanceToPrevPos.Clear();
        laserInput.rayDistanceToLastTarget.Clear();
        laserInput.rayDistanceToCurrentTarget.Clear();
    }

    private void SpawnFirstTarget()
    {
        currentTarget = Instantiate(target);
        Vector3 firstTargetVector;
        SphericalToCartesian(radiusPlayerDome, startElevation, startPolar, out firstTargetVector);
        currentTarget.transform.position = currentTarget.transform.position + firstTargetVector;
        currentTargetPos = currentTarget.transform.position;
        previousTargetPos = currentTargetPos;
       // currentPlane = Instantiate(planePrefab, new Vector3(0, 0, 0), transform.rotation);

        cameraPos = transform.position;
        distanceToPlayer = Mathf.Abs(Vector3.Distance(currentTarget.transform.position, cameraPos));
        distanceToPrevTarget = 0;

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
        // currentPlane.transform.position = currentTargetPos;
        // currentPlane.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, previousTargetPos.y));

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

    private void Differentiate(List<KeyValuePair<float, float>> list)
    {
        int scale = 100;
        List<KeyValuePair<float, float>> vList = new();

        for (int i = 1; i < list.Count; i++)
        {
            float key;
            float value;

            key = list[i].Key;
            value = (list[i].Value - list[i - 1].Value) / scale;

            vList.Add(new KeyValuePair<float, float>(key, value));
        }

        string fileName = Application.dataPath + "/CSVFiles/VELOCITY" + playerId + "_"+ currentTargetCount + ".csv";
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;velocity");
        foreach (var item in vList)
        {
            writer.WriteLine(item.Key + ";" + item.Value);
        }
        writer.Close();
    }

    private void Differentiate2(List<KeyValuePair<float, float>> list)
    {
        int scale = 100;
        List<KeyValuePair<float, float>> vList = new();
        List<KeyValuePair<float, float>> aList = new();

        for (int i = 1; i < list.Count; i++)
        {
            float key;
            float value;

            key = list[i].Key;
            value = (list[i].Value - list[i - 1].Value) / scale;

            vList.Add(new KeyValuePair<float, float>(key, value));
        }

        for (int i = 1; i < vList.Count; i++)
        {
            float key;
            float value;

            key = vList[i].Key;
            value = (vList[i].Value - vList[i - 1].Value) / scale;

            aList.Add(new KeyValuePair<float, float>(key, value));
        }

        string fileName = Application.dataPath + "/CSVFiles/ACCELERATION" + playerId + "_" + currentTargetCount + ".csv";
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;acceleration");
        foreach (var item in aList)
        {
            writer.WriteLine(item.Key + ";" + item.Value);
        }
        writer.Close();
    }

    private void DifferentiateTest()
    {
        List<KeyValuePair<float, float>> vList = new();

        for (int i = 1; i < 100; i++)
        {
            float key;
            float value;

            key = i;
            value = (float) (Math.Sin(i) - Math.Sin(i-1)) / 100;

            vList.Add(new KeyValuePair<float, float>(key, value));
        }
        string fileName = Application.dataPath + "/CSVFiles/VELOCITYTEST" + currentTargetCount + ".csv";
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;velocity");
        foreach (var item in vList)
        {
            writer.WriteLine(item.Key + ";" + item.Value);
        }
        writer.Close();
    }
}
