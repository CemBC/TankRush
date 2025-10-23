using UnityEngine;

public class DragPlacement : MonoBehaviour
{
    public static DragPlacement Instance;

    [Header("References")]
    public Camera cam; 
    public Transform mapRoot;
    public LayerMask groundMask;
    public PlacementRules rules;

    [Header("Ghost Tint")]
    public Color validTint   = new Color(0f, 1f, 0f, 0.5f); 
    public Color invalidTint = new Color(1f, 0f, 0f, 0.5f); 
    public Color neutralTint = new Color(1f, 1f, 1f, 0.5f); 

    [Header("Range Tint")]
    public Material rangeMaterial;
    public Color rangeValidColor   = new Color(0f, 1f, 0f, 0.15f);
    public Color rangeInvalidColor = new Color(1f, 0f, 0f, 0.15f);
    public float rangeYOffset = 0.02f;

    private TowerData data;
    private GameObject ghost;
    private Renderer[] ghostRenderers;
    private MaterialPropertyBlock mpb;

    private GameObject rangeGameObject;
    private Renderer rangeRenderer;

    void Awake()
    {
        Instance = this;
        mpb = new MaterialPropertyBlock();
    }

    public void BeginGhost(TowerData data)
    {
        this.data = data;
        if (!this.data) return;

        var prefab = this.data.ghostPrefab ? this.data.ghostPrefab : this.data.prefab;
        ghost = Instantiate(prefab);
        ghostRenderers = ghost.GetComponentsInChildren<Renderer>(true);
        foreach (var c in ghost.GetComponentsInChildren<Collider>(true)) c.enabled = false;
        SetGhostTint(neutralTint);
        CreateRangeSphere();
    }

    
    public void UpdateGhostToScreenPos(Vector2 screenPos)
    {
        if (!ghost || !cam) return;
        if (!ScreenToWorld(screenPos, out var world)) return;

        if (data) world.y += data.yOffset;
        ghost.transform.position = world;
        bool canPlace = (rules == null) || rules.CanPlaceAt(world, data);

        SetGhostTint(canPlace ? validTint : invalidTint);
        UpdateRangeSphere(canPlace);
    }
    public void EndGhost(bool place, Vector2 screenPos)
    {
        if (!ghost) return;

        if (place && ScreenToWorld(screenPos, out var world))
        {
            if (data) world.y += data.yOffset;
            if (rules == null || rules.CanPlaceAt(world, data))
            {
                if (GameManager.Instance.TrySpend(data.cost))
                {
                    Instantiate(data.prefab, world, Quaternion.identity, mapRoot);
                    GameManager.Instance?.AddUnit();
                }
                else
                {
                    GameManager.Instance.NoMoneyFeedback();
                }
            }
        }

        if (rangeGameObject) Destroy(rangeGameObject); rangeGameObject = null; rangeRenderer = null;
        Destroy(ghost);
        ghost = null; 
        ghostRenderers = null;
        data = null;
    }

    bool ScreenToWorld(Vector2 screenPos, out Vector3 world)
    {
        var ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out var hit, 2000f, groundMask, QueryTriggerInteraction.Ignore))
        { world = hit.point; return true; }
        var plane = new Plane(Vector3.up, Vector3.zero);
        if (plane.Raycast(ray, out float d))
        { 
            world = ray.GetPoint(d); return true; 
        }
        world = default; return false;
    }

    void SetGhostTint(Color tint)
    {
        if (ghostRenderers == null) return;

        for (int i = 0; i < ghostRenderers.Length; i++)
        {
            var r = ghostRenderers[i]; if (!r) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_Color", tint);
            mpb.SetColor("_BaseColor", tint);
            r.SetPropertyBlock(mpb);
        }
    }
    void CreateRangeSphere()
    {
        if (rangeGameObject != null || data == null || data.range <= 0f || rangeMaterial == null) return;
        rangeGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        rangeGameObject.name = "RangeSphere (ghost)";
        rangeGameObject.transform.SetParent(ghost.transform, worldPositionStays: false);

        var col = rangeGameObject.GetComponent<Collider>(); if (col) Destroy(col);
        rangeGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        rangeRenderer = rangeGameObject.GetComponent<Renderer>();
        rangeRenderer.sharedMaterial = new Material(rangeMaterial);
        rangeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rangeRenderer.receiveShadows = false;

        rangeGameObject.transform.localPosition = new Vector3(0f, rangeYOffset, 0f);
        float d = Mathf.Max(0.001f, data.range * 2f); 
        rangeGameObject.transform.localScale = new Vector3(d, d, d);
        SetRangeColor(rangeValidColor);
    }

    void UpdateRangeSphere(bool canPlace)
    {
        if (!rangeGameObject || data == null) return;
        rangeGameObject.transform.localPosition = new Vector3(0f, rangeYOffset, 0f);
        float d = Mathf.Max(0.001f, data.range * 2f);
        rangeGameObject.transform.localScale = new Vector3(d, d, d);
        SetRangeColor(canPlace ? rangeValidColor : rangeInvalidColor);
    }

    void SetRangeColor(Color c)
    {
        if (!rangeRenderer) return;
        var material = rangeRenderer.sharedMaterial;
        material.color = c;
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", c); // URP
    }
}
