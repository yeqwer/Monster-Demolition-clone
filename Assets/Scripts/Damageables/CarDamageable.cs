using VoxelTools;

public class CarDamageable : Damageable
{
    private VoxelFragmentedObject _voxelFragmentedObject;

    private void Start()
    {
        _voxelFragmentedObject = GetComponentInChildren<VoxelFragmentedObject>();

        _voxelFragmentedObject.OnRemainingFragmentsRatioUpdate += DestroyAtRatio;
    }

    private void DestroyAtRatio(float currentRatio)
    {
        if (currentRatio <= _destroyAtHealthRatio)
            DestroySelf();
    }
}