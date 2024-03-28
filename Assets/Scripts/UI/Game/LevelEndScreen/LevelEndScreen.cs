using UnityEngine;
using Zenject;

public class LevelEndScreen : MonoBehaviour
{
    [Inject]
    private LevelManager _levelManager;

    public void LoadNextLevel()
    {
        _levelManager.LoadNextLevel();
    }
}
