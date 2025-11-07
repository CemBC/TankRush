using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GoldMine : MonoBehaviour , IPointerClickHandler
{
    public GameObject XIcon;
    int suppressClickFrame = -1;
    bool uiVisible = false;
    public  int goldPerTick = 5;
    public float intervalSeconds = 2f;
    public TMP_Text floatingTextPrefab;
    public Transform textSpawnPoint;
    public Vector3 worldOffset = new Vector3(0, 1f, 0);
    public float riseDistance = 1.0f;
    public float lifeTime = 1f;
    public string textFormat = "+{0}";

    Coroutine loop;
    void Awake()
    {
        if (XIcon)
        {
            XIcon.gameObject.SetActive(false);
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
            if (RayHitsObject(this.gameObject))
                return;

            HideUI();
            suppressClickFrame = Time.frameCount;
        }
    }
    void OnEnable()
    {
        if (loop == null)
            loop = StartCoroutine(MineLoop());
    }

    void OnDisable()
    {
        if (loop != null)
        {
            StopCoroutine(loop);
            loop = null;
        }
    }

    IEnumerator MineLoop()
    {
        if (!textSpawnPoint) textSpawnPoint = transform;
        var wait = new WaitForSeconds(intervalSeconds);

        while (true)
        {
            yield return new WaitUntil(() => GameManager.Instance && GameManager.Instance.getWaveInfo());

            yield return wait;

            if (!GameManager.Instance.getWaveInfo())
            {
                continue;   
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMoney(goldPerTick);
            }
            if (floatingTextPrefab)
            {
                SpawnFloatingText(string.Format(textFormat, goldPerTick));
            }
        }
    }

    void SpawnFloatingText(string msg)
    {
        TMP_Text txt = Instantiate(floatingTextPrefab);
        txt.text = msg;

        Transform transform = txt.transform;
        transform.position = (textSpawnPoint ? textSpawnPoint.position : base.transform.position) + worldOffset;


        Vector3 targetPosition = transform.position + new Vector3(0f, 0f, riseDistance);

        transform.DOMove(targetPosition, lifeTime).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            if (txt)
            {
                Destroy(txt.gameObject);
            }
        });
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

    void ShowUI()
    {
        if (XIcon) XIcon.gameObject.SetActive(true);
        uiVisible = true;
    }

    void HideUI()
    {
        if (XIcon) XIcon.SetActive(false);
        uiVisible = false;
    }
}
