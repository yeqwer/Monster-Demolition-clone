using UnityEngine;
using Zenject;

public class GameUIManager : ManagerBase<GameUIManager.State>
{
    public enum State
    {
        StartScreen,
        RacingScreen,
        RaceFailScreen,
        RaceFinishScreen,
        LevelCompleteScreen
    }

    [Inject]
    private void Construct(GameManagerBase gameManager)
    {
        gameManager.OnStateChanged += CheckChangeState;
    }

    [SerializeField]
    private GameObject _startScreen;

    [SerializeField]
    private GameObject _failScreen;

    [SerializeField]
    private GameObject _finishScreen;

    [SerializeField]
    private GameObject _completeScreen;

    private void DisableAllScreens()
    {
        _startScreen.SetActive(false);
        _failScreen.SetActive(false);
        _finishScreen.SetActive(false);
        _completeScreen.SetActive(false);
    }

    protected override void OnStateChangedCallback(State newState)
    {
        switch (newState)
        {
            case State.StartScreen:
                DisableAllScreens();
                _startScreen.SetActive(true);
                break;
            case State.RacingScreen:
                DisableAllScreens();
                _startScreen.SetActive(false);
                break;
            case State.RaceFailScreen:
                DisableAllScreens();
                _failScreen.SetActive(true);
                break;
            case State.RaceFinishScreen:
                DisableAllScreens();
                _finishScreen.SetActive(true);
                break;
            case State.LevelCompleteScreen:
                DisableAllScreens();
                Invoke("ShowCompleteLevelScreen", 3);
                break;
        }
    }

    private void CheckChangeState(GameState state)
    {
        switch (state)
        {
            case GameState.Start:
                SwitchState(State.StartScreen);
                break;
            case GameState.Playing:
                SwitchState(State.RacingScreen);
                break;
            case GameState.Fail:
                SwitchState(State.RaceFailScreen);
                break;
            case GameState.End:
                SwitchState(State.RaceFinishScreen);
                break;
            case GameState.Victory:
                SwitchState(State.LevelCompleteScreen);
                break;
        }
    }

    private void ShowCompleteLevelScreen()
    {
        _completeScreen.SetActive(true);
    }
}