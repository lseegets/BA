using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    Rigidbody rb;
    public Transform controller;
    [Range(0.0f, 360.0f)]
    public float rotateBy;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb.MovePosition(controller.position);
        rb.MoveRotation(controller.rotation * Quaternion.Euler(rotateBy, 0, 0));
    }
}
