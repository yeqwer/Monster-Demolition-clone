using UnityEngine;

public class MonsterBombController : MonsterAttackControllerBase
{
    [SerializeField] private int targetCountMin;
    [SerializeField] private int targetCountMax;

    public override void Starter()
    {
        var i = FindObjectOfType<SpawnBombsTargets>();
        i.targetMin = targetCountMin;
        i.targetMax = targetCountMax;
        i.SpawnTargets();
    }
}
