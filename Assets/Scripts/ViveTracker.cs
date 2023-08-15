using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViveTracker : MonoBehaviour
{

    private Vector3 currentPos;
    private Vector3 lastPos;

    // Start is called before the first frame update
    void Start()
    {
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        currentPos = transform.position;
        Debug.Log(currentPos);
    }

    
}
