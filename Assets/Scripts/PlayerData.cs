using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int id;
    public int targetNumber;
    public WeightManager.WeightLevel level;
    public float time;

    public PlayerData(int playerId, int targetCount, WeightManager.WeightLevel weightLevel, float playerTime)
    {
        id = playerId;
        targetNumber = targetCount;
        level = weightLevel;
        time = playerTime;
    }
}
