using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Plotter : MonoBehaviour
{
    private int id;
    private int targetCount;
    //private string directoryName;
    private string fileName;
    private string fileName2;
    private string fileNameRay;

    public Plotter(int playerId, int count)
    {
        id = playerId;
        targetCount = count;
        //directoryName = Application.dataPath + "/CSVFiles/P" + id;
        fileName = Application.dataPath + "/CSVFiles/Player" + id + "_" + targetCount + "_plot.csv";
        fileName2 = Application.dataPath + "/CSVFiles/2DPlayer" + id + "_" + targetCount + "_plot.csv";
        fileNameRay = Application.dataPath + "/CSVFiles/Player" + id + "_" + targetCount + "_plot_RayData.csv";
        if (!File.Exists(fileName))
        {
            WriteHeader();
        }
        if (!File.Exists(fileName2))
        {
            WriteHeader2();
        }
        if (!File.Exists(fileNameRay))
        {
            WriteRayHeader();
        }
    }

    public void WriteCSV(List<KeyValuePair<float, /*float*/decimal>> dataList, List<decimal> distanceToPrevPos, List<string> controllerPos, List<Quaternion> rotation, List<string> cameraPos, List<decimal> distanceToLastTarget, List<float> distanceToCurrentTarget, Vector3 previousTarget, Vector3 currentTarget,/* string previousTarget, string currentTarget,*/ List<float> vectorX, List<float> vectorY, List<float> vectorZ)
    {
       // System.IO.Directory.CreateDirectory(directoryName);
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + distanceToPrevPos[i] + ";" + controllerPos[i] + ";" + rotation[i] + ";" + cameraPos[i] + ";" + distanceToLastTarget[i] + ";" + distanceToCurrentTarget[i] + ";" + ";" + ";" + vectorX[i] + ";" + vectorY[i] + ";" + vectorZ[i]);
        }

        writer.Close();
    }

    public void WriteRayCSV(List<KeyValuePair<float, decimal>> dataList, List<decimal> rayDistanceToPrevPos, List<string> rayPos, List<string> cameraPos, List<decimal> rayDistanceToLastTarget, List<float> rayDistanceToCurrentTarget, Vector3 previousTarget, Vector3 currentTarget)
    {
        TextWriter writer = new StreamWriter(fileNameRay, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + rayDistanceToPrevPos[i] + ";" + rayPos[i] + ";" + cameraPos[i] + ";" + rayDistanceToLastTarget[i] + ";" + rayDistanceToCurrentTarget[i]);
        }

        writer.Close();
    }

    public void WriteCSV2(List<KeyValuePair<float, /*float*/decimal>> dataList, List<decimal> distanceToPrevPos, List<string> controllerPos, List<Quaternion> rotation, List<string> cameraPos, List<decimal> distanceToLastTarget, List<float> distanceToCurrentTarget, Vector3 previousTarget, Vector3 currentTarget /*string previousTarget, string currentTarget*/, List<float> vectorX, List<float> vectorY)
    {
        TextWriter writer = new StreamWriter(fileName2, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + distanceToPrevPos[i] + ";" + controllerPos[i] + ";" + rotation[i] + ";" + cameraPos[i] + ";" + distanceToLastTarget[i] + ";" + distanceToCurrentTarget[i] + ";" + ";" + ";" + vectorX[i] + ";" + vectorY[i]);
        }

        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;total distance traveled;distance to prev position;controller position;controller rotation;camera position;distance to prev target;distance to current target;;rayPos;ray distance to last target;ray distance to current target;ray distance to prev pos");
        writer.Close();
    }
    private void WriteHeader2()
    {
        TextWriter writer = new StreamWriter(fileName2, true);
        writer.WriteLine("time;total distance traveled;distance to prev position;controller position;controller rotation;camera position;distance to prev target;distance to current target");
        writer.Close();
    }

    private void WriteRayHeader()
    {
        TextWriter writer = new StreamWriter(fileNameRay, true);
        writer.WriteLine("time;total distance traveled;distance to prev position;ray position;camera position;distance to prev target;distance to current target");
        writer.Close();
    }
}
