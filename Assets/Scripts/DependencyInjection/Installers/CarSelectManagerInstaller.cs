using UnityEngine;
using Zenject;

public class CarSelectManagerInstaller : MonoInstaller
{
    [SerializeField]
    private CarSelectManager _carSelectManager;

    public override void InstallBindings()
    {
        Container.Bind<CarSelectManager>().FromComponentInNewPrefab(_carSelectManager).AsSingle().NonLazy();
    }
}