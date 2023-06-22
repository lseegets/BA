using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSpawn : MonoBehaviour
{
    private const float radiusPlayerDome = 5f;
    private const float radiusTargetSafeSpace = 2f;
    const float startElevation = 0.25f * Mathf.PI;
    const float startPolar = 0.5f * Mathf.PI;

    public int maxTargetCount;
    public int playerId;
    public int playerTrial;

    [SerializeField] GameObject target;
    [SerializeField] Text timeDisplay;

    private GameObject currentTarget;
    private Vector3 currentTargetPos;
    private Vector3 cameraPos;

    private int currentTargetCount = 0;

    private CSVWriter csvWriter;

    float distanceToPlayer;
    float distanceToPrevTarget;

    // Start is called before the first frame update
    void Start()
    {
        SpawnFirstTarget();
        csvWriter = new CSVWriter(playerId);
    }

    // Update is called once per frame
    void Update()
    {
        if (Timer.keepTiming) Timer.UpdateTimer();
        Debug.Log(Timer.timer);

        if ((currentTarget == null) && (currentTargetCount < maxTargetCount)) SpawnNextTarget();

       // else if (currentTargetCount == maxTargetCount) timeDisplay.text = currentTargetCount.ToString();
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
        timeDisplay.text += currentTargetCount.ToString() + ": " + Timer.timer.ToString() + ";\n";
        PlayerData playerData = new PlayerData(playerId, playerTrial, Timer.timer);
        Timer.StopTimer();
        csvWriter.WriteCSV(playerData);
    }

    private void SpawnFirstTarget()
    {
        currentTarget = Instantiate(target);
        Vector3 firstTargetVector;
        SphericalToCartesian(radiusPlayerDome, startElevation, startPolar, out firstTargetVector);
        currentTarget.transform.position = currentTarget.transform.position + firstTargetVector;
        currentTargetPos = currentTarget.transform.position;

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

        Timer.StartTimer();
    }
}
