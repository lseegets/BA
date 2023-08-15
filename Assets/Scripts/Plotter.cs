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

    public Plotter(int playerId, int count)
    {
        id = playerId;
        targetCount = count;
        fileName = Application.dataPath + "/CSVFiles/Player" + id + "_" + targetCount + "_plot.csv";
        if (!File.Exists(fileName))
        {
            WriteHeader();
        }
    }

    public void WriteCSV(List<KeyValuePair<float, float>> dataList, List<Vector3> controllerPos, List<Vector3> cameraPos, Vector3 previousTarget, Vector3 currentTarget)
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(previousTarget + ";" + currentTarget);
        writer.WriteLine();

        for (int i = 0; i < dataList.Count; i++)
        {
            writer.WriteLine(dataList[i].Key + ";" + dataList[i].Value + ";" + controllerPos[i] + ";" +  cameraPos[i]);
        }

        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;distance;controller position;camera position");
        writer.Close();
    }
}
