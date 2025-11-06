using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Linq.Expressions;
[RequireComponent(typeof(SphereCollider))]
public class CrystalUnit : MonoBehaviour, IPointerClickHandler
{
    public float enemySlowMultiplier = 0.1f;
    public float enemySlowDuration = 1f;

    public float towerAtkSpeedMultiplier = 2f;
    public float towerBuffDuration = 2f;

    Vector3 xInitPos, uInitPos;
    Quaternion xInitRot, uInitRot;
    public GameObject XIcon;
    public GameObject UpgradeIcon;
    public float refreshInterval;
    bool uiVisible = false;
    int  suppressClickFrame = -1;  

    public TowerData data;
    public LayerMask enemyMask;
    public LayerMask towerMask;

    private SphereCollider rangeCollider;

    Coroutine loop;

    void Awake()
    {
        refreshInterval = 1f / data.attackSpeed;
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
        loop = StartCoroutine(ScanLoop());
    }

    void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
    }
    IEnumerator ScanLoop()
    {
        var wait = new WaitForSeconds(refreshInterval);
        var pos = transform;
        while (true)
        {
        Vector3 p = pos.position;
            var eHits = Physics.OverlapSphere(p,rangeCollider.radius, enemyMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < eHits.Length; i++)
            {
                if (!eHits[i]) continue;
                var enemy = eHits[i].GetComponentInParent<WaypointManager>();
                if (enemy != null)
                {
                    // içeri girdikçe/taramada görüldükçe süre yenilensin
                    enemy.ApplySlow(enemySlowMultiplier, enemySlowDuration);
                }
            }
            var tHits = Physics.OverlapSphere(p, rangeCollider.radius, towerMask, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < tHits.Length; i++)
            {
                if (!tHits[i]) continue;
                var tower = tHits[i].GetComponentInParent<TowerUnit>();
                if (tower != null)
                {
                    tower.ApplyAttackSpeedBuff(towerAtkSpeedMultiplier, towerBuffDuration);
                }
            }

            yield return wait;
        }
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

}
