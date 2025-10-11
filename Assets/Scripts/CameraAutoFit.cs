using UnityEngine;

[ExecuteAlways]
public class CameraAutoFit : MonoBehaviour
{
    public Camera cam;
    public LevelData level;  
    public float tileSize = 1f;
    public float padding = 0.5f;

    public void Fit()
    {
        if (cam == null || level == null 
        || level.terrainRows == null 
        || level.terrainRows.Length == 0)
        {
            Debug.LogError("Camera or Level missing");
            return;
        }

        int height = level.terrainRows.Length;
        int width = CountCells(level.terrainRows[0]);

        float worldWidth = width * tileSize;
        float worldHeight = height * tileSize;

        Vector3 center = new Vector3((width - 1) * tileSize * 0.5f, 0f,
        (height - 1) * tileSize * 0.5f);
        cam.orthographic = true;
        cam.transform.position = new Vector3(center.x, 10f, center.z);
        cam.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        float sizeByHeight = (worldHeight * 0.5f) + padding;
        float sizeByWidth = (worldWidth * 0.5f / cam.aspect) + padding;
        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
    }

    private int CountCells(string row)
    {
        if (string.IsNullOrWhiteSpace(row)) return 0;
        return row.Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private void Start()
    {
        Fit();
    }
}
