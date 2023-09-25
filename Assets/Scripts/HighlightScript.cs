using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HighlightTrigger"))
        {
            transform.gameObject.SetActive(false);
            Debug.Log("COLLISION");
        }
    }
}
