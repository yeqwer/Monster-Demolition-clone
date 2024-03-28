using System;
using UnityEngine;

namespace VoxelDestruction
{
    [Serializable]
    public struct VoxelData
    {
        public Vector3Int Size;
        public Voxel[] Blocks;
        public Vector3 PositionOffset;

        public VoxelData(Vector3Int length, Voxel[] blocks, Vector3 positionOffset = default)
        {
            Size = length;

            Blocks = blocks;

            PositionOffset = positionOffset;
        }

        public int[] GetVoxels()
        {
            int[] v = new int[Blocks.Length];

            for (int i = 0; i < v.Length; i++)
                v[i] = Blocks[i].active ? 1 : 0;

            return v;
        }

        public static VoxelData Merge(VoxelData into, VoxelData from)
        {
            if (into.Size != from.Size)
                throw new ArgumentException($"Argument {nameof(into)} has not the same {nameof(into.Size)} as argument {nameof(from)}.");

            for (int i = 0; i < from.Blocks.Length; i++)
            {
                Voxel voxel = from.Blocks[i];

                if (voxel.active is false)
                    continue;

                into.Blocks[i] = voxel;
            }

            return into;
        }

        public static Bound GetBounds(VoxelData voxelData)
        {
            Vector3Int startBound = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            Vector3Int endBound = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);

            for (int x = 0; x < voxelData.Size.x; x++)
            {
                for (int y = 0; y < voxelData.Size.y; y++)
                {
                    for (int z = 0; z < voxelData.Size.z; z++)
                    {
                        if (!voxelData.Blocks[CoordinateConverter.To1D(new Vector3(x, y, z), voxelData.Size)].active)
                            continue;

                        startBound.x = startBound.x > x ? x : startBound.x;
                        startBound.y = startBound.y > y ? y : startBound.y;
                        startBound.z = startBound.z > z ? z : startBound.z;

                        endBound.x = endBound.x < x ? x : endBound.x;
                        endBound.y = endBound.y < y ? y : endBound.y;
                        endBound.z = endBound.z < z ? z : endBound.z;
                    }
                }
            }

            return new Bound(startBound, endBound);
        }

        public static VoxelData RemoveEmptyAreas(VoxelData voxelData)
        {
            Bound bounds = GetBounds(voxelData);

            int startBound = CoordinateConverter.To1D(bounds.startBound, voxelData.Size);
            int endBound = CoordinateConverter.To1D(bounds.endBound, voxelData.Size);

            Voxel[] newVoxelData = new Voxel[endBound - startBound + 1];

            for (int i = 0; i < newVoxelData.Length; i++)
            {
                newVoxelData[i] = voxelData.Blocks[i + startBound];
            }

            // Position offset from the initial position in the world.
            // Z and Y are swapped since MagicaVoxel and Blender has Z as height and Y as depth meanwhile
            // Unity has Y as height and Z as depth.
            Vector3 offset = new Vector3(bounds.startBound.x, bounds.startBound.z, -bounds.startBound.y);

            return new VoxelData(voxelData.Size, newVoxelData, offset);
        }
    }
}