using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVWriter : MonoBehaviour
{
    private int id;
    private string fileName;
    //private string fileName = Application.dataPath + "/CSVFiles/Player_" + id + ".csv";

    public CSVWriter(int playerId)
    {
        id = playerId;
        fileName = Application.dataPath + "/CSVFiles/Player_" + id + ".csv";

        WriteHeading();
    }

    // Start is called before the first frame update
    void Start()
    {
        //fileName = Application.dataPath + "/testCSV.csv";
        
    }

    private void WriteHeading()
    {
        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("id;trial;time");
        writer.Close();
    }

    public void WriteCSV(PlayerData playerData)
    {
       /* TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine("id,trial,time");
        writer.Close();*/

        TextWriter writer = new StreamWriter(fileName, true);
        writer.WriteLine(playerData.id + ";" + playerData.trial + ";" + playerData.time);
        writer.Close();
    }
}
