using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VoxelDestruction;

namespace VoxelDestruction
{
    public sealed class CollapseBuilder : IDisposable
    {
        private CollapseJob collapseJob;
        private JobHandle handle;

        public CollapseBuilder(int[] v, Vector3 size)
        {
            int left = v.Count(s => s == 1);

            collapseJob = new CollapseJob()
            {
                voxels = new NativeArray<int>(v, Allocator.Persistent),
                sortedVoxels = new NativeList<int>(Allocator.Persistent),
                size = new int3(size),
                left = left
            };

            handle = collapseJob.Schedule();
        }

        public int[] GetResult()
        {
            handle.Complete();

            return collapseJob.sortedVoxels.ToArray();
        }

        public bool IsCompleted()
        {
            return handle.IsCompleted;
        }

        public void Dispose()
        {
            handle.Complete();
        
            if (collapseJob.voxels.IsCreated)
                collapseJob.voxels.Dispose();
            if (collapseJob.sortedVoxels.IsCreated)
                collapseJob.sortedVoxels.Dispose();
            GC.SuppressFinalize(this);
        }

        ~CollapseBuilder()
        {
            Dispose();
        }
    }
}