using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawn : MonoBehaviour
{
    [SerializeField] GameObject target;

    private GameObject currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        SpawnFirstTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget == null)
        {
            SpawnNextTarget();
        }
    }

    private void SpawnFirstTarget()
    {
        Vector3 targetCenter = new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(0f, 5.0f), 5.0f);
        currentTarget = Instantiate(target, targetCenter, Quaternion.identity);
        Debug.Log(currentTarget);
    }

    private void SpawnNextTarget()
    {
        Vector3 targetCenter = new Vector3(Random.Range(-2.5f, 2.5f), Random.Range(0f, 5.0f), 5.0f);
        currentTarget = Instantiate(target, targetCenter, Quaternion.identity);
        Debug.Log(currentTarget);
    }
}
