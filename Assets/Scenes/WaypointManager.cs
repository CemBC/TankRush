using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public Transform[] wayPoints;
    public int targetPoint;
    public float moveSpeed = 2f;
    private Animator animator;
    private bool isDead = false;
    private Renderer[] renderers;
     [Header("Death Settings")]
    public float deathAnimDuration = 2f;   // ölüm animasyon süresi
    public float deadBodyStayTime = 3f;    // yerde kalma süresi
    public float fadeDuration = 2f;        // yavaşça kaybolma süresi
    public GameObject deathEffect;
    void Start()
    {
        targetPoint = 0;
        animator = GetComponent<Animator>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        if (isDead) return;
        
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
            Die();
            return;
        }
    }
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        moveSpeed = 0f;
        animator.SetTrigger("Die");

        StartCoroutine(DeathSequence());
    }

    // 💀 Yavaşça yok olma efekti
    private IEnumerator DeathSequence()
{
    yield return new WaitForSeconds(deathAnimDuration + deadBodyStayTime);

    float elapsed = 0f;

    // Yavaş yavaş kaybol
    while (elapsed < fadeDuration)
    {
        elapsed += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

        foreach (Renderer r in renderers)
        {
            foreach (Material mat in r.materials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }
            }
        }

        yield return null;
    }

    Destroy(gameObject);
}

}
