using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static bool keepTiming = false;
    public static float timer = 0f;

    public static void StartTimer()
    {
        keepTiming = true;
    }

    public static void StopTimer()
    {
        keepTiming = false;
        timer = 0;
    }

    public static void UpdateTimer()
    {
        timer += Time.deltaTime;
    }
}

