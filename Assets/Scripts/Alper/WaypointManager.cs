using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public TMP_Text floatingTextPrefab;
    public List<Transform> wayPoints = new List<Transform>();
    private int targetPoint = 1;
    public float baseSpeed = 2f;
    private float currentSpeed;
    public float rotationSpeed = 10f;
    public float maxHealth;
    private float currentHealth;
    public bool IsDead => currentHealth <= 0f;
    public int rewardMoney = 1; //default 1
    public int heathReduction = 1;

    private Animator animator;
    private SphereCollider sphereCollider;
    private bool isDying = false;
    public bool IsAlive => !IsDead && !isDying;
    Coroutine slowRoutine;

    void Awake()
    {
        currentSpeed = baseSpeed;
        animator = GetComponent<Animator>();
        sphereCollider = GetComponent<SphereCollider>();
    }
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
            currentSpeed * Time.deltaTime
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
        if (isDying) return;
        isDying = true;

        currentSpeed = 0f;

        if (sphereCollider != null)
            sphereCollider.enabled = false;

        if (rewardMoney > 0 && GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(rewardMoney);

            if(floatingTextPrefab != null)
            {
                SpawnFloatingRewardText(string.Format("+{0}" , rewardMoney));
            }
        }
        if (animator != null)
            animator.SetTrigger("Die");

        StartCoroutine(WaitForDeathAnimAndDestroy("Die 0"));
    }

    IEnumerator WaitForDeathAnimAndDestroy(string deathStateName, int layer = 0)
    {
        if (animator == null)
        {
            Destroy(gameObject);
            yield break;
        }
        float safety = 5f; //bekleme süresi max 5 saniye
        while (!animator.GetCurrentAnimatorStateInfo(layer).IsName(deathStateName) && safety > 0f)
        {
            safety -= Time.deltaTime;
            yield return null;
        }
        if (animator.GetCurrentAnimatorStateInfo(layer).IsName(deathStateName))
        {
            var info = animator.GetCurrentAnimatorStateInfo(layer);
            while (info.normalizedTime < 0.99f)
            {
                yield return null;
                info = animator.GetCurrentAnimatorStateInfo(layer);
            }
        }
        Destroy(gameObject);
    }

    public void ApplySlow(float multiplier, float duration)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(SlowCo(multiplier, duration));
    }

    IEnumerator SlowCo(float multiplier, float duration)
    {
        currentSpeed = baseSpeed * Mathf.Clamp(multiplier, 0.05f, 10f);
        Debug.Log("Slow Uygulandı = " + currentSpeed);
        yield return new WaitForSeconds(duration);
        currentSpeed = baseSpeed;
        slowRoutine = null;
    }

        void SpawnFloatingRewardText(string msg)
    {
        TMP_Text txt = Instantiate(floatingTextPrefab);
        txt.text = msg;

        Transform tr = txt.transform;
        tr.position = (transform? transform.position : transform.position) + new Vector3(0,1f,0);
        Vector3 targetPosition = tr.position + new Vector3(0f, 0f, 0.5f);

        tr.DOMove(targetPosition, 1.1f)
          .SetEase(Ease.OutCubic)
          .OnComplete(() =>
          {
              if (txt)
                  Destroy(txt.gameObject);
          });
    }

}
