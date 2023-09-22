using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter : MonoBehaviour
{
    private int id;
    private string fileName;

    public CSVWriter(int playerId)
    {
        id = playerId;
        fileName = Application.dataPath + "/CSVFiles/Player_" + id + ".csv";
        if (!File.Exists(fileName)) WriteHeader();
    }

    public void WriteCSV(PlayerData playerData)
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(playerData.id + ";" + playerData.targetNumber + ";" + playerData.level + ";" + playerData.time + ";" + playerData.speed);
        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("id;target number;weight level;time;max speed");
        writer.Close();
    }
}
