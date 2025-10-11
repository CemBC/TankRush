using UnityEngine;

[CreateAssetMenu(fileName = "TokenData", menuName = "TD/Token")]
public class TokenData : ScriptableObject
{
    [Header("Token Letter")]
    public string token;

    [Header("Prefab")]
    public GameObject prefab;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;

    public void ApplyTo(GameObject go)
    {
        if (!go) return;
        go.transform.localEulerAngles = rotation;
        go.transform.localScale = scale;
    }
}
