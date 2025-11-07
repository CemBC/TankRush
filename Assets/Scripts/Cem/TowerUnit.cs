using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening;
[RequireComponent(typeof(SphereCollider))]
public class TowerUnit : MonoBehaviour, IPointerClickHandler
{
    private TMP_Text floatingTextPrefab; 
    public TextMeshPro upgradeCostUI;
    Vector3 xInitPos, uInitPos;
    Quaternion xInitRot, uInitRot;
    public GameObject XIcon;
    public GameObject UpgradeIcon;

    bool uiVisible = false;
    int suppressClickFrame = -1;

    public TowerData data;

    public float attackSpeed;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public float projectileLifeTime;
    public float projectileRadius;
    public int projectilePierce;

    public LayerMask enemyMask;

    private SphereCollider rangeCollider;
    private Transform currentTarget;
    private Coroutine shootCoroutine;

    public Material rangeMaterial;
    public float rangeYOffset = 0.02f;
    GameObject rangeGO;
    Renderer rangeRend;

    void Awake()
    {
        attackSpeed = data.attackSpeed;
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
            if (upgradeCostUI != null) upgradeCostUI.text = data.upgradeCost.ToString();
            UpgradeIcon.gameObject.SetActive(false);
        }
        floatingTextPrefab = data.textPrefab;
        if (data != null && data.upgradePopups != null && data.upgradePopups.Length > 0)
        {
            StartCoroutine(ShowUpgradePopups(data.upgradePopups));
        }

    }
    
    IEnumerator ShowUpgradePopups(string[] messages)
    {
        if (messages.Length == 0) yield break;
            for (int i = 0; i < messages.Length; i++)
            {
                TMP_Text txt = Instantiate(floatingTextPrefab);
                Transform tr = txt.transform;
                tr.position = transform.position + data.popupOffset;
                txt.text = messages[i];
                Vector3 targetPos = tr.position + new Vector3(0f, 0f, data.popupRiseDistance);
                tr.DOMove(targetPos, data.popupLifeTime).SetEase(Ease.OutCubic)
                    .OnComplete(() => { if (txt) Destroy(txt.gameObject); });

                yield return new WaitForSeconds(data.popupInterval);
        }
    }

    void OnEnable()
    {
        if (!data)
        {
            return;
        }

        CreateRangeSphere();
        if (rangeGO) rangeGO.SetActive(false);
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
                GameManager.Instance?.RemoveUnit();
                Destroy(gameObject);

                return;
            }
            if (RayHitsObject(UpgradeIcon))
            {
                TryUpgrade();
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

    void OnDestroy()
    {
        if (rangeRend && rangeRend.sharedMaterial) Destroy(rangeRend.sharedMaterial);        
    }

    void CreateRangeSphere()
    {
        if (rangeGO != null || data == null || data.range <= 0f || rangeMaterial == null) return;
        rangeGO = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeGO.name = "RangeSphere";
        rangeGO.transform.SetParent(transform, false);
        var col = rangeGO.GetComponent<Collider>(); if (col) Destroy(col);
        rangeGO.layer = LayerMask.NameToLayer("Ignore Raycast");
        rangeRend = rangeGO.GetComponent<Renderer>();
        rangeRend.sharedMaterial = Instantiate(rangeMaterial);
        rangeRend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rangeRend.receiveShadows = false;
        rangeGO.transform.localPosition = new Vector3(0f, rangeYOffset, 0f);
        float d = Mathf.Max(0.001f, data.range * 2f);
        rangeGO.transform.localScale = new Vector3(d, d, d);   
    }

    IEnumerator ShootLoop()
    {
        if (attackSpeed == 0)
        {
            yield break;
        }
        float period = 1f / attackSpeed;
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
            transform.position + Vector3.up * 0.2f,
            Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z), Vector3.up)
        );

        var projectile = gameObject.GetComponent<Projectile>();
        if (!projectile) projectile = gameObject.AddComponent<Projectile>();
        projectile.Init(direction,
         projectileSpeed,
          projectileLifeTime,
           projectileRadius,
            projectilePierce,
             enemyMask,
              data.range,
              data.damage);
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
        if (rangeGO) rangeGO.SetActive(true);
        uiVisible = true;
    }

    void HideUI()
    {
        if (XIcon) XIcon.SetActive(false);
        if (UpgradeIcon) UpgradeIcon.SetActive(false);
        if (rangeGO) rangeGO.SetActive(false);
        uiVisible = false;

    }

    bool RayHitsObject(GameObject target)  //boşa tıklama için kontrol
    {
        if (target == null) return false;

        var cam = Camera.main;
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


    void TryUpgrade()
    {
        if (data.nextUpgrade == null)
        {
            Debug.Log("Bu kule en üst seviyede!");
            return;
        }
        if (GameManager.Instance != null &&
            GameManager.Instance.TrySpend(data.upgradeCost))
        {
            Vector3 pos = transform.position;
            Transform parent = transform.parent;

            GameObject newTower = Instantiate(
                data.nextUpgrade,
                pos,
                Quaternion.identity,
                parent
            );

            Destroy(gameObject);
        }
        else
        {
            GameManager.Instance.NoMoneyFeedback();
        }
    }

    private Coroutine atkBuffRoutine;
    public void ApplyAttackSpeedBuff(float multiplier, float duration)
    {
        if (atkBuffRoutine != null) StopCoroutine(atkBuffRoutine);

        multiplier = Mathf.Clamp(multiplier, 0.1f, 10f);
        attackSpeed = data.attackSpeed * multiplier;
        RestartShootLoop();
        Debug.Log("AttackSpeed Arttı = " + attackSpeed);
        atkBuffRoutine = StartCoroutine(ResetAttackSpeedAfter(duration));
    }

    IEnumerator ResetAttackSpeedAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        attackSpeed = data.attackSpeed;
        RestartShootLoop(); 
        atkBuffRoutine = null;
    }

    void RestartShootLoop()
    {
        if (shootCoroutine != null) StopCoroutine(shootCoroutine);
        shootCoroutine = StartCoroutine(ShootLoop());
    }

}
