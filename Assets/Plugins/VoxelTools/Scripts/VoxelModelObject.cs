using System.Collections;
using UnityEngine;

namespace VoxelTools
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    public class VoxelModelObject : MonoBehaviour, ICollapsable
    {
        private MeshFilter _meshFilter;

        private MeshCollider _meshCollider;

        public GameObject FragmentedObjectPrefab;

        public float RemainingFragmentsRatio => 1f;

        private bool _isCollapsed { get; set; } = false;

        private float _disappearDelaySeconds = 5f;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();

            _meshCollider.sharedMesh = _meshFilter.mesh;
            _meshCollider.convex = true;
        }

        private void Start()
        {
            if (transform.parent.GetComponent<VoxelFragmentedParent>() == null)
                transform.parent.gameObject.AddComponent<VoxelFragmentedParent>();
        }

        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.collider.tag != "Player")
        //        return;

        //    ReplaceWithFragmented();
        //}

        public void ReplaceWithFragmented()
        {
            if (transform.parent == null)
                return;

            Transform fragmentedObject = Instantiate(FragmentedObjectPrefab).transform;
            fragmentedObject.parent = transform.parent;
            fragmentedObject.localPosition = Vector3.zero;
            fragmentedObject.localRotation = transform.localRotation * Quaternion.Euler(90, 0, 0);
            fragmentedObject.localScale = transform.localScale;

            VoxelRootObject voxelRootObject = GetComponentInParent<VoxelRootObject>();
            if (voxelRootObject != null)
            {
                transform.parent = null;
                voxelRootObject.RefreshFragments();
            }

            Destroy(gameObject);
        }

#if UNITY_EDITOR
        [ContextMenu("Collapse entirely")]
#endif
        public void CollapseEntirely()
        {
            if (_isCollapsed)
                return;

            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();

            _isCollapsed = true;

            StartCoroutine(DestroyCoroutine(_disappearDelaySeconds));
        }

        private IEnumerator DestroyCoroutine(float secondsDelay)
        {
            yield return new WaitForSeconds(secondsDelay);

            Destroy(gameObject);
        }
    }
}
