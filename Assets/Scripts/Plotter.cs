using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Plotter : MonoBehaviour
{
    private int id;
    private int targetCount;
    private string fileName;
    private string fileNameRay;

    public Plotter(int playerId, int count)
    {
        id = playerId;
        targetCount = count;
        fileName = Application.dataPath + "/CSVFiles/Player" + id + "_" + targetCount + "_plot.csv";
        fileNameRay = Application.dataPath + "/CSVFiles/Player" + id + "_" + targetCount + "_plot_RayData.csv";
        if (!File.Exists(fileName)) WriteHeader();
        if (!File.Exists(fileNameRay)) WriteRayHeader();
    }

    public void WriteCSV(List<KeyValuePair<float, decimal>> dataList, List<decimal> distanceToPrevPos, List<string> controllerPos, List<Vector3> rotation, List<string> cameraPos, List<string> trackerDistanceToHmd, List<decimal> distanceToLastTarget, List<float> distanceToCurrentTarget, string previousTarget, string currentTarget,/* string previousTarget, string currentTarget,*/ List<float> vectorX, List<float> vectorY, List<float> vectorZ)
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + distanceToPrevPos[i] + ";" + controllerPos[i] + ";" + rotation[i] + ";" + cameraPos[i] + ";" + trackerDistanceToHmd[i] + ";" + distanceToLastTarget[i] + ";" + distanceToCurrentTarget[i] + ";" + ";" + ";" + vectorX[i] + ";" + vectorY[i] + ";" + vectorZ[i]);
        }

        writer.Close();
    }

    public void WriteRayCSV(List<KeyValuePair<float, decimal>> dataList, List<decimal> rayDistanceToPrevPos, List<string> rayPos, List<string> cameraPos, List<decimal> rayDistanceToLastTarget, List<float> rayDistanceToCurrentTarget, string previousTarget, string currentTarget, float reactionTime, float reactionTimeDistance, List<float> vectorX, List<float> vectorY, List<float> vectorZ)
    {
        TextWriter writer = new StreamWriter(fileNameRay, true);
        writer.WriteLine(previousTarget + ";" + currentTarget + ";" + reactionTime + ";" + reactionTimeDistance);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + rayDistanceToPrevPos[i] + ";" + rayPos[i] + ";" + cameraPos[i] + ";" + rayDistanceToLastTarget[i] + ";" + rayDistanceToCurrentTarget[i] + ";;;" + vectorX[i] + ";" + vectorY[i] + ";" + vectorZ[i]);
        }

        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;total distance traveled;distance to prev position;controller position;controller rotation;camera position;tracker distance to hmd;distance to prev target;distance to current target");
        writer.Close();
    }

    private void WriteRayHeader()
    {
        TextWriter writer = new StreamWriter(fileNameRay, true);
        writer.WriteLine("time;total distance traveled;distance to prev position;ray position;hmd position;distance to prev target;distance to current target");
        writer.Close();
    }
}
