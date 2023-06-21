using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int id;
    public int trial;
    public float time;

    public PlayerData(int playerId, int playerTrial, float playerTime)
    {
        id = playerId;
        trial = playerTrial;
        time = playerTime;
    }
}
