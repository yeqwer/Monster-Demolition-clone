using UnityEngine;
using Zenject;

public class GameUIManagerInstaller : MonoInstaller
{
    [SerializeField]
    private GameUIManager _gameUIManager;

    public override void InstallBindings()
    {
        Container.Bind<GameUIManager>().FromComponentInNewPrefab(_gameUIManager).AsSingle().NonLazy();
    }
}