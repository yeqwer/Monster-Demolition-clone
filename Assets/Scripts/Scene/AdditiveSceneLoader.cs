using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveSceneLoader : MonoBehaviour
{
    [SerializeField]
    private string[] _sceneNames;

    private void Start()
    {
        foreach (string sceneName in _sceneNames)
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}
