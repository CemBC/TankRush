using UnityEngine;

public class PlacementRules : MonoBehaviour
{
    public LayerMask groundMask;
    public LayerMask forbiddenMask; 
    public string[] forbiddenTags;
    public LayerMask towerMask;
    public LayerMask obsticleMask;
    
    public bool CanPlaceAt(Vector3 world, TowerData tower)
    {
        if (tower == null) return false;
        if (IsForbiddenUnder(world)) return false;
        if (Physics.OverlapSphere(world + Vector3.up * 0.5f, tower.footprintRadius, towerMask,    QueryTriggerInteraction.Ignore).Length > 0) return false;
        if (Physics.OverlapSphere(world + Vector3.up * 0.5f, tower.footprintRadius, obsticleMask, QueryTriggerInteraction.Ignore).Length > 0) return false;

        return true;
    }

    bool IsForbiddenUnder(Vector3 world)
    {
        int mask = LayerMask.GetMask("Ground", "Forbidden", "Obsticle");


        Ray ray = new Ray(world + Vector3.up * 5f, Vector3.down);
        if (!Physics.Raycast(ray, out RaycastHit hit, 15f, mask, QueryTriggerInteraction.Ignore)) return true;

        GameObject hitObj = hit.collider.gameObject;
        int hitLayer = hitObj.layer;

        bool fromLayer = (forbiddenMask.value & (1 << hitLayer)) != 0;
        if (fromLayer) return true;

        if (forbiddenTags != null)
        {
            for (int i = 0; i < forbiddenTags.Length; i++)
            {
                string tg = forbiddenTags[i];
                if (!string.IsNullOrEmpty(tg) && hitObj.CompareTag(tg))
                    return true;
            }
        }

        return false;
    }
}
