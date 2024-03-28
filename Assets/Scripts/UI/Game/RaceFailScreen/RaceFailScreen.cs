using UnityEngine;
using Zenject;

public class RaceFailScreen : MonoBehaviour
{
    [Inject]
    private GameManagerBase _gameManager;

    public void RespawnCar()
    {
        _gameManager.SwitchState(GameState.Start);
    }
}
