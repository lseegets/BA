using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

    public void WriteCSV(List<KeyValuePair<float, float>> dataList)
   // public void WriteCSV(List<float> dataList)
    {
        TextWriter writer = new StreamWriter(fileName, true);
        foreach (var item in dataList)
        {
            writer.WriteLine(item.Key + ";" + item.Value);
           // writer.WriteLine(item);
        }
        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("time;distance");
        writer.Close();
    }
}
