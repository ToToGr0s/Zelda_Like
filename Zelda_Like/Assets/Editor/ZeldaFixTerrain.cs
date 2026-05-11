using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class ZeldaFixTerrain
{
    private const string MenuPath = "Zelda/Fix Terrain - Remove Missing Trees";
    private const string DialogTitle = "Zelda Fix Terrain";

    private struct FixResult
    {
        public int terrainAssetsScanned;
        public int terrainAssetsModified;
        public int removedPrototypes;
        public int removedInstances;
    }

    [MenuItem(MenuPath)]
    public static void FixTerrainFromMenu()
    {
        FixResult result = FixAllTerrainData();
        EditorUtility.DisplayDialog(
            DialogTitle,
            BuildResultMessage(result),
            "OK");
    }

    [InitializeOnLoadMethod]
    private static void RunFixOnDomainReload()
    {
        EditorApplication.delayCall += RunSilentFix;
    }

    private static void RunSilentFix()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            return;
        }

        FixResult result = FixAllTerrainData();
        Debug.Log("[ZeldaFixTerrain] " + BuildResultMessage(result));
    }

    private static FixResult FixAllTerrainData()
    {
        string[] guids = AssetDatabase.FindAssets("t:TerrainData");
        FixResult result = new FixResult
        {
            terrainAssetsScanned = guids.Length
        };

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            TerrainData td = AssetDatabase.LoadAssetAtPath<TerrainData>(assetPath);
            if (td == null)
            {
                continue;
            }

            if (FixTerrainData(td, ref result))
            {
                EditorUtility.SetDirty(td);
            }
        }

        AssetDatabase.SaveAssets();
        return result;
    }

    private static bool FixTerrainData(TerrainData td, ref FixResult result)
    {
        TreePrototype[] originalPrototypes = td.treePrototypes ?? new TreePrototype[0];
        List<TreePrototype> validPrototypes = new List<TreePrototype>(originalPrototypes.Length);
        Dictionary<int, int> oldToNewPrototypeIndex = new Dictionary<int, int>(originalPrototypes.Length);

        int removedPrototypeCount = 0;
        for (int i = 0; i < originalPrototypes.Length; i++)
        {
            TreePrototype tp = originalPrototypes[i];
            if (tp != null && tp.prefab != null)
            {
                oldToNewPrototypeIndex[i] = validPrototypes.Count;
                validPrototypes.Add(tp);
            }
            else
            {
                removedPrototypeCount++;
            }
        }

        TreeInstance[] originalInstances = td.treeInstances ?? new TreeInstance[0];
        List<TreeInstance> validInstances = new List<TreeInstance>(originalInstances.Length);
        int removedInstanceCount = 0;
        bool instancesChanged = false;

        for (int i = 0; i < originalInstances.Length; i++)
        {
            TreeInstance treeInstance = originalInstances[i];
            int oldPrototypeIndex = treeInstance.prototypeIndex;

            if (oldToNewPrototypeIndex.TryGetValue(oldPrototypeIndex, out int newPrototypeIndex) && newPrototypeIndex < validPrototypes.Count)
            {
                if (newPrototypeIndex != oldPrototypeIndex)
                {
                    treeInstance.prototypeIndex = newPrototypeIndex;
                    instancesChanged = true;
                }

                validInstances.Add(treeInstance);
            }
            else
            {
                removedInstanceCount++;
                instancesChanged = true;
            }
        }

        bool prototypesChanged = removedPrototypeCount > 0;
        bool modified = prototypesChanged || instancesChanged;
        if (!modified)
        {
            return false;
        }

        td.treePrototypes = validPrototypes.ToArray();

        if (instancesChanged)
        {
            td.treeInstances = validInstances.ToArray();
        }

        result.terrainAssetsModified++;
        result.removedPrototypes += removedPrototypeCount;
        result.removedInstances += removedInstanceCount;
        return true;
    }

    private static string BuildResultMessage(FixResult result)
    {
        return string.Format(
            "Scanned {0} TerrainData asset(s). Modified {1}. Removed {2} missing tree prototype(s). Removed {3} invalid tree instance(s).",
            result.terrainAssetsScanned,
            result.terrainAssetsModified,
            result.removedPrototypes,
            result.removedInstances);
    }
}
