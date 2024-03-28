using System.Collections.Generic;
using UnityEngine;
using VoxelDestruction;

public class CountPerObject : MonoBehaviour
{
    public List<VoxelFragment> voxelFragments = new List<VoxelFragment>();
    public List<VoxelFragment> toRemoveLocal = new List<VoxelFragment>();
    public bool keyDestroy = false;
    [HideInInspector] public bool oneTime = true;

    private void Update()
    {
        if (100 * toRemoveLocal.Count > 40 * voxelFragments.Count)
        {
            if (oneTime)
            {
                keyDestroy = true;
                oneTime = false;
            }
        }
    }
}