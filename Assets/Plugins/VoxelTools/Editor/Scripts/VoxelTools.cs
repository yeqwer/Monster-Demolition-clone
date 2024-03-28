using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VoxReader.Interfaces;

namespace VoxelTools.Editor
{
    public class VoxelTools : EditorWindow
    {
        private VoxelToolsSettings _settings;

        private const string _voxelFileExtension = "vox";

        [MenuItem("Voxel/Voxel Tools")]
        private static void ShowWindow()
        {
            EditorWindow window = GetWindow(typeof(VoxelTools));
            window.titleContent = new GUIContent("Voxel Tools");
            window.Show();
        }

        private void OnEnable()
        {
            _settings = VoxelToolsSettings.Load();
        }

        private void OnDisable()
        {
            _settings.Save();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            {
                GUILayout.Label("Convert voxel file to mesh", EditorStyles.boldLabel);

                EditorGUILayout.Separator();

                GUILayout.Label("Voxel file(s) path:");
                GUI.enabled = false;
                EditorGUILayout.TextField(string.Empty, _settings.VoxelPath);
                GUI.enabled = true;

                if (GUILayout.Button("Browse folder"))
                {
                    _settings.VoxelPath = EditorUtility.OpenFolderPanel("Select Folder", _settings.VoxelPath, string.Empty);
                }

                if (GUILayout.Button("Browse file"))
                {
                    _settings.VoxelPath = EditorUtility.OpenFilePanel("Select File", _settings.VoxelPath, _voxelFileExtension);
                }

                EditorGUILayout.Separator();

                GUILayout.Label("Output file(s) path:");
                GUI.enabled = false;
                EditorGUILayout.TextField(string.Empty, _settings.OutputPath);
                GUI.enabled = true;

                if (GUILayout.Button("Browse folder"))
                {
                    _settings.OutputPath = EditorUtility.OpenFolderPanel("Select Folder", _settings.OutputPath, string.Empty);
                }

                EditorGUILayout.Separator();

                GUILayout.Label("Operations");
                if (_settings.VoxelPath == string.Empty || _settings.OutputPath == string.Empty)
                {
                    EditorGUILayout.HelpBox("First you need to select input path and output path", MessageType.Info);
                    GUI.enabled = false;
                }

                if (GUILayout.Button("Extract mesh"))
                {
                    string projectRelativePath = FileUtil.GetProjectRelativePath(_settings.OutputPath);

                    IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    Mesh[] allMesh = VoxUtil.ExtractAllMesh(voxFile.Models);

                    for (int i = 0; i < allMesh.Length; i++)
                    {
                        AssetDatabase.CreateAsset(allMesh[i], $"{projectRelativePath}/Mesh_{i}.asset");
                    }
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Extract models"))
                {
                    IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    VoxUtil.GenerateGameObject(VoxUtil.ExtractAllMesh(voxFile.Models), "RootObject", "Models");
                }

                if (GUILayout.Button("Generate prefab"))
                {
                    IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    string projectRelativePath = FileUtil.GetProjectRelativePath(_settings.OutputPath);

                    Mesh[] allMesh = VoxUtil.ExtractAllMesh(voxFile.Models);

                    VoxUtil.GeneratePrefab(allMesh, projectRelativePath, "Prefab");
                }

                if (GUILayout.Button("Calculate fragments and extract all fragments mesh"))
                {
                    IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    string projectRelativePath = FileUtil.GetProjectRelativePath(_settings.OutputPath);

                    FragmentCalculationSettings calculationSettings = new FragmentCalculationSettings()
                    {
                        FragSphereMinRadius = 3,
                        FragSphereMaxRadius = 6,
                    };

                    Fragment3D[] allFragments = VoxUtil.CalculateFragments(voxFile.Models[0], calculationSettings);

                    Mesh[] allMesh = VoxUtil.CreateMeshForFragments(allFragments);

                    for (int i = 0; i < allMesh.Length; i++)
                    {
                        AssetDatabase.CreateAsset(allMesh[i], $"{projectRelativePath}/Mesh_{i}.asset");
                    }
                    AssetDatabase.SaveAssets();
                }

                if (GUILayout.Button("Generate fragmented game object"))
                {
                    IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    string projectRelativePath = FileUtil.GetProjectRelativePath(_settings.OutputPath);

                    FragmentCalculationSettings calculationSettings = new FragmentCalculationSettings()
                    {
                        FragSphereMinRadius = 3,
                        FragSphereMaxRadius = 6,
                    };

                    VoxUtil.GenerateFragmentedGameObject(voxFile.Models, "RootObject", calculationSettings);
                }

                if (GUILayout.Button("Generate fragmented prefab"))
                {
                    IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    string projectRelativePath = FileUtil.GetProjectRelativePath(_settings.OutputPath);

                    FragmentCalculationSettings calculationSettings = new FragmentCalculationSettings()
                    {
                        FragSphereMinRadius = 3,
                        FragSphereMaxRadius = 6,
                    };

                    VoxUtil.GenerateFragmentedPrefab(voxFile.Models, projectRelativePath, "RootObject", calculationSettings);
                }

                if (GUILayout.Button("Generate simple and fragmented prefabs"))
                {
                    IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    string projectRelativePath = FileUtil.GetProjectRelativePath(_settings.OutputPath);

                    string prefabName = "RootObject";

                    VoxUtil.GeneratePrefab(voxFile.Models, projectRelativePath, prefabName + "_Simple");

                    FragmentCalculationSettings calculationSettings = new FragmentCalculationSettings()
                    {
                        FragSphereMinRadius = 3,
                        FragSphereMaxRadius = 6,
                    };

                    VoxUtil.GenerateFragmentedPrefab(voxFile.Models, projectRelativePath, prefabName + "_Fragmented", calculationSettings);
                }

                if (GUILayout.Button("Generate destructible prefab"))
                {
                    //IVoxFile voxFile = VoxReader.VoxReader.Read(_settings.VoxelPath, useBSA: false);

                    //string projectRelativePath = FileUtil.GetProjectRelativePath(_settings.OutputPath);

                    //string prefabName = "RootObject";

                    //FragmentCalculationSettings calculationSettings = new FragmentCalculationSettings()
                    //{
                    //    FragSphereMinRadius = 3,
                    //    FragSphereMaxRadius = 6,
                    //};

                    //VoxUtil.GenerateDestructiblePrefab(voxFile.Models, projectRelativePath, prefabName, calculationSettings);

                    string[] allVoxFilesNames;

                    if (_settings.VoxelPath.EndsWith(".vox"))
                    {
                        allVoxFilesNames = new[] { _settings.VoxelPath };
                    }
                    else
                    {
                        DirectoryInfo directoryInfo = new DirectoryInfo(_settings.VoxelPath);
                        FileInfo[] voxFiles = directoryInfo.GetFiles("*.vox");

                        if (voxFiles.Length == 0)
                            throw new ArgumentException($"There are no .vox files at path: {_settings.VoxelPath}");

                        allVoxFilesNames = voxFiles.Select(f => f.Name).ToArray();
                    }

                    foreach (string voxFileName in allVoxFilesNames)
                    {
                        string voxFileNameNoExt = voxFileName.Remove(voxFileName.IndexOf(".vox"));
                        string fullInputPath = Path.Combine(_settings.VoxelPath, voxFileName);
                        string relativeInputPath = FileUtil.GetProjectRelativePath(fullInputPath);

                        IVoxFile voxFile = VoxReader.VoxReader.Read(fullInputPath, useBSA: false);

                        FragmentCalculationSettings fragmentCalculationSettings = new FragmentCalculationSettings()
                        {
                            FragSphereMinRadius = 3,
                            FragSphereMaxRadius = 6,
                        };

                        string fullOutputPath = Path.Combine(_settings.OutputPath, voxFileNameNoExt);
                        string relativeOutputFolder = FileUtil.GetProjectRelativePath(_settings.OutputPath);
                        string relativeOutputPath = FileUtil.GetProjectRelativePath(fullOutputPath);
                        if (!AssetDatabase.IsValidFolder(relativeOutputPath))
                        {
                            string guid = AssetDatabase.CreateFolder(relativeOutputFolder, voxFileNameNoExt);
                        }

                        try
                        {
                            VoxUtil.GenerateDestructiblePrefab(voxFile.Models, relativeOutputPath, voxFileNameNoExt, fragmentCalculationSettings);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"{ex.GetType()} thrown while trying to generate destructible prefab. Path: {Path.Combine(relativeInputPath, voxFileName)}\n" +
                                $"{ex}");
                        }
                    }
                }
                GUI.enabled = true;

                EditorGUILayout.Separator();

                GUILayout.Label("Fixing old prefabs", EditorStyles.boldLabel);

                EditorGUILayout.Separator();



                GUILayout.Label("Prefab to fix path:");
                GUI.enabled = false;
                EditorGUILayout.TextField(string.Empty, fullPrefabToFixPath);
                GUI.enabled = true;

                if (GUILayout.Button("Browse prefab"))
                {
                    fullPrefabToFixPath = EditorUtility.OpenFilePanel("Select File", "Assets/Resources/Prefabs/Monsters", "prefab");
                }

                EditorGUILayout.Separator();

                GUILayout.Label("New prefab models path:");
                GUI.enabled = false;
                EditorGUILayout.TextField(string.Empty, fullInputDataFolderPath);
                GUI.enabled = true;

                if (GUILayout.Button("Browse folder"))
                {
                    fullInputDataFolderPath = EditorUtility.OpenFolderPanel("Select Folder", _settings.OutputPath, string.Empty);
                }

                EditorGUILayout.Separator();

                if (GUILayout.Button("Fix old prefabs"))
                {
                    string relativePrefabToFixPath = FileUtil.GetProjectRelativePath(fullPrefabToFixPath);
                    string relativeInputDataFolderPath = FileUtil.GetProjectRelativePath(fullInputDataFolderPath);

                    try
                    {
                        VoxUtil.FixOldPrefab(relativePrefabToFixPath, relativeInputDataFolderPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }

                GUILayout.FlexibleSpace();
            }
            GUILayout.EndVertical();
        }

        string fullPrefabToFixPath = string.Empty;
        string fullInputDataFolderPath = string.Empty;

        [Serializable]
        private class VoxelToolsSettings
        {
            public const string SettingsKey = "VoxelTools";
            public string VoxelPath = string.Empty;
            public string OutputPath = string.Empty;

            public static VoxelToolsSettings Load()
            {
                string json = EditorPrefs.GetString(SettingsKey, "");

                VoxelToolsSettings settings;

                if (string.IsNullOrEmpty(json))
                {
                    settings = new VoxelToolsSettings();
                }
                else
                {
                    settings = JsonUtility.FromJson<VoxelToolsSettings>(json);
                }

                if (settings.VoxelPath.EndsWith(".vox"))
                {
                    if (!File.Exists(settings.VoxelPath))
                        settings.VoxelPath = string.Empty;
                }
                else
                {
                    if (!Directory.Exists(settings.VoxelPath))
                        settings.VoxelPath = string.Empty;
                }

                if (!Directory.Exists(settings.OutputPath))
                    settings.OutputPath = string.Empty;

                return settings;
            }

            public void Save()
            {
                EditorPrefs.SetString(SettingsKey, JsonUtility.ToJson(this));
            }

            public static void Clear()
            {
                EditorPrefs.DeleteKey(SettingsKey);
            }
        }
    }
}