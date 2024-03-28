using UnityEngine;
using Zenject;

public class UiSoundInstaller : MonoInstaller
{
    [SerializeField]
    private UiSoundCon _uiSound;

    public override void InstallBindings()
    {
        Container.Bind<UiSoundCon>().FromComponentInNewPrefab(_uiSound).AsSingle().NonLazy();
    }
}