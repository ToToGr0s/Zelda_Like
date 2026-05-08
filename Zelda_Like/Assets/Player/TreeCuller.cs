using System.Collections.Generic;
using UnityEngine;

public class TreeCuller : MonoBehaviour
{
    public Terrain terrain;
    public Vector3 cullBox = new Vector3(10f, 20f, 10f);
    public Vector3 cullOffset = Vector3.zero;
    public Color gizmoFillColor = new Color(1f, 0f, 0f, 0.3f);
    public Color gizmoWireColor = new Color(1f, 0f, 0f, 1f);

    private TerrainData terrainData;
    private readonly Dictionary<int, TreeInstance> hiddenTrees = new();
    private Bounds cullBounds;

    private void Start()
    {
        if (terrain == null)
            terrain = Terrain.activeTerrain;

        if (terrain == null)
        {
            enabled = false;
            return;
        }

        terrainData = terrain.terrainData;
    }

    private void Update()
    {
        if (terrainData == null)
            return;

        UpdateTreeVisibility();
    }

    private void OnApplicationQuit()
    {
        RestoreAllTrees();
    }

    private void OnDisable()
    {
        RestoreAllTrees();
    }

    private void RestoreAllTrees()
    {
        if (terrainData == null || hiddenTrees.Count == 0)
            return;

        TreeInstance[] trees = terrainData.treeInstances;

        foreach (KeyValuePair<int, TreeInstance> kvp in hiddenTrees)
            trees[kvp.Key] = kvp.Value;

        terrainData.treeInstances = trees;
        terrain.Flush();
        hiddenTrees.Clear();
    }

    private Vector3 GetZoneCenter()
    {
        return transform.position + transform.TransformDirection(cullOffset);
    }

    private void UpdateTreeVisibility()
    {
        TreeInstance[] trees = terrainData.treeInstances;
        bool modified = false;

        cullBounds.center = GetZoneCenter();
        cullBounds.size = cullBox;

        for (int i = 0; i < trees.Length; i++)
        {
            Vector3 worldPos = Vector3.Scale(trees[i].position, terrainData.size) + terrain.transform.position;
            bool inZone = cullBounds.Contains(worldPos);

            if (inZone && trees[i].widthScale > 0f)
            {
                hiddenTrees[i] = trees[i];
                trees[i].widthScale = 0f;
                trees[i].heightScale = 0f;
                modified = true;
            }
            else if (!inZone && trees[i].widthScale == 0f && hiddenTrees.TryGetValue(i, out TreeInstance originalTree))
            {
                trees[i] = originalTree; // OPTIMIZED: restore using a single dictionary lookup.
                hiddenTrees.Remove(i);
                modified = true;
            }
        }

        if (!modified)
            return;

        terrainData.treeInstances = trees;
        terrain.Flush();
    }

    private void OnDrawGizmos()
    {
        Vector3 zoneCenter = GetZoneCenter();
        Gizmos.color = gizmoFillColor;
        Gizmos.DrawCube(zoneCenter, cullBox);

        Gizmos.color = gizmoWireColor;
        Gizmos.DrawWireCube(zoneCenter, cullBox);
    }
}
