using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace VoxelDestruction
{
     [BurstCompile]
    public struct CollapseJob : IJob
    {
        [ReadOnly]
        public NativeArray<int> voxels;

        public NativeList<int> sortedVoxels;
        public int3 size;

        public int left;
        
        public void Execute()
        {
            int _left = left;
            
            int startIndex;

            for (startIndex = 0; startIndex < voxels.Length; startIndex++)
                if (voxels[startIndex] == 1)
                    goto exitLoop;
            
            exitLoop:

            var tempMask = new NativeList<int>(Allocator.Temp);
            var tempChecked = new NativeList<int>(Allocator.Temp);
            tempMask.Add(startIndex);
            _left--;
            
            int found = 1;
            while (found > 0 && _left > 0)
            {
                found = 0;
                
                for (int i = 0; i < tempMask.Length; i++)
                {
                    if (tempChecked.Contains(i))
                        continue;

                    for (int j = 0; j < 6; j++)
                    {
                        int3 dir = 0;
                        dir[j % 3] = 1;
                        if (j >= 3)
                            dir = -dir;

                        int3 d = To3D(tempMask[i]) + dir;
                        
                        if (d.x < 0 || d.x >= size.x || d.y < 0 || d.y >= size.y || d.z < 0 || d.z >= size.z)
                            continue;
     
                        int targetIndex = To1D(d);

                        if (voxels[targetIndex] == 1 && !tempMask.Contains(targetIndex))
                        {
                            tempMask.Add(targetIndex);
                            _left--;
                            found++;
                        }
                    }

                    tempChecked.Add(i);
                }
            } 
            
            for (int i = 0; i < tempMask.Length; i++)
                sortedVoxels.Add(tempMask[i]);
            
            tempMask.Clear();
            
            if (left > 0)
            {
                sortedVoxels.Add(-1);
                
                for (startIndex = 0; startIndex < voxels.Length; startIndex++)
                    if (voxels[startIndex] == 1 && !sortedVoxels.Contains(startIndex))
                        goto exitLoop;
            }
        }
        
        private int To1D(int3 index)
        {
            return (int)(index.x + size.x * (index.y + size.y * index.z));
        }
        
        private int3 To3D(long index)
        {
            int z = (int)index / (size.x * size.y);
            int idx = (int)index - (z * size.x * size.y);
            int y = idx / size.x;
            int x = idx % size.x;
            return new int3(x, y, z);
        }
    }   
}
