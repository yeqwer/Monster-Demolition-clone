using UnityEngine;
using Zenject;

public class CarInstaller : MonoInstaller
{
    [SerializeField]
    private CarController _carController;

    public override void InstallBindings()
    {
        Container.Bind<CarController>().FromComponentInNewPrefab(_carController).AsSingle();
    }
}