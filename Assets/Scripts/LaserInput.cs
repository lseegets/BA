using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInput : MonoBehaviour
{

    public static GameObject currentObject;
    int currentID;

    private float range = 100.0f;
    private float countdown = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        currentID = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.forward, range);

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            int id = hit.collider.gameObject.GetInstanceID();

            if (currentID != id)
            {
                currentID = id;
                currentObject = hit.collider.gameObject;

                if (currentObject.CompareTag("Target"))
                {
                    countdown -= Time.deltaTime;
                    if (countdown < 0) Destroy(currentObject);
                }

                /*string name = currentObject.name;
                if (name == "Next")
                {
                    Debug.Log("HIT NEXT");
                }

                if (currentObject.CompareTag("Target"))
                {
                    Debug.Log("HIT TARGET");
                }*/
            }
        }
    }
}
