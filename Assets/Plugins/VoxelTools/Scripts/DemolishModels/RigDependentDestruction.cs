using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VoxelTools
{
    [RequireComponent(typeof(VoxelRootObject))]
    [RequireComponent(typeof(RealTimeSettingsToRigger))]
    public class RigDependentDestruction : MonoBehaviour
    {
        private VoxelRootObject _voxelRootObject;

        private RealTimeSettingsToRigger _rigSettings;

        private void Awake()
        {
            _voxelRootObject = GetComponent<VoxelRootObject>();
            _rigSettings = GetComponent<RealTimeSettingsToRigger>();
        }

        private void FixedUpdate()
        {
            CheckDestruction();
        }

        private void CheckDestruction()
        {
            if (_voxelRootObject.IsDestroyed)
                return;

            foreach (Element leg in _rigSettings.successLegs)
            {
                if (!CheckHierarchyDestruction(leg, destroyAll: true))
                    continue;

                _rigSettings.successLegs.Remove(leg);

                if (!_rigSettings.IsFlyingObject && _rigSettings.successLegs.Count <= 0)
                    _voxelRootObject.CollapseEntirely();
                break;
            }

            foreach (Element arm in _rigSettings.successArms)
            {
                CheckHierarchyDestruction(arm);
            }

            foreach (Element body in _rigSettings.successBody)
            {
                if (!CheckHierarchyDestruction(body))
                    continue;

                _voxelRootObject.CollapseEntirely();
                break;
            }

            foreach (Element head in _rigSettings.successHead)
            {
                CheckHierarchyDestruction(head);
            }
        }

        private bool CheckHierarchyDestruction(Element element, bool destroyAll = false)
        {
            for (int i = 0; i < element.list.Count; i++)
            {
                GameObject part = element.list[i];

                if (part.GetComponentInChildren<ICollapsable>() == null)
                {
                    List<GameObject> objectsToDestroy = destroyAll ? element.list : element.list.Skip(i + 1).ToList();

                    objectsToDestroy
                        .ForEach(e => e.GetComponentsInChildren<ICollapsable>()
                        .ToList().ForEach(c => c.CollapseEntirely()));

                    return true;
                }
            }

            return false;
        }
    }
}
