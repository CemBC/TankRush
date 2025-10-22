using System;
using UnityEngine;

public class PathProvider : MonoBehaviour
{
   
    public LevelData levelData;         
    
    public static PathProvider Instance { get; private set; }

    public Transform[] Waypoints { get; private set; }

    Transform Transforms;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;

        BuildFromLevelData();  
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
        ClearBuilt();
    }

    public Transform[] GetWaypoints() => Waypoints;


    public void BuildFromLevelData()
    {
        ClearBuilt();

        if (levelData == null || levelData.waypointPositions == null || levelData.waypointPositions.Count == 0)
        {
            Waypoints = Array.Empty<Transform>();
            return;
        }

        Transforms = new GameObject("Path_Waypoints").transform;
        Transforms.SetParent(transform, false);

        int count = levelData.waypointPositions.Count;
        Waypoints = new Transform[count];

        for (int i = 0; i < count; i++)
        {
            var gameObject = new GameObject($"Waypoint_{i}");
            var t = gameObject.transform;
            t.SetParent(Transforms, false);
            t.position = levelData.waypointPositions[i];

            Waypoints[i] = t;
        }
    }

    void ClearBuilt()
    {
        if (Transforms != null)
            Destroy(Transforms.gameObject);

        Waypoints = null;
    }
}
