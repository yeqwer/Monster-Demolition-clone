using System.Collections.Generic;
using UnityEngine;

namespace VoxelDestruction
{
    public class VoxelObjectData : MonoBehaviour
    {
        private VoxelData _voxelData;

        private Vector3Int _size;

        private Dictionary<int, Bound> _chunksBounds = new Dictionary<int, Bound>();

        private int _currentChunkIndex = -1;

        private VoxelData _currentChunk;

        public VoxelData this[int chunkId]
        {
            get
            {
                if (chunkId == _currentChunkIndex)
                    return _currentChunk;

                // Clearing previous chunk data.
                if (_currentChunkIndex != -1)
                    for (int x = _chunksBounds[_currentChunkIndex].startBound.x; x <= _chunksBounds[_currentChunkIndex].endBound.x; x++)
                        for (int y = _chunksBounds[_currentChunkIndex].startBound.y; y <= _chunksBounds[_currentChunkIndex].endBound.y; y++)
                            for (int z = _chunksBounds[_currentChunkIndex].startBound.z; z <= _chunksBounds[_currentChunkIndex].endBound.z; z++)
                                _currentChunk.Blocks[VoxToVoxelData.To1D(new Vector3Int(x, y, z), _size)] = new Voxel(Color.black, false);

                _currentChunkIndex = chunkId;

                // Writing new chunk data.
                for (int x = _chunksBounds[chunkId].startBound.x; x <= _chunksBounds[chunkId].endBound.x; x++)
                    for (int y = _chunksBounds[chunkId].startBound.y; y <= _chunksBounds[chunkId].endBound.y; y++)
                        for (int z = _chunksBounds[chunkId].startBound.z; z <= _chunksBounds[chunkId].endBound.z; z++)
                            _currentChunk.Blocks[VoxToVoxelData.To1D(new Vector3Int(x, y, z), _size)] =
                                _voxelData.Blocks[VoxToVoxelData.To1D(new Vector3Int(x, y, z), _size)];

                return _currentChunk;
            }
        }

        public void Initialize(VoxelData voxelData, Vector3Int size, Bound chunkBound, int chunkId)
        {
            _voxelData = voxelData;
            _size = size;
            _chunksBounds.Add(chunkId, chunkBound);

            _currentChunk = new VoxelData(_size, new Voxel[_voxelData.Blocks.Length]);
        }

        public void InsertVoxelData(VoxelData voxelData, Bound chunkBound, int chunkId)
        {
            VoxelData.Merge(_voxelData, voxelData);
            _chunksBounds.Add(chunkId, chunkBound);
        }
    }

    public struct Bound
    {
        public Vector3Int startBound;
        public Vector3Int endBound;

        public Bound(Vector3Int startBound, Vector3Int endBound)
        {
            this.startBound = startBound;
            this.endBound = endBound;
        }
    }
}
