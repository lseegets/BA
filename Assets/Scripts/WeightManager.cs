using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public enum WeightLevel
    {
        light,
        medium,
        heavy
    }

    public WeightLevel level = new WeightLevel();

    private GameObject weightLight, weightMedium, weightHeavy;

    public void Start()
    {
        weightLight = GameObject.Find("ContainerLight");
        weightMedium = GameObject.Find("ContainerMedium");
        weightHeavy = GameObject.Find("ContainerHeavy");

        GameObject[] weightList = { weightLight, weightMedium, weightHeavy };

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

            case WeightLevel.medium:
                weightMedium.tag = "Dumbbell";
                weightMedium.SetActive(true);
                foreach (GameObject weight in weightList)
                {
                    if (weight != weightMedium) weight.SetActive(false);
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
