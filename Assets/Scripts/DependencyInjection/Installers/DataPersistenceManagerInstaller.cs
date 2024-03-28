using Zenject;

public class DataPersistenceManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<DataPersistenceManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}