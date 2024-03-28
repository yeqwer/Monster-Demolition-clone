using UnityEngine;
using Zenject;

public class GameManagerInstaller : MonoInstaller
{
    [SerializeField]
    private GameManagerBase _gameManager;

    public override void InstallBindings()
    {
        Container.Bind<GameManagerBase>().To<GameManager>().FromComponentInNewPrefab(_gameManager).AsSingle().NonLazy();
    }
}