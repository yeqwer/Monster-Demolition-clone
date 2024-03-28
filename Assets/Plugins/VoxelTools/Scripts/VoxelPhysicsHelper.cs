using UnityEngine;

namespace VoxelTools
{
    public class VoxelPhysicsHelper : MonoBehaviour
    {
        private static VoxelPhysicsHelper _singleton;

        public static VoxelPhysicsHelper Singleton
        {
            get => _singleton ??= new GameObject(nameof(VoxelPhysicsHelper)).AddComponent<VoxelPhysicsHelper>();
        }

        private void OnDestroy()
        {
            _singleton = null;
        }
    }
}
