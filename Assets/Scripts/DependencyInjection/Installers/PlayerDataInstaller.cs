using Zenject;

public class PlayerDataInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerData>().AsSingle();
    }
}