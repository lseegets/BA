using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInput : MonoBehaviour
{
    [SerializeField] GameObject cube;

    public static GameObject currentObject;

    public Material activatedMaterial;
    public Material defaultMaterial;

    private const float DestructionTime = 0.2f;

    private float countdown;
    private Plane plane;
    private ViveTracker viveTracker;
    private Vector3 currentRayPos;
    private Vector3 lastRayPos;

    // Start is called before the first frame update
    void Start()
    {
        currentObject = null;
        countdown = DestructionTime;
        viveTracker = GameObject.FindGameObjectsWithTag("Tracker")[0].GetComponent<ViveTracker>();

        lastRayPos = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
   /* void FixedUpdate()
    {
      if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 100.0f))
      {
            Ray ray = new Ray(transform.position, transform.forward);
            float enter = 0.0f;
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                currentRayPos = hitPoint;
                SendRayData(currentRayPos, lastRayPos);
                lastRayPos = currentRayPos;
                //Debug.Log(hitPoint);
            }
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
    }*/

    void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float enter = 0.0f;
            if (plane.Raycast(ray, out enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                currentRayPos = hitPoint;
                cube.transform.position = hitPoint;
                SendRayData(currentRayPos, lastRayPos);
                lastRayPos = currentRayPos;
                //Debug.Log(hitPoint);
            }
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

    public void GetPlaneData(Plane plane)
    {
        this.plane = plane;
      //  Debug.Log("Sent Plane Data");
    }

    public void SendRayData(Vector3 currentRayPos, Vector3 lastRayPos)
    {
        viveTracker.HandleRayData(currentRayPos, lastRayPos);
    }

    private void ResetCountdown()
    {
        countdown = DestructionTime;
    }
}
