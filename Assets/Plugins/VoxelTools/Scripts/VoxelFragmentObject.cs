using System.Collections;
using UnityEngine;

namespace VoxelTools
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class VoxelFragmentObject : MonoBehaviour
    {
        private MeshFilter _meshFilter;

        private MeshCollider _meshCollider;

        private float _disappearDelaySeconds = 5f;

        public bool IsDestructible
        {
            get
            {
                VoxelRootObject voxelRootObject = gameObject.GetComponentInParent<VoxelRootObject>();

                return voxelRootObject == null || voxelRootObject.IsDestructible;
            }
        }

        public bool IsCollapsed { get; set; } = false;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();

            _meshCollider.convex = true;
            _meshCollider.sharedMesh = _meshFilter.mesh;
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.collider.tag != "Player")
        //        return;

        //    Collapse();
        //}

        public void Collapse()
        {
            if (!IsDestructible)
                return;

            if (IsCollapsed)
                return;

            IsCollapsed = true;

            VoxelFragmentedObject voxelFragmentedObject = transform.parent.GetComponent<VoxelFragmentedObject>();
            transform.parent = null;
            voxelFragmentedObject.RefreshFragments();

            gameObject.AddComponent<Rigidbody>();
            //_rigidbody.isKinematic = false;

            StartCoroutine(DestroyCoroutine(_disappearDelaySeconds));
        }

        private IEnumerator DestroyCoroutine(float secondsDelay)
        {
            yield return new WaitForSeconds(secondsDelay);

            Destroy(gameObject);
        }
    }
}
