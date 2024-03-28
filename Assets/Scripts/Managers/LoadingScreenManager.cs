using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _allLoadingScreens;

    private int _currentLoadingScreenIndex = -1;

    private void Awake()
    {
        ShowRandomLoadingScreen();
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        VoxelObjectManager.Instance.OnAllVoxelObjectsBuilt += HideActiveLoadingScreen;
    }

    public void ShowLoadingScreen(int index)
    {
        if (index == _currentLoadingScreenIndex)
            return;

        foreach (var loadingScreen in _allLoadingScreens)
            loadingScreen.SetActive(false);

        _currentLoadingScreenIndex = index;
        _allLoadingScreens[index].SetActive(true);
    }

    public void ShowRandomLoadingScreen()
    {
        int loadingScreenIndex = Random.Range(0, _allLoadingScreens.Length);

        ShowLoadingScreen(loadingScreenIndex);
    }

    public void HideLoadingScreen(int index)
    {
        _currentLoadingScreenIndex = -1;
        _allLoadingScreens[index].SetActive(false);
    }

    public void HideActiveLoadingScreen()
    {
        if (_currentLoadingScreenIndex < 0)
            return;

        HideLoadingScreen(_currentLoadingScreenIndex);
    }

#if UNITY_EDITOR
    [ContextMenu("Show random loading screen")]
    public void ShowRandomLoadingScreenContextMenu()
    {
        ShowRandomLoadingScreen();
    }

    [ContextMenu("Hide active loading screen")]
    public void HideActiveLoadingScreenContextMenu()
    {
        HideActiveLoadingScreen();
    }
#endif
}
