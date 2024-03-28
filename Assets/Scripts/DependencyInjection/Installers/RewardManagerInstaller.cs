using Zenject;

public class RewardManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<RewardManager>().FromNewComponentOnNewGameObject().AsSingle();
    }
}