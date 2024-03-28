using UnityEngine;
using Zenject;

public class LevelManagerInstaller : MonoInstaller
{
    [SerializeField]
    private LevelManager _levelManager;

    public override void InstallBindings()
    {
        Container.Bind<LevelManager>().FromComponentInNewPrefab(_levelManager).AsSingle();
    }
}