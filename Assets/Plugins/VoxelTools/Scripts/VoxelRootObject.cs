using System;
using UnityEngine;

namespace VoxelTools
{
    public class VoxelRootObject : MonoBehaviour, ICollapsable
    {
        private ICollapsable[] _fragmentCountRatioProviders;

        private int _maxFragmentCount { get; set; }

        public float RemainingFragmentsRatio { get; private set; }

        public float DamageRatio => 1 - RemainingFragmentsRatio;

        public int DamagePercentage => (int)Mathf.Round(DamageRatio * 100);

        public bool IsDestructible { get; set; } = true;

        public bool IsDestroyed { get; private set; }

        public event Action OnDestroyed;

        public event Action<float> OnRemainingFragmentsRatioUpdate;

        private void Awake()
        {
            _maxFragmentCount = GetComponentsInChildren<ICollapsable>().Length;

            RefreshFragments();
        }

        public void RefreshFragments()
        {
            _fragmentCountRatioProviders = GetComponentsInChildren<ICollapsable>();

            RemainingFragmentsRatio = (float)_fragmentCountRatioProviders.Length / _maxFragmentCount;

            OnRemainingFragmentsRatioUpdate?.Invoke(RemainingFragmentsRatio);
        }

#if UNITY_EDITOR
        [ContextMenu("Collapse entirely")]
#endif
        public void CollapseEntirely()
        {
            if (IsDestroyed)
                return;

            ICollapsable[] collapsables = GetComponentsInChildren<ICollapsable>();
            foreach (ICollapsable collapsable in collapsables)
            {
                if (collapsable is VoxelRootObject)
                    continue;

                collapsable.CollapseEntirely();
            }

            RefreshFragments();

            IsDestroyed = true;
            OnDestroyed?.Invoke();
        }
    }
}
