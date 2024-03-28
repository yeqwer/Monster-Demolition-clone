using System.Collections;
using UnityEngine;

namespace VoxelTools
{
    public static class VoxelPhysics
    {
        public static void DestroyInRadius(Vector3 position, float radius)
        {
            VoxelPhysicsHelper coroutineExecutor = VoxelPhysicsHelper.Singleton;
            coroutineExecutor.StartCoroutine(DestroyInRadiusCoroutine(position, radius));
        }

        public static IEnumerator DestroyInRadiusCoroutine(Vector3 position, float radius)
        {
            Collider[] colliders = Physics.OverlapSphere(position, radius);

            foreach (Collider collider in colliders)
                if (collider.TryGetComponent(out VoxelModelObject voxelModelObject))
                    voxelModelObject.ReplaceWithFragmented();

            yield return null;

            colliders = Physics.OverlapSphere(position, radius);

            foreach (Collider collider in colliders)
                if (collider.TryGetComponent(out VoxelFragmentObject voxelFragmentObject))
                    voxelFragmentObject.Collapse();
        }

        public static void DestroyInCapsule(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            VoxelPhysicsHelper coroutineExecutor = VoxelPhysicsHelper.Singleton;
            coroutineExecutor.StartCoroutine(DestroyInCapsuleCoroutine(startPosition, endPosition, radius));
        }

        public static IEnumerator DestroyInCapsuleCoroutine(Vector3 startPosition, Vector3 endPosition, float radius)
        {
            Collider[] colliders = Physics.OverlapCapsule(startPosition, endPosition, radius);

            foreach (Collider collider in colliders)
                if (collider.TryGetComponent(out VoxelModelObject voxelModelObject))
                    voxelModelObject.ReplaceWithFragmented();
            yield return null;

            colliders = Physics.OverlapCapsule(startPosition, endPosition, radius);

            foreach (Collider collider in colliders)
                if (collider.TryGetComponent(out VoxelFragmentObject voxelFragmentObject))
                    voxelFragmentObject.Collapse();
        }
    }
}
