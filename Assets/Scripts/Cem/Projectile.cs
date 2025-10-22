using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Projectile : MonoBehaviour
{
    private float speed, lifetime, radius;
    private int pierce;
    private LayerMask enemyMask;
    private float maxRange;

    private Vector3 direction;
    private float age;
    private float traveled;
    private readonly HashSet<Collider> hitSet = new();

    public void Init(Vector3 direction, float speed, float lifetime, float radius, int pierce, LayerMask enemyMask, float maxRange)
    {
        this.direction = direction.normalized;
        this.speed = speed;
        this.lifetime = lifetime;
        this.radius = radius;
        this.pierce = pierce;
        this.enemyMask = enemyMask;
        this.maxRange = maxRange * 1.2f;

        transform.rotation = Quaternion.LookRotation(new Vector3(this.direction.x, 0f, this.direction.z), Vector3.up);
        age = 0f;
        traveled = 0f;
    }

    void Update()
    {
        float time = Time.deltaTime;
        age += time;
        if (age > lifetime)
        {
            Destroy(gameObject); return;
        }
        
        Vector3 prev = transform.position;
        Vector3 step = direction * speed * time;
        Vector3 next = prev + step;
        if (Physics.SphereCast(prev, radius, step.normalized, out RaycastHit hit, step.magnitude, enemyMask, QueryTriggerInteraction.Collide))
        {
            if (!hitSet.Contains(hit.collider))
            {
                hitSet.Add(hit.collider);
                pierce--;
                // TODO: hasar
                if (pierce <= 0)
                {
                    transform.position = hit.point;
                    Destroy(gameObject);
                    return;
                }
            }
        }
        else
        {
            var hits = Physics.OverlapSphere(next, radius, enemyMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < hits.Length; i++)
            {
                var currentHit = hits[i];
                if (hitSet.Contains(currentHit)) continue;
                hitSet.Add(currentHit);
                pierce--;
                // TODO: hasar
                if (pierce <= 0)
                {
                    transform.position = next;
                    Destroy(gameObject);
                    return;
                }
            }
        }

        transform.position = next;
        //if (step.sqrMagnitude > 0.000001f)
        if (step.sqrMagnitude > 1e-6f)  //hareket sıfırsa boşuna yön hesaplama koruması
            transform.forward = Vector3.Lerp(transform.forward, step.normalized, 0.5f);

        traveled += step.magnitude;
        if (traveled >= maxRange) Destroy(gameObject);
    }
}
