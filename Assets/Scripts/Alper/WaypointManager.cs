using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> wayPoints = new List<Transform>();
    public int targetPoint = 0;
    public float moveSpeed = 2f;
    public float rotationSpeed = 10f;


    void Start()
    {
        LookAt();
    }

    void Update()
    {
        if (wayPoints.Count == 0) return;

        float distance = Vector3.Distance(transform.position, wayPoints[targetPoint].position);

        if (distance < 0.1f)
        {
            targetPoint++;
            if (targetPoint >= wayPoints.Count)
            {
                Destroy(gameObject); 
                return;
            }
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            wayPoints[targetPoint].position,
            moveSpeed * Time.deltaTime
        );

        Vector3 direction = (wayPoints[targetPoint].position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void LookAt()
    {
        if (wayPoints.Count == 0) return;
        Vector3 direction = (wayPoints[targetPoint].position - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);
    }
}
