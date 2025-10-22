using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
[RequireComponent(typeof(SphereCollider))]
public class TowerUnit : MonoBehaviour, IPointerClickHandler
{
    Vector3 xInitPos, uInitPos;
    Quaternion xInitRot, uInitRot;
    public GameObject XIcon;
    public GameObject UpgradeIcon;
    
    bool uiVisible = false;
    int  suppressClickFrame = -1;  

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
        rangeCollider.radius = data.range;
        rangeCollider.enabled = false;
        if (XIcon)
        {
            xInitPos = XIcon.transform.position;
            xInitRot = XIcon.transform.rotation;
            XIcon.gameObject.SetActive(false);
        }
        if (UpgradeIcon)
        {
            uInitPos = UpgradeIcon.transform.position;
            uInitRot = UpgradeIcon.transform.rotation;
            UpgradeIcon.gameObject.SetActive(false);
        }

    }

    void OnEnable()
    {
        if (!data)
        {
            return;
        }
        shootCoroutine = StartCoroutine(ShootLoop());


    }

     public void OnPointerClick(PointerEventData e)
    {
        if (e.button != PointerEventData.InputButton.Left) return;
        if (Time.frameCount == suppressClickFrame) return;

        if (!uiVisible)
        {
            ShowUI();
        }
        
    }

    void Update()
    {
        if (!uiVisible || Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (RayHitsObject(XIcon))
            {
                Destroy(gameObject);
                return;
            }
            if (RayHitsObject(this.gameObject))
                return;

            HideUI();
            suppressClickFrame = Time.frameCount;
        }
    }
    void LateUpdate()
    {
        if (!uiVisible) return;

        if (XIcon)
        {
            XIcon.transform.position = xInitPos;
            XIcon.transform.rotation = xInitRot;
        }
        if (UpgradeIcon)
        {
            UpgradeIcon.transform.position = uInitPos;
            UpgradeIcon.transform.rotation = uInitRot;
        }


        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!RayHitsObject(gameObject))
            {
                HideUI();
            }
        }
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


    void ShowUI()
    {
       if (XIcon)
        {
            XIcon.transform.position = xInitPos;
            XIcon.transform.rotation = xInitRot;
            XIcon.gameObject.SetActive(true);
        }
        if (UpgradeIcon)
        {
            UpgradeIcon.transform.position = uInitPos;
            UpgradeIcon.transform.rotation = uInitRot;
            UpgradeIcon.gameObject.SetActive(true);
        }
        uiVisible = true;
    }

    void HideUI()
    {
        if (XIcon) XIcon.SetActive(false);
        if (UpgradeIcon) UpgradeIcon.SetActive(false);
        uiVisible = false;
    }

    bool RayHitsObject(GameObject target)
    {
        if (target == null) return false;

        var cam   = Camera.main;
        var mouse = Mouse.current;

        if (cam == null) return false;

         Vector2 screenPos;
        if (mouse != null)
            screenPos = mouse.position.ReadValue();
        else if (Touchscreen.current != null)
            screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
        else
            return false;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~0, QueryTriggerInteraction.Ignore))
        {
            return hit.transform == target.transform || hit.transform.IsChildOf(target.transform);
        }
        return false;
    }

}
