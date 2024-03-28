using System;
using UnityEngine;
using Zenject;

public class CarSpawnManager : MonoBehaviour
{
    [Inject]
    private DiContainer Container { get; }

    [Inject]
    private DataPersistenceManager _dataPersistenceManager;

    [SerializeField]
    private Vector3 _spawnPosition = Vector3.zero;

    private GameObject _carInstance;

    public event Action<CarController> OnCarSpawned;

    [Inject]
    private void Construct(GameManagerBase gameManager)
    {
        gameManager.OnStateChanged += newState =>
        {
            if (newState is GameState.Start)
                Respawn();
        };
    }

    private void Spawn()
    {
        _carInstance = Container.InstantiatePrefab(_dataPersistenceManager.PlayerData.SelectedCarPrefab, _spawnPosition, Quaternion.identity, null);
        OnCarSpawned?.Invoke(_carInstance.GetComponent<CarController>());
    }

    public void Respawn()
    {
        if (_carInstance is not null)
            Destroy(_carInstance);
        Spawn();
    }
}
