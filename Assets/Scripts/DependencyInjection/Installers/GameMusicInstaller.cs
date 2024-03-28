using UnityEngine;
using Zenject;

public class GameMusicInstaller : MonoInstaller
{
    [SerializeField]
    private MusicGameCon _gameMusic;

    public override void InstallBindings()
    {
        Container.Bind<MusicGameCon>().FromComponentInNewPrefab(_gameMusic).AsSingle().NonLazy();
    }
}
