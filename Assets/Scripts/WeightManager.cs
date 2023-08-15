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

    private GameObject weightLight, weightMediumLight, weightMediumHeavy, weightHeavy;

    public void Start()
    {
       // weightLight = GameObject.Find("ContainerPlastic");
        weightLight = GameObject.Find("ContainerLight");
       // weightMediumLight = GameObject.Find("Container1Weight");
        weightMediumLight = GameObject.Find("ContainerMedium");
        //weightMediumHeavy = GameObject.Find("Container4Weight");
        weightMediumHeavy = GameObject.Find("ContainerMedium2");
        //weightHeavy = GameObject.Find("Container5Weight");
        weightHeavy = GameObject.Find("ContainerVeryHeavy");

        GameObject[] weightList = { weightLight, weightMediumLight, weightMediumHeavy, weightHeavy };

        switch (level)
        {
            case WeightLevel.light:
                weightLight.tag = "Dumbbell";
                weightLight.SetActive(true);
                foreach (GameObject weight in weightList)
                {
                    if (weight != weightLight) weight.SetActive(false);
                }
                break;

            case WeightLevel.mediumLight:
                weightMediumLight.tag = "Dumbbell";
                weightMediumLight.SetActive(true);
                foreach (GameObject weight in weightList)
                {
                    if (weight != weightMediumLight) weight.SetActive(false);
                }
                break;

            case WeightLevel.mediumHeavy:
                weightMediumHeavy.tag = "Dumbbell";
                weightMediumHeavy.SetActive(true);
                foreach (GameObject weight in weightList)
                {
                    if (weight != weightMediumHeavy) weight.SetActive(false);
                }
                break;

            case WeightLevel.heavy:
                weightHeavy.tag = "Dumbbell";
                weightHeavy.SetActive(true);
                foreach (GameObject weight in weightList)
                {
                    if (weight != weightHeavy) weight.SetActive(false);
                }
                break;

            default:
                weightLight.tag = "Dumbbell";
                weightLight.SetActive(true);
                foreach (GameObject weight in weightList)
                {
                    if (weight != weightLight) weight.SetActive(false);
                }
                break;
        }
    }

}
