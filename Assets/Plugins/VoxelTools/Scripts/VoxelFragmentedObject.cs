using System;
using System.Collections;
using UnityEngine;

namespace VoxelTools
{
    public class VoxelFragmentedObject : MonoBehaviour, ICollapsable
    {
        private VoxelFragmentedParent _voxelFragmentedParent;

        private VoxelFragmentObject[] _allFragments;

        public int MaxFragmentCount { get; private set; }

        public int CurrentFragmentCount => _allFragments.Length;

        public float RemainingFragmentsRatio { get; private set; }

        [SerializeField]
        private float _collapseAtRatio = 0.4f;

        private float _disappearDelaySeconds = 5f;

        private bool _isCollapsedEntirely { get; set; } = false;

        public event Action<float> OnRemainingFragmentsRatioUpdate;

        private void Start()
        {
            MaxFragmentCount = GetComponentsInChildren<VoxelFragmentObject>().Length;

            _voxelFragmentedParent = transform.parent.GetComponent<VoxelFragmentedParent>();

            RefreshFragments();
        }

        public void RefreshFragments()
        {
            if (_isCollapsedEntirely)
                return;

            _allFragments = GetComponentsInChildren<VoxelFragmentObject>();

            RemainingFragmentsRatio = (float)CurrentFragmentCount / MaxFragmentCount;

            OnRemainingFragmentsRatioUpdate?.Invoke(RemainingFragmentsRatio);

            if (_allFragments.Length == 0)
            {
                VoxelRootObject voxelRootObject = transform.GetComponentInParent<VoxelRootObject>();
                transform.parent = null;
                voxelRootObject.RefreshFragments();

                Destroy(gameObject);
            }

            if (RemainingFragmentsRatio <= _collapseAtRatio)
                CollapseEntirely();

            _voxelFragmentedParent?.RefreshChilds();
        }

#if UNITY_EDITOR
        [ContextMenu("Collapse entirely")]
#endif
        public void CollapseEntirely()
        {
            if (_isCollapsedEntirely)
                return;

            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();

            StartCoroutine(DestroyCoroutine(_disappearDelaySeconds));

            foreach (VoxelFragmentObject voxelFragmentObject in _allFragments)
            {
                voxelFragmentObject.IsCollapsed = true;
            }

            _isCollapsedEntirely = true;
        }

        private IEnumerator DestroyCoroutine(float secondsDelay)
        {
            yield return new WaitForSeconds(secondsDelay);

            Destroy(gameObject);
        }
    }
}
