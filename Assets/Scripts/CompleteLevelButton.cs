using UnityEngine;
using Zenject;

public class CompleteLevelButton : MonoBehaviour
{
    [Inject]
    private LevelManager _levelManager;

    private void OnGUI()
    {
        GUI.skin.button.fontSize = 70;

        if (GUILayout.Button("Complete level"))
            _levelManager.CompleteLevel();
    }
}
