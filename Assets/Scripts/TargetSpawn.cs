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

    private Vector3 previousTargetPos;

    [SerializeField] GameObject target;
    [SerializeField] Text display;

    private GameObject currentTarget;
    private Vector3 currentTargetPos;
    private Vector3 cameraPos;

    // !!!
    public int currentTargetCount = 0;
    // !!!

    private float totalTime = 0;

    private CSVWriter csvWriter;
    private Plotter plotter;
    private Tracker tracker;
    private ViveTracker viveTracker;
    private GameObject weight;

    float distanceToPlayer;
    float distanceToPrevTarget;

    // Start is called before the first frame update
    void Start()
    {
        SpawnFirstTarget();
        csvWriter = new CSVWriter(playerId);
        //weight = GameObject.FindGameObjectsWithTag("Dumbbell")[0];
        //tracker = weight.transform.Find("TrackPoint").GetComponent<Tracker>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer.keepTiming) Timer.UpdateTimer();
        if (weight == null) weight = GameObject.FindGameObjectsWithTag("Dumbbell")[0];
        //if (tracker == null) tracker = weight.transform.Find("TrackPoint").GetComponent<Tracker>();
        // if (viveTracker == null) viveTracker = GameObject.Find("CameraRig.ViveTracker").GetComponent<ViveTracker>();
        if (viveTracker == null) viveTracker = transform.parent.Find("ViveTracker").GetComponent<ViveTracker>();
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
        plotter.WriteCSV(viveTracker.trackingData, viveTracker.controllerPos, viveTracker.cameraPos, viveTracker.distanceToLastTarget, viveTracker.distanceToCurrentTarget, previousTargetPos.ToString("F4"), currentTargetPos.ToString("F4"));
        Differentiate(viveTracker.trackingData);
        Differentiate2(viveTracker.trackingData);
        viveTracker.trackingData.Clear();
        viveTracker.controllerPos.Clear();
        viveTracker.cameraPos.Clear();
        viveTracker.distanceToLastTarget.Clear();
        viveTracker.distanceToCurrentTarget.Clear();
        viveTracker.totalDistance = 0;
        csvWriter.WriteCSV(playerData);
       // SendPositionData();
        previousTargetPos = currentTargetPos;

        if (currentTargetCount < maxTargetCount) SpawnNextTarget();
        else if (currentTargetCount == maxTargetCount) display.text = "DONE \n" + "Total Time: " + totalTime.ToString("n2") + " seconds";
    }

    public void SendPositionData()
    {
        viveTracker.HandlePositionData(currentTargetPos, previousTargetPos);
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

        SendPositionData();

        Timer.StartTimer();
    }

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
