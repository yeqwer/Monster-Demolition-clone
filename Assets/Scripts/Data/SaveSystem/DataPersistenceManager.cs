using UnityEngine;

public class DataPersistenceManager : MonoBehaviour
{
    private IDataPersistenceService _dataPersistenceService =
#if YANDEX_BUILD
        new CloudSaver();
#else
        new LocalSaver();
#endif


    private PlayerData _playerData;

    public PlayerData PlayerData
    {
        get
        {
            if (_playerData is null)
                Load();

            return _playerData;
        }
    }

    private void Awake()
    {
        if (_playerData is null)
            Load();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
            Save();

        if (Input.GetKeyDown(KeyCode.F9))
            Load();

        if (Input.GetKeyDown(KeyCode.Delete))
            Delete();
    }
#endif

    public void Save()
    {
        _dataPersistenceService.Save(_playerData);

#if UNITY_EDITOR
        Debug.Log("PlayerData saved.");
#endif
    }

    public void Load()
    {
        _playerData = _dataPersistenceService.Load();

#if UNITY_EDITOR
        Debug.Log("PlayerData loaded.");
#endif
    }

    public void Delete()
    {
        _dataPersistenceService.Delete(_playerData);

#if UNITY_EDITOR
        Debug.Log("Deleted PlayerData.");
#endif
    }
}
