using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int id;
    public int targetNumber;
   // public string level;
    public WeightManager.WeightLevel level;
    public float time;
    public float speed;

    public PlayerData(int playerId, int targetCount, /*string*/ WeightManager.WeightLevel weightLevel, float playerTime, float maxSpeed)
    {
        id = playerId;
        targetNumber = targetCount;
        level = weightLevel;
        time = playerTime;
        speed = maxSpeed;
    }
}
