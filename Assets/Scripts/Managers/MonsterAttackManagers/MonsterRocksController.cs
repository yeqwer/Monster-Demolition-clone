using UnityEngine;

public class MonsterRocksController : MonsterAttackControllerBase
{
    [SerializeField] private RockType rockType;
    [SerializeField] private int targetCountMin;
    [SerializeField] private int targetCountMax;

    public override void Starter()
    {
        var i = FindObjectOfType<SpawnRocksTargets>();
        i.rockType = rockType;
        i.targetMin = targetCountMin;
        i.targetMax = targetCountMax;
        i.SpawnTargets();
    }
}
