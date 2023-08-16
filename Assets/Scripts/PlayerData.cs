using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int id;
    public string level;
    public float time;

    public PlayerData(int playerId, string weightLevel, float playerTime)
    {
        id = playerId;
        level = weightLevel;
        time = playerTime;
    }
}
