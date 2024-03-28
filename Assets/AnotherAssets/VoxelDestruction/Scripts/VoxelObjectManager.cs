using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using VoxelDestruction;

public class VoxelObjectManager : MonoBehaviour
{
    private static VoxelObjectManager _instance;

    public static VoxelObjectManager Instance
    {
        get => _instance ??= new GameObject(nameof(VoxelObjectManager)).AddComponent<VoxelObjectManager>();
    }

    private List<VoxelObject> _voxelObjects = new List<VoxelObject>();

    public bool AllVoxelObjectsBuilt { get; private set; } = false;

    public Action OnAllVoxelObjectsBuilt;

    private void Start()
    {
        _voxelObjects = FindObjectsOfType<VoxelObject>().ToList();
    }

    private bool IsAllVoxelObjectsBuilt()
    {
        return _voxelObjects.All(e => e.isMeshBuilt);
    }

    private void CheckIfAllVoxelObjectsBuilt()
    {
        //if (AllVoxelObjectsBuilt)
        //    return;

        if (!IsAllVoxelObjectsBuilt())
            return;

        AllVoxelObjectsBuilt = true;
        OnAllVoxelObjectsBuilt?.Invoke();

#if UNITY_EDITOR
        Debug.Log($"All voxel objects are built.");
#endif
    }

    public bool AddVoxelObject(VoxelObject voxelObject)
    {
        foreach (VoxelObject trackedVoxelObject in _voxelObjects)
        {
            if (voxelObject == trackedVoxelObject)
                return false;
        }

        _voxelObjects.Add(voxelObject);
        voxelObject.OnMeshBuilt += CheckIfAllVoxelObjectsBuilt;
        return true;
    }

    public bool RemoveVoxelObject(VoxelObject voxelObject)
    {
        foreach (VoxelObject trackedVoxelObject in _voxelObjects)
        {
            if (voxelObject != trackedVoxelObject)
                continue;

            _voxelObjects.Remove(voxelObject);
            voxelObject.OnMeshBuilt -= CheckIfAllVoxelObjectsBuilt;
            return true;
        }

        return false;
    }

#if UNITY_EDITOR
    [ContextMenu("Print all voxel count")]
    public void LogAllVoxelCount()
    {
        MethodInfo getVoxelCountMethod = typeof(VoxelObject).GetMethod("GetVoxelCount", (BindingFlags)~0);

        StringBuilder sb = new StringBuilder();

        sb.Append($"All voxels count on scene is: {_voxelObjects.Sum(e => (int)getVoxelCountMethod.Invoke(e, null))}\n");

        foreach (var voxelObject in _voxelObjects)
            sb.Append($"Object name: {voxelObject.transform.name}; Voxels count: {(int)getVoxelCountMethod.Invoke(voxelObject, null)}\n");

        Debug.Log(sb.ToString());
    }
#endif
}
