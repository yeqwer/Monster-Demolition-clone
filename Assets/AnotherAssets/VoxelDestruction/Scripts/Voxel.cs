using UnityEngine;

namespace VoxelDestruction
{
    public struct Voxel
    {
        public Color color;
        public bool active;

        public Voxel(Color _color, bool _active)
        {
            color = new Color32((byte)_color.r, (byte)_color.g, (byte)_color.b, 255);
            active = _active;
        }
        
        public Voxel(Voxel vox)
        {
            color = vox.color;
            active = vox.active;
        }
    }
}