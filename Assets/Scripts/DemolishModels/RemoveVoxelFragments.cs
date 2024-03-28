using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VoxelDestruction;
using Zenject;

public class RemoveVoxelFragments : MonoBehaviour
{
    private List<VoxelFragment> voxelFragments = new List<VoxelFragment>();

    [Inject]
    private void Construct(GameManagerBase gameManager, LevelManager levelManager)
    {
        gameManager.OnStateChanged += state =>
        {
            if (state is GameState.Start)
                ResetAttackTargets();
        };
        levelManager.OnLevelLoaded += index => RemoveOnRestartLevel();
    }

    [ContextMenu("NewLevel")]
    public void RemoveOnRestartLevel()
    {
        voxelFragments.AddRange(FindObjectsOfType<VoxelFragment>(true));

        foreach (VoxelFragment fragment in voxelFragments) { Destroy(fragment.gameObject); }
        voxelFragments.Clear();

        ResetAttackTargets();
    }

    [ContextMenu("ResetAttackTargets")]
    public static void ResetAttackTargets()
    {
        GameObject[] bombs = GameObject.FindGameObjectsWithTag("TargetBombs");
        GameObject[] plazma = GameObject.FindGameObjectsWithTag("TargetPlazma");
        GameObject[] rocks = GameObject.FindGameObjectsWithTag("TargetRocks");

        bombs.ToList().ForEach(i => Destroy(i));
        plazma.ToList().ForEach(i => Destroy(i));
        rocks.ToList().ForEach(i => Destroy(i));
    }
}