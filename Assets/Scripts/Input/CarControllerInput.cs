using UnityEngine;

[RequireComponent(typeof(CarController))]
public class CarControllerInput : MonoBehaviour
{
    private CarController _carController;

    private void Awake()
    {
        _carController = GetComponent<CarController>();

        GameManagerBase gameManager = FindObjectOfType<GameManagerBase>();
        if (gameManager is not null)
        {
            if (gameManager.CurrentState is GameState.Start)
                SetActiveInputs(true);

            gameManager.OnStateChanged += newState =>
            {
                if (newState is GameState.End or GameState.Fail or GameState.Victory)
                    SetActiveInputs(false);
            };
        }
    }

    private void Update()
    {
        _carController.UpdateInputs();
    }

    public void SetActiveInputs(bool isActive)
    {
        _carController.IsInputActive = isActive;
    }
}
