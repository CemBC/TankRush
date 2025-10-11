using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] wayPoints;
    public int targetPoint;
    public float moveSpeed;
    void Start()
    {
        targetPoint = 0;

    }
    public void increaseTargetInt()
    {
        targetPoint++;
        if(targetPoint >= wayPoints.Length)
        {
            targetPoint = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == wayPoints[targetPoint].position)
        {
            increaseTargetInt();

        }

        transform.position = Vector3.MoveTowards(transform.position, wayPoints[targetPoint].position, Time.deltaTime*moveSpeed);
        
    }
}
