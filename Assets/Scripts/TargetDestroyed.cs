using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDestroyed : MonoBehaviour
{
    private void OnDestroy()
    {
        GameObject.Find("[CameraRig]/Camera").GetComponent<TargetSpawn>().OnTargetDestroyed();
    }
}
