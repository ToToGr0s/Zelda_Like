using UnityEngine;
using System.Collections.Generic;

public class TreeCuller : MonoBehaviour
{
    public Terrain terrain;
    public Vector3 cullBox = new Vector3(10f, 20f, 10f);
    public Vector3 cullOffset = Vector3.zero;
    public Color gizmoFillColor = new Color(1f, 0f, 0f, 0.3f);
    public Color gizmoWireColor = new Color(1f, 0f, 0f, 1f);

    private TerrainData terrainData;
    private Dictionary<int, TreeInstance> hiddenTrees = new Dictionary<int, TreeInstance>();

    void Start()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;
    
        terrainData = terrain.terrainData;
    }

    void Update()
    {
        UpdateTreeVisibility();
    }
    
    void OnApplicationQuit()
    {
        RestoreAllTrees();
    }

    void OnDisable()
    {
        RestoreAllTrees();
    }

    void RestoreAllTrees()
    {
        if (hiddenTrees.Count == 0) return;

        TreeInstance[] trees = terrainData.treeInstances;

        foreach (var kvp in hiddenTrees)
        {
            trees[kvp.Key] = kvp.Value;
        }

        terrainData.treeInstances = trees;
        terrain.Flush();
        hiddenTrees.Clear();
    }

    Vector3 GetZoneCenter()
    {
        return transform.position + transform.TransformDirection(cullOffset);
    }

    void UpdateTreeVisibility()
    {
        TreeInstance[] trees = terrainData.treeInstances;
        bool modified = false;

        Bounds cullBounds = new Bounds(GetZoneCenter(), cullBox);

        for (int i = 0; i < trees.Length; i++)
        {
            Vector3 worldPos = Vector3.Scale(trees[i].position, terrainData.size) + terrain.transform.position;
            bool inZone = cullBounds.Contains(worldPos);

            if (inZone && trees[i].widthScale > 0)
            {
                hiddenTrees[i] = trees[i];
                trees[i].widthScale = 0;
                trees[i].heightScale = 0;
                modified = true;
            }
            else if (!inZone && trees[i].widthScale == 0 && hiddenTrees.ContainsKey(i))
            {
                trees[i] = hiddenTrees[i];
                hiddenTrees.Remove(i);
                modified = true;
            }
        }

        if (modified)
        {
            terrainData.treeInstances = trees;
            terrain.Flush();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoFillColor;
        Gizmos.DrawCube(GetZoneCenter(), cullBox);

        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireCube(GetZoneCenter(), cullBox);
    }
}