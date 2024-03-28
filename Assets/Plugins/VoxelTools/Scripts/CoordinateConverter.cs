using UnityEngine;

namespace VoxelTools
{
    public static class CoordinateConverter
    {
        public static Vector3Int To3D(long index, Vector3Int size)
        {
            int z = (int)index / (size.x * size.y);
            int idx = (int)index - (z * size.x * size.y);
            int y = idx / size.x;
            int x = idx % size.x;
            return new Vector3Int(x, y, z);
        }

        public static int To1D(Vector3Int index, Vector3Int size)
        {
            return (int)(index.x + size.x * (index.y + size.y * index.z));
        }
    }
}