using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections;
using UnityEditor;
using UnityEngine;
using VoxelDestruction;
using VoxReader.Interfaces;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Voxel1D = VoxelDestruction.Voxel;
using Voxel3D = VoxReader.Voxel;

namespace VoxelTools.Editor
{
    public static class VoxUtil
    {
        public static Mesh ExtractMesh(IModel model)
        {
            VoxelData data = VoxToVoxelData.GenerateVoxelData(model);

            MeshBuilderSafe safeBuilder = new MeshBuilderSafe();
            safeBuilder.StartMeshDrawing(data, Allocator.Persistent);

            Mesh mesh = safeBuilder.GetVoxelObject(use32BitInt: false);
            safeBuilder.Dispose();

            return mesh;
        }

        public static Mesh[] ExtractAllMesh(IModel[] models)
        {
            Mesh[] meshes = new Mesh[models.Length];

            for (int i = 0; i < models.Length; i++)
            {
                meshes[i] = ExtractMesh(models[i]);
            }

            return meshes;
        }

        public static GameObject GenerateGameObject(Mesh mesh)
        {
            GameObject gameObject = new GameObject("Generated Game Object");
            //gameObject.transform.localPosition = new Vector3(-0.5f, 0.5f, 0.5f);
            gameObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            gameObject.transform.localScale = Vector3.one;

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            //meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            meshRenderer.material = Resources.Load<Material>("VoxelTools/Materials/DefaultVoxelMaterial");
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            return gameObject;
        }

        public static GameObject GenerateGameObject(Mesh[] allMesh, string parentName, string childName)
        {
            GameObject parentObject = new GameObject(parentName);

            for (int i = 0; i < allMesh.Length; i++)
            {
                GameObject childObject = GenerateGameObject(allMesh[i]);
                childObject.name = $"{childName}_{i}";
                childObject.transform.parent = parentObject.transform;
            }

            return parentObject;
        }

        public static GameObject GeneratePrefab(Mesh[] allMesh, string prefabPath, string prefabName)
        {
            string meshFolderName = "Mesh";
            string meshPath = Path.Combine(prefabPath, meshFolderName);
            if (!AssetDatabase.IsValidFolder(meshPath))
            {
                AssetDatabase.CreateFolder(prefabPath, meshFolderName);
            }

            for (int i = 0; i < allMesh.Length; i++)
            {
                AssetDatabase.CreateAsset(allMesh[i], $"{meshPath}/{prefabName}_Mesh_{i}.asset");
            }
            AssetDatabase.SaveAssets();

            GameObject generatedGameObject = GenerateGameObject(allMesh, prefabName, "Fragment");


            for (int i = 0; i < generatedGameObject.transform.childCount; i++)
            {
                Transform currentFragment = generatedGameObject.transform.GetChild(i);

                currentFragment.gameObject.AddComponent<VoxelModelObject>();
            }


            GameObject generatedPrefab = PrefabUtility.SaveAsPrefabAsset(generatedGameObject, $"{prefabPath}/{generatedGameObject.name}.prefab");

            Object.DestroyImmediate(generatedGameObject);

            return generatedPrefab;
        }

        public static GameObject GeneratePrefab(IModel[] models, string prefabPath, string prefabName)
        {
            Mesh[] allMesh = ExtractAllMesh(models);

            return GeneratePrefab(allMesh, prefabPath, prefabName);
        }

        public static Fragment3D[] CalculateFragments(IModel model, FragmentCalculationSettings settings)
        {
            List<Voxel3D[]> allFragments = new List<Voxel3D[]>();
            List<Voxel3D> currentFragment = new List<Voxel3D>();

            VoxelData data = VoxToVoxelData.GenerateVoxelData(model);

            int stackOverflow = 0;
            int currentVoxelIndex = GetFirstVoxelIndex1D(data);
            while (currentVoxelIndex != -1 && stackOverflow < data.Blocks.Length)
            {
                SimpleSphere fragmentSphere = new SimpleSphere(CoordinateConverter.To3D(currentVoxelIndex, data.Size),
                    Random.Range(settings.FragSphereMinRadius, settings.FragSphereMaxRadius));

                for (int i = 0; i < data.Blocks.Length; i++)
                {
                    if (data.Blocks[i].active && fragmentSphere.IsInsideSphere(CoordinateConverter.To3D(i, data.Size)))
                    {
                        Color currentVoxelColor = data.Blocks[i].color;

                        Voxel3D currentVoxel = new Voxel3D(CoordinateConverter.To3D(i, data.Size),
                            new Color(currentVoxelColor.r * 255, currentVoxelColor.g * 255, currentVoxelColor.b * 255, 255));

                        currentFragment.Add(currentVoxel);

                        data.Blocks[i] = new Voxel1D(Color.black, false);
                    }
                }

                allFragments.Add(currentFragment.ToArray());
                currentFragment.Clear();

                currentVoxelIndex = GetFirstVoxelIndex1D(data);
                stackOverflow++;
            }

            if (stackOverflow >= data.Blocks.Length)
            {
                throw new Exception($"Calculating fragments for {model.Name} caused Stackoverflow, make sure values are correct!");
            }

            return allFragments.Select(fragArr => new Fragment3D(fragArr)).ToArray();
        }

        public static Mesh[] CreateMeshForFragments(Fragment3D[] fragments)
        {
            Mesh[] allFragmentsMesh = new Mesh[fragments.Length];

            for (int i = 0; i < fragments.Length; i++)
            {
                Fragment3D fragment = fragments[i];

                Vector3Int fragmentSize = GetSize(fragment.Voxels);

                if (fragmentSize == Vector3Int.zero)
                    throw new Exception("Fragment Size can not be zero");

                Voxel1D[] final = new Voxel1D[fragmentSize.x * fragmentSize.y * fragmentSize.z];

                for (int j = 0; j < fragment.Voxels.Length; j++)
                {
                    Voxel3D currentVoxel = fragment.Voxels[j];

                    try
                    {
                        final[CoordinateConverter.To1D(Vector3Int.FloorToInt(currentVoxel.Position), fragmentSize)] = new Voxel1D(currentVoxel.Color, true);
                    }
                    catch (Exception)
                    {
                        throw new Exception($"Error: Position {currentVoxel.Position}, " +
                            $"Final Index {CoordinateConverter.To1D(Vector3Int.FloorToInt(currentVoxel.Position), fragmentSize)}, " +
                            $"Data Index {j}, " +
                            $"Final Length {fragmentSize.x * fragmentSize.y * fragmentSize.z}");
                    }
                }

                MeshBuilderSafe meshBuilder = new MeshBuilderSafe();

                meshBuilder.StartMeshDrawing(new VoxelData(fragmentSize, final), Allocator.Persistent);

                allFragmentsMesh[i] = meshBuilder.GetVoxelObject(false);

                meshBuilder.Dispose();

                if (allFragmentsMesh[i].vertices.Length == 0 || allFragmentsMesh[i].triangles.Length == 0)
                {
                    throw new Exception("MeshBuilder returned invalid fragment Mesh!");
                }
            }

            return allFragmentsMesh;
        }

        public static GameObject GenerateFragmentedGameObject(IModel[] models, string name, FragmentCalculationSettings settings)
        {
            GameObject parent = new GameObject(name);

            for (int i = 0; i < models.Length; i++)
            {
                IModel model = models[i];

                Fragment3D[] fragments = CalculateFragments(model, settings);

                Mesh[] allFragmentsMesh = CreateMeshForFragments(fragments);

                GameObject generatedObject = GenerateGameObject(allFragmentsMesh, $"Model_{model.Id}", "Fragment");
                generatedObject.transform.parent = parent.transform;
            }

            return parent;
        }

        public static GameObject GenerateFragmentedPrefab(IModel[] models, string prefabPath, string prefabName, FragmentCalculationSettings settings)
        {
            string meshFolderName = "Mesh";
            string meshPath = Path.Combine(prefabPath, meshFolderName);
            if (!AssetDatabase.IsValidFolder(meshPath))
            {
                AssetDatabase.CreateFolder(prefabPath, meshFolderName);
            }

            GameObject rootGameObject = new GameObject(prefabName);

            for (int i = 0; i < models.Length; i++)
            {
                string modelFolderName = $"Model_{i}";
                string modelPath = Path.Combine(meshPath, modelFolderName);
                if (!AssetDatabase.IsValidFolder(modelPath))
                {
                    AssetDatabase.CreateFolder(meshPath, modelFolderName);
                }

                IModel model = models[i];

                Fragment3D[] fragments = CalculateFragments(model, settings);

                Mesh[] fragmentsMesh = CreateMeshForFragments(fragments);

                for (int j = 0; j < fragmentsMesh.Length; j++)
                {
                    AssetDatabase.CreateAsset(fragmentsMesh[j], $"{modelPath}/{prefabName}_{modelFolderName}_Fragment_{j}_Mesh.asset");
                }

                GameObject generatedModelGameObject = GenerateGameObject(fragmentsMesh, modelFolderName, "Fragment");
                generatedModelGameObject.transform.parent = rootGameObject.transform;


                generatedModelGameObject.AddComponent<VoxelFragmentedObject>();

                Transform generatedModelTransform = generatedModelGameObject.transform;
                for (int j = 0; j < generatedModelTransform.childCount; j++)
                {
                    Transform currentFragment = generatedModelTransform.GetChild(j);

                    currentFragment.gameObject.AddComponent<VoxelFragmentObject>();
                }


                PrefabUtility.SaveAsPrefabAsset(generatedModelGameObject, $"{meshPath}/{prefabName}_{modelFolderName}.prefab");
            }
            AssetDatabase.SaveAssets();

            GameObject generatedPrefab = PrefabUtility.SaveAsPrefabAsset(rootGameObject, $"{prefabPath}/{prefabName}.prefab");

            Object.DestroyImmediate(rootGameObject);

            return generatedPrefab;
        }

        public static void GenerateDestructiblePrefab(IModel[] models, string prefabPath, string prefabName, FragmentCalculationSettings settings)
        {
            GenerateFragmentedPrefab(models, prefabPath, prefabName, settings);

            GeneratePrefab(models, prefabPath, prefabName);
            GameObject simpleObjectPrefab = PrefabUtility.LoadPrefabContents(Path.Combine(prefabPath, $"{prefabName}.prefab"));

            VoxelModelObject[] voxelModelObjects = simpleObjectPrefab.GetComponentsInChildren<VoxelModelObject>();

            for (int i = 0; i < voxelModelObjects.Length; i++)
            {
                string fragmentedObjectPath = Path.Combine(prefabPath, "Mesh", $"{prefabName}_Model_{i}.prefab");

                GameObject fragmentedObjectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(fragmentedObjectPath);

                if (fragmentedObjectPrefab == null)
                    Debug.LogError($"Loading fragmented object prefab failed. Object path: {fragmentedObjectPath}");

                voxelModelObjects[i].FragmentedObjectPrefab = fragmentedObjectPrefab;
            }

            PrefabUtility.SaveAsPrefabAsset(simpleObjectPrefab, Path.Combine(prefabPath, $"{prefabName}.prefab"));
            PrefabUtility.UnloadPrefabContents(simpleObjectPrefab);

            AssetDatabase.SaveAssets();
        }

        public static void FixOldPrefab(string oldPrefabPath, string rootFolderWithNewPrefabs)
        {
            if (!oldPrefabPath.EndsWith(".prefab"))
                throw new ArgumentException($"{nameof(oldPrefabPath)} has to be prefab type.");

            if (Regex.IsMatch(rootFolderWithNewPrefabs, @".*\..*$"))
                throw new ArgumentException($"{nameof(rootFolderWithNewPrefabs)} has to point to folder, not file.");

            GameObject oldPrefab = null;

            try
            {
                oldPrefab = PrefabUtility.LoadPrefabContents(oldPrefabPath);

                VoxelObject[] allVoxelObjects = oldPrefab.GetComponentsInChildren<VoxelObject>();

                for (int i = 0; i < allVoxelObjects.Length; i++)
                {
                    VoxelObject currentVoxelObject = allVoxelObjects[i];

                    while (currentVoxelObject.transform.childCount > 0)
                    {
                        Object.DestroyImmediate(currentVoxelObject.transform.GetChild(0).gameObject);
                    }

                    string voxFilePath = currentVoxelObject.path;
                    string voxFileName = Regex.Match(voxFilePath, @".*[/\\](.*)\.vox").Groups[1].Value;

                    string newPrefabPath = Path.Combine(rootFolderWithNewPrefabs, voxFilePath.Replace(".vox", string.Empty), string.Concat(voxFileName, ".prefab"));

                    GameObject newModelPrefabReference = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);
                    GameObject newModelPrefab = Object.Instantiate(newModelPrefabReference, currentVoxelObject.transform);

                    // Removing VoxelObject component since now there is no need for it.
                    //Object.DestroyImmediate(currentVoxelObject);
                    currentVoxelObject.enabled = false;
                }

                // Removing old components.
                //Object.DestroyImmediate(oldPrefab.GetComponent<DemolishCounter>());
                //Object.DestroyImmediate(oldPrefab.GetComponent<RealTimeSettingsToRigger>());
                //Object.DestroyImmediate(oldPrefab.GetComponent<FakeRigAndDestroy>());

                //oldPrefab.GetComponent<DemolishCounter>().enabled = false;
                oldPrefab.GetComponent<RealTimeSettingsToRigger>().enabled = false;
                //oldPrefab.GetComponent<FakeRigAndDestroy>().enabled = false;

                // Adding new components.
                if (oldPrefab.GetComponent<VoxelRootObject>() == null)
                    oldPrefab.AddComponent<VoxelRootObject>();

                PrefabUtility.SaveAsPrefabAsset(oldPrefab, oldPrefabPath);
                Debug.Log($"Successfully updated and saved prefab at path {oldPrefabPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
            finally
            {
                if (oldPrefab != null)
                    PrefabUtility.UnloadPrefabContents(oldPrefab);
            }
        }

        private static int GetFirstVoxelIndex1D(VoxelData data)
        {
            Vector3Int nearest = new Vector3Int(-1, -1, -1);
            for (int i = 0; i < data.Blocks.Length; i++)
            {
                if (!data.Blocks[i].active)
                    continue;

                if (nearest.x == -1)
                {
                    nearest = Vector3Int.RoundToInt(CoordinateConverter.To3D(i, data.Size));
                    continue;
                }

                if (Vector3.Min(nearest, CoordinateConverter.To3D(i, data.Size)) != nearest)
                    nearest = Vector3Int.RoundToInt(CoordinateConverter.To3D(i, data.Size));
            }

            if (nearest.x == -1)
                return -1;

            return CoordinateConverter.To1D(nearest, data.Size);
        }

        private static Vector3 GetMinVoxelPosition3D(Voxel3D[] array)
        {
            Vector3 min = array[0].Position;

            for (int i = 1; i < array.Length; i++)
            {
                min.x = array[i].Position.x < min.x ? array[i].Position.x : min.x;
                min.y = array[i].Position.y < min.y ? array[i].Position.y : min.y;
                min.z = array[i].Position.z < min.z ? array[i].Position.z : min.z;
            }

            return min;
        }

        private static Vector3Int GetSize(Voxel3D[] array)
        {
            Vector3Int max = Vector3Int.one;

            for (int i = 0; i < array.Length; i++)
            {
                max.x = array[i].Position.x > max.x ? (int)array[i].Position.x : max.x;
                max.y = array[i].Position.y > max.y ? (int)array[i].Position.y : max.y;
                max.z = array[i].Position.z > max.z ? (int)array[i].Position.z : max.z;
            }

            return max + Vector3Int.one;
        }
    }

    public class FragmentCalculationSettings
    {
        public float FragSphereMinRadius;
        public float FragSphereMaxRadius;
    }

    public struct Fragment3D
    {
        public Voxel3D[] Voxels;

        public Fragment3D(Voxel3D[] voxels)
        {
            Voxels = voxels;
        }
    }
}
