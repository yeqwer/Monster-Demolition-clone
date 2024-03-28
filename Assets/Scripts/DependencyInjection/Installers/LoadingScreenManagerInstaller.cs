using UnityEngine;
using Zenject;

public class LoadingScreenManagerInstaller : MonoInstaller
{
    [SerializeField]
    private LoadingScreenManager _loadingScreenPrefab;

    public override void InstallBindings()
    {
        Container.Bind<LoadingScreenManager>().FromComponentInNewPrefab(_loadingScreenPrefab).AsSingle().NonLazy();
    }
}