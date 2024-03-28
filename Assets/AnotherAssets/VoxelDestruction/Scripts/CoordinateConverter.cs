using UnityEngine;

namespace VoxelDestruction
{
    public static class CoordinateConverter
    {
        public static Vector3 To3D(long index, Vector3Int size)
        {
            int z = (int)index / (size.x * size.y);
            int idx = (int)index - (z * size.x * size.y);
            int y = idx / size.x;
            int x = idx % size.x;
            return new Vector3(x, y, z);
        }

        public static int To1D(Vector3 index, Vector3 size)
        {
            return (int)(index.x + size.x * (index.y + size.y * index.z));
        }
    }
}