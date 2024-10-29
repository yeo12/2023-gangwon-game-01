using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoMove : MonoBehaviour
{
    public Vector3 worldDirection;
    public Vector3 localDirection;

    public bool unscale;
    public float delay;
    public float worldSpeed;
    public bool randomSpeed;
    public float localMin;
    public float localMax;
    public float localSpeed;
    void Start()
    {
        delay += Time.time;
        if (randomSpeed)
        {
            localSpeed = Random.Range(localMin, localMax);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (delay > Time.time)
        {
            return;
        }
        Vector3 worldVelocity = Time.deltaTime * worldSpeed * worldDirection.normalized;
        if (unscale)
        {
            worldVelocity = Time.unscaledDeltaTime * worldSpeed * worldDirection.normalized;
        }
        Vector3 localVelocity = Vector3.zero;
        if (StageManager.inst.enemyStopTime <= Time.time)
        {            
            localVelocity = localSpeed * Time.deltaTime * localDirection.normalized;
            localVelocity = transform.TransformDirection(localVelocity);
        }
        transform.position += worldVelocity + localVelocity;
    }
}
