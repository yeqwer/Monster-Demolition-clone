using KiYandexSDK;
using UnityEngine;
using Zenject;

public class YandexSdkInitializerInstaller : MonoBehaviour
{
    [SerializeField]
    private YandexSDKInitialize _yandexSdkInitializePrefab;

    [Inject]
    private void Construct(DiContainer diContainer)
    {
#if YANDEX_BUILD
        diContainer.InstantiatePrefab(_yandexSdkInitializePrefab);
#else
        diContainer.InstantiateComponent<SceneLoader>(gameObject).LoadScene(Scenes.Main);
#endif
    }
}