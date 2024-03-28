using UnityEngine;
using Zenject;

public class RaceFinishScreen : MonoBehaviour
{
    [Inject]
    private GameManagerBase _gameManager;

    public void RespawnCar()
    {
        _gameManager.SwitchState(GameState.Start);
    }
}
