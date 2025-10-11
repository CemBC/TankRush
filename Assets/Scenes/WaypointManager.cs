using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] wayPoints;
    public int targetPoint;
    public float moveSpeed = 2f;
    private Animator animator;

    void Start()
    {
        targetPoint = 0;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (wayPoints == null || wayPoints.Length == 0) return;
        if (wayPoints[targetPoint] == null) return;

        float distance = Vector3.Distance(transform.position, wayPoints[targetPoint].position);

        if (distance < 0.1f)
        {
            increaseTargetInt();
        }

        Vector3 direction = (wayPoints[targetPoint].position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            transform.forward = Vector3.Lerp(transform.forward, direction, Time.deltaTime * 5f);
        }

        // hareket
        transform.position = Vector3.MoveTowards(
            transform.position,
            wayPoints[targetPoint].position,
            Time.deltaTime * moveSpeed
        );

        // animator parametresi
        if (animator != null)
        {
            float currentSpeed = (moveSpeed > 0.05f) ? moveSpeed : 0f;
            animator.SetFloat("Speed", currentSpeed);
        }
    }

    public void increaseTargetInt()
    {
        targetPoint++;
        if (targetPoint >= wayPoints.Length)
        {
            targetPoint = 0;
        }
    }
}
