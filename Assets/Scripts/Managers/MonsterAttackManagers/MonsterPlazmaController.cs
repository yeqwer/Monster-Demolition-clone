using UnityEngine;

public class MonsterPlazmaController : MonsterAttackControllerBase
{
    [SerializeField] private PlazmaType plazmaType;
    public int plazmaCountMin;
    public int plazmaCountMax;

    public override void Starter()
    {
        var i = FindObjectOfType<SpawnPlazmaTargets>();
        i.plazmaType = plazmaType;
        i.plazmaMin = plazmaCountMin;
        i.plazmaMax = plazmaCountMax;
        i.SpawnTargets();
    }
}
