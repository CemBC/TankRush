using System.Collections.Generic;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public List<Transform> wayPoints = new List<Transform>();
    private int targetPoint = 0;
    public float moveSpeed = 2f;
    public float rotationSpeed = 10f;
    public float maxHealth;
    private float currentHealth;
    public bool IsDead => currentHealth <= 0f;
    public int rewardMoney = 1; //default 1
    public int heathReduction = 1;
    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("enemy Başlangıç canı" + currentHealth);
        LookAt();
    }

    void Update()
    {
        if (IsDead) return;
        if (wayPoints.Count == 0) return;

        float distance = Vector3.Distance(transform.position, wayPoints[targetPoint].position);

        if (distance < 0.1f)
        {
            targetPoint++;
            if (targetPoint >= wayPoints.Count)
            {
                GameManager.Instance?.TakeDamage(heathReduction);
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
    
    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        currentHealth -= amount;
        Debug.Log("damage yedik :" + currentHealth);
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    void Die()
    {
        //Buraya para düşürme ve elme animasyonu eklenecek
        if (rewardMoney > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(rewardMoney);
        }
        Destroy(gameObject);
    }
}
