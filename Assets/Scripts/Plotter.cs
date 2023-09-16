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

    public void WriteCSV(List<KeyValuePair<float, decimal>> dataList, List<decimal> distanceToPrevPos, List<string> controllerPos, List<Vector3> rotation, List<string> cameraPos, List<decimal> distanceToLastTarget, List<float> distanceToCurrentTarget, string previousTarget, string currentTarget,/* string previousTarget, string currentTarget,*/ List<float> vectorX, List<float> vectorY, List<float> vectorZ)
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + distanceToPrevPos[i] + ";" + controllerPos[i] + ";" + rotation[i] + ";" + cameraPos[i] + ";" + distanceToLastTarget[i] + ";" + distanceToCurrentTarget[i] + ";" + ";" + ";" + vectorX[i] + ";" + vectorY[i] + ";" + vectorZ[i]);
        }

        writer.Close();
    }

    public void WriteRayCSV(List<KeyValuePair<float, decimal>> dataList, List<decimal> rayDistanceToPrevPos, List<string> rayPos, List<string> cameraPos, List<decimal> rayDistanceToLastTarget, List<float> rayDistanceToCurrentTarget, string previousTarget, string currentTarget, List<KeyValuePair<float, decimal>> dataList2, List<decimal> rayDistanceToPrevPos2, List<string> rayPos2, List<decimal> rayDistanceToLastTarget2, List<float> rayDistanceToCurrentTarget2, List<float> vectorX, List<float> vectorY, List<float> vectorZ)
    {
        TextWriter writer = new StreamWriter(fileNameRay, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + rayDistanceToPrevPos[i] + ";" + rayPos[i] + ";" + cameraPos[i] + ";" + rayDistanceToLastTarget[i] + ";" + rayDistanceToCurrentTarget[i] + ";;;" + dataList2[i].Key + ";" + dataList2[i].Value + ";" + rayDistanceToPrevPos2[i] + ";" + rayPos2[i] + ";" + rayDistanceToLastTarget2[i] + ";" + rayDistanceToCurrentTarget2[i] + ";" + ";" + ";" + vectorX[i] + ";" + vectorY[i] + ";" + vectorZ[i]);
        }

        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;total distance traveled;distance to prev position;controller position;controller rotation;camera position;distance to prev target;distance to current target;;rayPos;ray distance to last target;ray distance to current target;ray distance to prev pos");
        writer.Close();
    }

    private void WriteRayHeader()
    {
        TextWriter writer = new StreamWriter(fileNameRay, true);
        writer.WriteLine("time;total distance traveled;distance to prev position;ray position;camera position;distance to prev target;distance to current target;;;time;total distance traveled;distance to prev position;ray position;distance to prev target;distance to current target");
        writer.Close();
    }
}
