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
        if (!File.Exists(fileName))
        {
            WriteHeader();
        }
    }

    public void WriteCSV(PlayerData playerData)
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(playerData.id + ";" + playerData.level + ";" + playerData.time);
        writer.Close();
    }

    private void WriteHeader()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("id;weight level;time");
        writer.Close();
    }
}
