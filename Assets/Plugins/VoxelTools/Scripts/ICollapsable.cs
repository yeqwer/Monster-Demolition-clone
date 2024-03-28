namespace VoxelTools
{
    public interface ICollapsable
    {
        float RemainingFragmentsRatio { get; }

        void CollapseEntirely();
    }
}
