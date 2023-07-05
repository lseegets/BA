using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInput : MonoBehaviour
{

    public static GameObject currentObject;

    public Material activatedMaterial;
    public Material defaultMaterial;

    private float countdown = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
    }

    // Update is called once per frame
    void Update()
    {
      if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100.0f))
      {
        if (hit.collider.gameObject.CompareTag("Target"))
        {
            currentObject = hit.collider.gameObject;
            currentObject.GetComponent<MeshRenderer>().material = activatedMaterial;
            countdown -= Time.deltaTime;
            if (countdown < 0)
            {
                Destroy(currentObject);
                ResetCountdown();
            }
        }
        else
        {
            ResetCountdown();
            if (currentObject != null) currentObject.GetComponent<MeshRenderer>().material = defaultMaterial;
        }
      }
    }

    private void ResetCountdown()
    {
        countdown = 2.0f;
    }
}
