using UnityEngine;

[CreateAssetMenu(fileName = "TokenData", menuName = "TD/Token")]
public class TokenData : ScriptableObject
{
    [Header("Token")]
    public string token;

    [Header("Prefab")]
    public GameObject prefab;
    public Vector3 rotation;
    public Vector3 scale = Vector3.one;

    public void ApplyTo(GameObject gameObject)
    {
        if (!gameObject) return;
        gameObject.transform.localEulerAngles = rotation;
        gameObject.transform.localScale = scale;
    }
}
