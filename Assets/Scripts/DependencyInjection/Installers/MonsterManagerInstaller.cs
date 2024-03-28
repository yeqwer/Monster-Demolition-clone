using UnityEngine;
using Zenject;

public class MonsterManagerInstaller : MonoInstaller
{
    [SerializeField]
    private MonsterManager _monsterManager;

    public override void InstallBindings()
    {
        Container.Bind<MonsterManager>().FromComponentInNewPrefab(_monsterManager).AsSingle().NonLazy();
    }
}