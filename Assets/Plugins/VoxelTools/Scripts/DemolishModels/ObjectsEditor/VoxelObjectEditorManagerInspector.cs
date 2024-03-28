#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VoxelDestruction;

[CustomEditor(typeof(VoxelObjectEditorManager))]
public class VoxelObjectEditorManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Build all children mesh"))
        {
            GameObject selectedGO = Selection.activeGameObject;

            VoxelObject[] voxelObjects = selectedGO.GetComponentsInChildren<VoxelObject>();

            Type voxelObjectInspectorType = typeof(VoxelObjectInspector);

            MethodInfo UpdateMeshMethod = voxelObjectInspectorType.GetMethod("BuildMesh", (BindingFlags)(~0));

            foreach (VoxelObject voxelObject in voxelObjects)
            {
                VoxelObjectInspector voxObjInspector = (VoxelObjectInspector)CreateEditor(voxelObject, voxelObjectInspectorType);

                UpdateMeshMethod.Invoke(voxObjInspector, null);
            }
        }

        if (GUILayout.Button("Clear all children mesh"))
        {
            GameObject selectedGO = Selection.activeGameObject;

            VoxelObject[] voxelObjects = selectedGO.GetComponentsInChildren<VoxelObject>();

            Type voxelObjectInspectorType = typeof(VoxelObjectInspector);

            MethodInfo ClearMeshMethod = voxelObjectInspectorType.GetMethod("ClearMesh", (BindingFlags)(~0));

            foreach (VoxelObject voxelObject in voxelObjects)
            {
                VoxelObjectInspector voxObjInspector = (VoxelObjectInspector)CreateEditor(voxelObject, voxelObjectInspectorType);

                ClearMeshMethod.Invoke(voxObjInspector, null);
            }
        }

        if (GUILayout.Button("Calculate all children fragments"))
        {
            GameObject selectedGO = Selection.activeGameObject;

            VoxelObject[] voxelObjects = selectedGO.GetComponentsInChildren<VoxelObject>();

            Type voxelObjectInspectorType = typeof(VoxelObjectInspector);

            MethodInfo RecalculateFragmentsMethod = voxelObjectInspectorType.GetMethod("CalculateFragment", (BindingFlags)(~0));

            foreach (VoxelObject voxelObject in voxelObjects)
            {
                VoxelObjectInspector voxObjInspector = (VoxelObjectInspector)CreateEditor(voxelObject, voxelObjectInspectorType);

                RecalculateFragmentsMethod.Invoke(voxObjInspector, null);
            }
        }
    }
}
#endif