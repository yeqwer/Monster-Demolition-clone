using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _allLevels;

    [Inject]
    private DataPersistenceManager _dataPersistenceManager;

    //[Inject]
    private LoadingScreenManager _loadingScreenManager;

    private GameObject _currentLevelGO;

    public int CurrentLevelIndex { get; private set; } = -1;

    public int CurrentLevelNumber => CurrentLevelIndex + 1;

    public bool IsLevelCompleted { get; private set; }

    public event Action<int> OnLevelCompleted;

    public event Action<int> OnLevelUnload;

    public event Action<int> OnLevelLoaded;

    [Inject]
    private void Construct(MonsterManager monsterManager)
    {
        monsterManager.OnMonsterDefeat += CompleteLevel;
    }

    private void Start()
    {
        CurrentLevelIndex = _dataPersistenceManager.PlayerData.CurrentLevelIndex;
        LoadLevel(CurrentLevelIndex % _allLevels.Length);
    }

#if UNITY_EDITOR
    [ContextMenu("Refresh levels")]
    private void RefreshLevels()
    {
        int previousLevelCount = _allLevels is null ? 0 : _allLevels.Length;

        _allLevels = Resources.LoadAll<GameObject>("Prefabs/Levels");

        int currentLevelCount = _allLevels is null ? 0 : _allLevels.Length;

        Debug.Log($"Refreshed levels. Added {currentLevelCount - previousLevelCount} new levels.");
    }

    [ContextMenu("Complete level")]
    private void CompleteLevelContextMenu()
    {
        if (!Application.isPlaying)
        {
            Debug.LogWarning("This method works only at runtime.");
            return;
        }

        CompleteLevel();
    }
#endif

    public void CompleteLevel()
    {
        IsLevelCompleted = true;
        OnLevelCompleted?.Invoke(CurrentLevelIndex);
        _dataPersistenceManager.PlayerData.CurrentLevelIndex = CurrentLevelIndex + 1;
    }

    private void LoadLevel(int index)
    {
        CurrentLevelIndex = index;
        _currentLevelGO = Instantiate(_allLevels[index]);
        OnLevelLoaded?.Invoke(CurrentLevelIndex);
        _dataPersistenceManager.PlayerData.CurrentLevelIndex = index;
    }

    public void LoadNextLevel()
    {
        if (_currentLevelGO != null)
        {
            Destroy(_currentLevelGO);
            OnLevelUnload?.Invoke(CurrentLevelIndex);
        }

        _loadingScreenManager?.ShowRandomLoadingScreen();
        SceneManager.LoadScene((int)Scenes.Main);
    }

    private IEnumerator LoadNextLevelCoroutine()
    {
        if (_currentLevelGO is not null)
            Destroy(_currentLevelGO);

        // Waiting untill previous level game object is destroyed.
        yield return new WaitForEndOfFrame();

        CurrentLevelIndex++;
        LoadLevel(CurrentLevelIndex % _allLevels.Length);
    }
}
