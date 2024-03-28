using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    private Scenes _sceneToLoad;

    //[Inject]
    private LoadingScreenManager _loadingScreenManager;

    public void LoadScene()
    {
        _loadingScreenManager?.ShowRandomLoadingScreen();
        SceneManager.LoadScene((int)_sceneToLoad);
    }

    public void LoadScene(Scenes scene)
    {
        _loadingScreenManager?.ShowRandomLoadingScreen();
        SceneManager.LoadScene((int)scene);
    }
}
