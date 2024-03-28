using UnityEngine;
using VoxelTools;
using Zenject;

public class GameManager : GameManagerBase
{
    [SerializeField]
    private TriggerEventSender _roadAreaTrigger;

    [SerializeField]
    private TriggerEventSender _monsterAreaTrigger;

    [Inject]
    private void Construct(CarSpawnManager carSpawnManager, LevelManager levelManager)
    {
        levelManager.OnLevelLoaded += levelIndex => SwitchState(GameState.Start);
        levelManager.OnLevelCompleted += levelIndex => SwitchState(GameState.Victory);
        carSpawnManager.OnCarSpawned += SubscribeForCarEvents;

        _roadAreaTrigger.onTriggerExit += collider =>
        {
            if (collider.tag == "Player")
                SwitchState(GameState.Finish);
        };
        _monsterAreaTrigger.onTriggerExit += collider =>
        {
            if (collider.tag == "Player")
                SwitchState(GameState.End);
        };
    }

    private void SubscribeForCarEvents(CarController carController)
    {
        carController.OnInputPressed += () => SwitchState(GameState.Playing);
        carController.gameObject.GetComponent<IDamageable>().OnDestroyed += () => SwitchState(GameState.Fail);
        carController.gameObject.GetComponent<Demolisher>().OnHitDestructible += (destructible) => SwitchState(GameState.End);
        carController.gameObject.GetComponent<CollisionEventSender>().onCollisionEnter += collision => SwitchState(GameState.End);

        _monsterAreaTrigger.onTriggerEnter += collider =>
        {
            if (collider.tag == "Player")
                collider.gameObject.GetComponent<CollisionEventSender>().ProcessEvents = true;
        };
    }

    public override void SwitchState(GameState state)
    {
        switch (state)
        {
            case GameState.Start when CurrentState is GameState.End or GameState.Fail or GameState.Victory:
                CurrentState = state;
                break;
            case GameState.Playing when CurrentState is GameState.Start:
                CurrentState = state;
                break;
            case GameState.Fail when CurrentState is GameState.Playing:
                CurrentState = state;
                break;
            case GameState.Finish when CurrentState is GameState.Playing:
                CurrentState = state;
                break;
            case GameState.End when CurrentState is GameState.Finish:
                CurrentState = state;
                break;
            case GameState.Victory:
                CurrentState = state;
                break;
        }
    }

#if UNITY_EDITOR
    [ContextMenu("Complete race")]
    private void CompleteRace()
    {
        SwitchState(GameState.Finish);
    }

    [ContextMenu("Complete level")]
    private void CompleteLevel()
    {
        SwitchState(GameState.Victory);
    }
#endif
}
