using UnityEngine;
using Zenject;

public class CarSpawnManagerInstaller : MonoInstaller
{
    [SerializeField]
    private CarSpawnManager _carSpawnManager;

    public override void InstallBindings()
    {
        Container.Bind<CarSpawnManager>().FromComponentInNewPrefab(_carSpawnManager).AsSingle();
    }
}