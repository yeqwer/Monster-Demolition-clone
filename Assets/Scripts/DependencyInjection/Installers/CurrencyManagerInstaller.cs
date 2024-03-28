using Zenject;

public class CurrencyManagerInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<CurrencyManager>().AsSingle();
    }
}