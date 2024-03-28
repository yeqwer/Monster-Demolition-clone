using System.Collections;
using UnityEngine;
using Zenject;

public abstract class MonsterAttackControllerBase : MonoBehaviour, IMonsterAttack
{
    [SerializeField]
    protected float _triggerDistance = Mathf.Infinity;

    private IEnumerator _coroutine;

    protected Transform _carTransform { get; private set; }

    protected Transform _monsterTransform { get; private set; }

    [Inject]
    protected virtual void Construct(CarSpawnManager carSpawnManager, MonsterManager monsterManager, GameManagerBase gameManager)
    {
        carSpawnManager.OnCarSpawned += carController => _carTransform = carController.transform;
        monsterManager.OnMonsterLoad += monster => _monsterTransform = monster.transform;
        gameManager.OnStateChanged += state =>
        {
            if (state is GameState.Start && _coroutine is not null)
                StopCoroutine(_coroutine);
        };
    }

    public void Activate()
    {
        if (_coroutine is not null)
            StopCoroutine(_coroutine);
        _coroutine = TriggerOnDistance(_triggerDistance);
        StartCoroutine(_coroutine);
    }

    private IEnumerator TriggerOnDistance(float distance)
    {
        while (_carTransform != null && _monsterTransform != null &&
            Vector3.Distance(_carTransform.position, _monsterTransform.position) > distance)
            yield return null;

        Starter();
    }

    public abstract void Starter();
}
