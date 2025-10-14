using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TowerUnit : MonoBehaviour
{
    public TowerData data; 
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileLifeTime;
    public float projectileRadius;
    public int projectilePierce;

    public LayerMask enemyMask;

    private SphereCollider rangeCollider;
    private Transform currentTarget;
    private Coroutine shootCoroutine;

    void Awake()
    {
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.isTrigger = true;
        rangeCollider.radius =data.range;
    }

    void OnEnable()
    {
        if (!data)
        {
            return;
        }
        shootCoroutine = StartCoroutine(ShootLoop());
    }

    void OnDisable()
    {
        if (shootCoroutine != null) StopCoroutine(shootCoroutine);
        currentTarget = null;
    }

    IEnumerator ShootLoop()
    {
        if (data.attackSpeed == 0)
        {
            yield break;
        }
        float period = 1f / data.attackSpeed;
        WaitForSeconds wait = new WaitForSeconds(period);

        while (true)
        {
            if (!IsValidTarget(currentTarget))
                currentTarget = PickNearestInRange();
            if (IsValidTarget(currentTarget))
            {
                Vector3 direction = currentTarget.position - transform.position;
                direction.y = 0f;
                if (direction.sqrMagnitude > 0.0001f)
                {
                    direction.Normalize();
                    transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                    Fire(direction);
                }
            }
            yield return wait;
        }
    }

    bool IsValidTarget(Transform t)
    {
        if (!t) return false;
        //buraya öldü ölmedi ya da mapten çıktı çıkmadı koyulacak ki ölene tekrardan vurmaya devam etmesin
        return (t.position - transform.position).sqrMagnitude <= rangeCollider.radius * rangeCollider.radius;
    }

    Transform PickNearestInRange()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, rangeCollider.radius, enemyMask);
        float best = float.MaxValue; 
        Transform final = null;
        Vector3 towerPosition = transform.position;

        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i]) continue;
            float distance = (hits[i].transform.position - towerPosition).sqrMagnitude;
            if (distance < best)
            {
                best = distance;
                final = hits[i].transform; 
            }
        }
        return final;
    }

    void Fire(Vector3 direction)
    {
        GameObject gameObject = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up)
        );

        var projectile = gameObject.GetComponent<Projectile>();
        if (!projectile) projectile = gameObject.AddComponent<Projectile>();
        projectile.Init(direction, projectileSpeed, projectileLifeTime, projectileRadius, projectilePierce, enemyMask, data.range);
    }
}
