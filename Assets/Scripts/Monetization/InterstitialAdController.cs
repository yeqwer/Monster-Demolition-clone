#if YANDEX_BUILD
using KiYandexSDK;
#endif
using UnityEngine;
using Zenject;

public class InterstitialAdController : MonoBehaviour
{
    [SerializeField]
    private int _showAdsOncePerRaceAmount;

    private int _counter;

    private bool _firstTime = true;

    private CarControllerInput _carControllerInput;

#if YANDEX_BUILD
    [Inject]
    private void Construct(GameManagerBase gameManager, CarSpawnManager carSpawnManager)
    {
        gameManager.OnStateChanged += OnGameStateChanged;
        carSpawnManager.OnCarSpawned += carController => _carControllerInput = carController.GetComponent<CarControllerInput>();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (_firstTime)
        {
            _firstTime = false;
            return;
        }

        if (state is not GameState.Start)
            return;

        _counter = ++_counter % _showAdsOncePerRaceAmount;

        if (_counter != 0)
            return;

        AdvertSDK.InterstitialAd(
            () => _carControllerInput.SetActiveInputs(false),
            value => _carControllerInput.SetActiveInputs(true),
            value => _carControllerInput.SetActiveInputs(true),
            () => _carControllerInput.SetActiveInputs(true));
    }
#endif
}
