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
      if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100.0f))
      {
        if (hit.collider.gameObject.CompareTag("Target"))
        {
            countdown -= Time.deltaTime;
            if (countdown < 0)
            {
                Destroy(hit.collider.gameObject);
                resetCountdown();
            }
        }
        else
        {
            resetCountdown();
        }
      }
      


       /* RaycastHit[] hits;
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
                    if (countdown < 0)
                    {
                        Destroy(currentObject);
                        resetCountdown();
                    }
                }
                else if (!currentObject.CompareTag("Target"))
                {
                    //resetCountdown();
                    Debug.Log("hitting canvas");
                }
            }
        }*/
    }

    private void resetCountdown()
    {
        countdown = 2.0f;
    }
}
