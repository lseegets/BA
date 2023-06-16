using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveData
{
    public static void Save(float[] trackingData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = "C:/UnitySaves/";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, trackingData);
        stream.Close();
    }
}
