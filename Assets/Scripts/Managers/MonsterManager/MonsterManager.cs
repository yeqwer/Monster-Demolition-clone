using System;
using System.Collections;
using UnityEngine;
using VoxelTools;
using Zenject;

public class MonsterManager : MonoBehaviour
{
    private DataPersistenceManager _dataPersistenceManager;

    [SerializeField]
    private GameObject[] _allMonsters;

    private GameObject _currentMonsterGO;

    public int CurrentMonsterIndex { get; private set; } = -1;

    public int CurrentMonsterNumber => CurrentMonsterIndex + 1;

    public event Action OnMonsterDefeat;

    public event Action<GameObject> OnMonsterLoad;

    [Inject]
    private void Construct(LevelManager levelManager, DataPersistenceManager dataPersistenceManager)
    {
        levelManager.OnLevelUnload += levelIndex =>
        {
            if (_currentMonsterGO != null)
                Destroy(_currentMonsterGO);
        };
        levelManager.OnLevelLoaded += levelIndex => LoadNextMonster();
        levelManager.OnLevelCompleted += index => _dataPersistenceManager.PlayerData.CurrentMonsterIndex = CurrentMonsterIndex + 1;
        _dataPersistenceManager = dataPersistenceManager;
        // Taking away 1 to force LoadNextMonster method load last saved monster
        // because LoadNextMonster increments CurrentMonsterIndex.
        CurrentMonsterIndex = dataPersistenceManager.PlayerData.CurrentMonsterIndex - 1;
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh monsters")]
    private void RefreshMonsters()
    {
        int previousMonstersCount = _allMonsters is null ? 0 : _allMonsters.Length;

        _allMonsters = Resources.LoadAll<GameObject>("Prefabs/Monsters");

        int currentMonstersCount = _allMonsters is null ? 0 : _allMonsters.Length;

        Debug.Log($"Refreshed monsters. Added {currentMonstersCount - previousMonstersCount} new monsters.");
    }

    [ContextMenu("Defeat monster")]
    private void DefeatMonster()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("This method works only at runtime.");
            return;
        }

        _currentMonsterGO.GetComponent<VoxelRootObject>().CollapseEntirely();
    }
#endif

    private void LoadMonster(int index)
    {
        GameObject monsterStart = GameObject.FindGameObjectWithTag("MonsterStart");

        if (monsterStart == null)
        {
            Debug.LogError("Failed to find monster start. There is no GameObject with tag \"MonsterStart\" on scene.");
            return;
        }

        CurrentMonsterIndex = index;
        _currentMonsterGO = Instantiate(_allMonsters[index], monsterStart.transform);
        _currentMonsterGO.GetComponent<VoxelRootObject>().OnDestroyed += OnMonsterDefeat;
        OnMonsterLoad?.Invoke(_currentMonsterGO);
        _dataPersistenceManager.PlayerData.CurrentMonsterIndex = index;
    }

    private void LoadNextMonster()
    {
        LoadMonster(++CurrentMonsterIndex % _allMonsters.Length);
    }

    private IEnumerator LoadNextMonsterCoroutine()
    {
        if (_currentMonsterGO is not null)
            Destroy(_currentMonsterGO);

        // Waiting until previous monster and level are destroyed.
        yield return new WaitForEndOfFrame();

        CurrentMonsterIndex++;
        LoadMonster(CurrentMonsterIndex % _allMonsters.Length);
    }
}
