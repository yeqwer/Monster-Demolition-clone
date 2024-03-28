using UnityEngine;
using Zenject;

public class MonsterAttackManager : MonoBehaviour
{
    private IMonsterAttack[] _monsterAttacks;

    [Inject]
    private void Construct(GameManagerBase gameManager)
    {
        gameManager.OnStateChanged += state =>
        {
            if (state is GameState.Playing)
                ApplyRandomAttack();
        };
    }

    private void Awake()
    {
        _monsterAttacks = GetComponents<IMonsterAttack>();
    }

    private void ApplyRandomAttack()
    {
        int attackIndex = Random.Range(0, _monsterAttacks.Length);

        _monsterAttacks[attackIndex].Activate();
    }
}
