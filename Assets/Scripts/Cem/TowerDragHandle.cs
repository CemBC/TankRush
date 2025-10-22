using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerDragHandle : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Data")]
    public TowerData tower;

    CanvasGroup group;

    void Awake()
    {
        TryGetComponent(out group);
    }

    public void OnBeginDrag(PointerEventData e)
    {
        if (group)
        {
            group.blocksRaycasts = false;
        }
        DragPlacement.Instance?.BeginGhost(tower);
        DragPlacement.Instance?.UpdateGhostToScreenPos(e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        DragPlacement.Instance?.UpdateGhostToScreenPos(e.position);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (group)
        group.blocksRaycasts = true;

        if (GameManager.Instance != null && !GameManager.Instance.CanPlaceUnit())
        {
            Debug.Log("Max Unit geçildi");   
            DragPlacement.Instance?.EndGhost(place: false, screenPos: e.position);
            return;
        }
        DragPlacement.Instance?.EndGhost(place: true, screenPos: e.position);
    }
}
