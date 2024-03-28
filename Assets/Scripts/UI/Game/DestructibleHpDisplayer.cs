using UnityEngine;
using UnityEngine.UI;
using VoxelTools;

public class DestructibleHpDisplayer : MonoBehaviour
{
    private VoxelRootObject _voxelRootObject;

    private Slider _hpSlider;

    private void Awake()
    {
        _voxelRootObject = GetComponent<VoxelRootObject>();
        _hpSlider = GetComponentInChildren<Slider>();
    }

    private void Update()
    {
        _hpSlider.value = _voxelRootObject.DamageRatio;
    }
}
