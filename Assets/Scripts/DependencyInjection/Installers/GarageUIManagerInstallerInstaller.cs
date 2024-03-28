using UnityEngine;
using Zenject;

public class GarageUIManagerInstallerInstaller : MonoInstaller
{
    [SerializeField]
    private GarageUIManager _garageUIManager;

    public override void InstallBindings()
    {
        Container.Bind<GarageUIManager>().FromComponentInNewPrefab(_garageUIManager).AsSingle().NonLazy();
    }
}