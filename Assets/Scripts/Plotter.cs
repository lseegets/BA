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

    public Plotter(int playerId, int count)
    {
        id = playerId;
        targetCount = count;
        //directoryName = Application.dataPath + "/CSVFiles/P" + id;
        fileName = Application.dataPath + "/CSVFiles/Player" + id + "_" + targetCount + "_plot.csv";
        fileName2 = Application.dataPath + "/CSVFiles/2DPlayer" + id + "_" + targetCount + "_plot.csv";
        if (!File.Exists(fileName))
        {
            WriteHeader();
        }
        if (!File.Exists(fileName2))
        {
            WriteHeader2();
        }
    }

    public void WriteCSV(List<KeyValuePair<float, /*float*/decimal>> dataList, List<decimal> distanceToPrevPos, List<string> controllerPos, List<string> cameraPos, List<decimal> distanceToLastTarget, List<float> distanceToCurrentTarget, Vector3 previousTarget, Vector3 currentTarget,/* string previousTarget, string currentTarget,*/ List<float> vectorX, List<float> vectorY, List<float> vectorZ)
    {
       // System.IO.Directory.CreateDirectory(directoryName);
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + distanceToPrevPos[i] + ";" + controllerPos[i] + ";" +  cameraPos[i] + ";" + distanceToLastTarget[i] + ";" + distanceToCurrentTarget[i] + ";" + ";" + ";" + vectorX[i] + ";" + vectorY[i] + ";" /*+ vectorZ[i]*/);
        }

        writer.Close();
    }

    public void WriteCSV2(List<KeyValuePair<float, /*float*/decimal>> dataList, List<decimal> distanceToPrevPos, List<string> controllerPos, List<string> cameraPos, List<decimal> distanceToLastTarget, List<float> distanceToCurrentTarget, Vector3 previousTarget, Vector3 currentTarget /*string previousTarget, string currentTarget*/, List<float> vectorX, List<float> vectorY)
    {
        // System.IO.Directory.CreateDirectory(directoryName);
        TextWriter writer = new StreamWriter(fileName2, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + distanceToPrevPos[i] + ";" + controllerPos[i] + ";" + cameraPos[i] + ";" + distanceToLastTarget[i] + ";" + distanceToCurrentTarget[i] + ";" + ";" + ";" + vectorX[i] + ";" + vectorY[i]);
        }

        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;distance traveled;distance to prev position;controller position;camera position;distance to prev target;distance to current target");
        writer.Close();
    }
    private void WriteHeader2()
    {
        TextWriter writer = new StreamWriter(fileName2, true);
        writer.WriteLine("time;distance traveled;distance to prev position;controller position;camera position;distance to prev target;distance to current target");
        writer.Close();
    }
}
