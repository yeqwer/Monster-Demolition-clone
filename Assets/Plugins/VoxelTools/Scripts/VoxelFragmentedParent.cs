using UnityEngine;

namespace VoxelTools
{
    public class VoxelFragmentedParent : MonoBehaviour
    {
        //private ICollapsable[] _collapsables;

        private int _maxCollapsablesCount;

        private int _currentCollapsablesCount;

        private float _aliveFragmentsRatio => (float)_currentCollapsablesCount / _maxCollapsablesCount;

        private float _collapseAllAtRatio = 0.5f;

        private void Start()
        {
            //_collapsables = GetComponentsInChildren<ICollapsable>();

            _maxCollapsablesCount = transform.childCount;
            _currentCollapsablesCount = _maxCollapsablesCount;
        }

        public void RefreshChilds()
        {
            _currentCollapsablesCount = transform.childCount;

            if (_aliveFragmentsRatio > _collapseAllAtRatio)
                return;

            ICollapsable[] collapsables = GetComponentsInChildren<ICollapsable>();

            foreach (ICollapsable collapsable in collapsables)
                collapsable.CollapseEntirely();
        }
    }
}
