using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> wayPoints = new List<Transform>(); // 👈 Artık List
    public int targetPoint = 0;
    public float moveSpeed = 2f;

    void Update()
    {
        if (wayPoints.Count == 0) return;

        float distance = Vector3.Distance(transform.position, wayPoints[targetPoint].position);

        if (distance < 0.1f)
        {
            targetPoint++;
            if (targetPoint >= wayPoints.Count)
            {
                Destroy(gameObject); // sona ulaşınca yok ol
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
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);
    }
}
