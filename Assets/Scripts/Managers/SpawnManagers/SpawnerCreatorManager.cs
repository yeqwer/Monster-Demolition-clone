using UnityEngine;
using Zenject;

public class SpawnerCreatorManager : MonoBehaviour
{
    [SerializeField] private GameObject plazmaTargetsSpawner;
    [SerializeField] private GameObject bombsTargetsSpawner;
    [SerializeField] private GameObject rocksTargetsSpawner;

    [SerializeField] private Vector3 positionPlazma;
    [SerializeField] private Vector3 positionBombs;
    [SerializeField] private Vector3 positionRocks;

    [Inject]
    private DiContainer _diContainer;

    void Awake()
    {
        SpawnTartgets();  
    }

    private void SpawnTartgets()
    {
        Setter(plazmaTargetsSpawner.transform, positionPlazma);
        Setter(bombsTargetsSpawner.transform, positionBombs);
        Setter(rocksTargetsSpawner.transform, positionRocks);
    }

    private GameObject Setter(Transform obj, Vector3 pos)
    {
        return _diContainer.InstantiatePrefab(obj, pos, Quaternion.identity, transform);
    }
}