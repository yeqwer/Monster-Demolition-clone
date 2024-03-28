using UnityEngine;

namespace VoxelTools.Editor
{
    public class VoxelAsset : ScriptableObject
    {
        [field: HideInInspector]
        [field: SerializeField]
        public byte[] Bytes { get; set; }
    }
}