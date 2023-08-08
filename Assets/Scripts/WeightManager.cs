using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightManager : MonoBehaviour
{

    public enum WeightLevel
    {
        light, 
        mediumLight,
        mediumHeavy,
        heavy
    }

    public WeightLevel level = new WeightLevel();

    public void Start()
    {
        switch (level)
        {
            case WeightLevel.light:
                GameObject.Find("ContainerPlastic").tag = "Dumbbell";
                break;
            case WeightLevel.mediumLight:
                GameObject.Find("Container1Weight").tag = "Dumbbell";
                break;
            case WeightLevel.mediumHeavy:
                GameObject.Find("Container4Weight").tag = "Dumbbell";
                break;
            case WeightLevel.heavy:
                GameObject.Find("Container5Weight").tag = "Dumbbell";
                break;
            default:
                GameObject.Find("ContainerPlastic").tag = "Dumbbell";
                break;
        }
    }

}
