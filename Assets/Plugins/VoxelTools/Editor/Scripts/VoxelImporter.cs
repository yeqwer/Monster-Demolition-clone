using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace VoxelTools.Editor
{
    [ScriptedImporter(0, "vox")]
    public class VoxelImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            byte[] data = File.ReadAllBytes(ctx.assetPath);
            VoxelAsset voxelAsset = ScriptableObject.CreateInstance<VoxelAsset>();
            voxelAsset.Bytes = data;
            ctx.AddObjectToAsset("vox", voxelAsset);
            ctx.SetMainObject(voxelAsset);
        }
    }
}