using UnityEngine;
using Zenject;

public class CarProgressionManagerInstaller : MonoInstaller
{
    [SerializeField]
    private CarProgressionManager _progressionManager;

    public override void InstallBindings()
    {
        Container.Bind<CarProgressionManager>().FromComponentInNewPrefab(_progressionManager).AsSingle();
    }
}