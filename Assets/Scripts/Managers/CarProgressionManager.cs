using System;
using UnityEngine;
using Zenject;

public class CarProgressionManager : MonoBehaviour
{
    [Inject]
    private CurrencyManager _currencyManager;

    [Inject]
    private DataPersistenceManager _dataPersistenceManager;

    [SerializeField]
    private CarProgressionObject _carProgression;

    private int _currentProgress;

    public int CurrentProgress
    {
        get => _currentProgress;
        private set
        {
            if (_currentProgress == value)
                return;

            _currentProgress = value;
            OnCurrentProgressChanged?.Invoke(value, _carProgression.CarUpgradeTiers[value]);
            _dataPersistenceManager.PlayerData.CurrentCarProgression = value;
        }
    }

    public bool IsMaxProgress => CurrentProgress == _carProgression.CarUpgradeTiers.Length - 1;

    public int NextUpgradeCost => IsMaxProgress ? -1 : _carProgression.CarUpgradeTiers[CurrentProgress + 1].UpgradeCost;

    public event Action<int, CarUpgradeTier> OnCurrentProgressChanged;

    public event Action OnMaxProgressReached;

    private void Start()
    {
        CurrentProgress = _dataPersistenceManager.PlayerData.CurrentCarProgression;
    }

    public void Upgrade()
    {
        if (IsMaxProgress)
            return;

        if (_currencyManager.Spend((uint)_carProgression.CarUpgradeTiers[CurrentProgress].UpgradeCost))
        {
            CurrentProgress++;

            if (IsMaxProgress)
                OnMaxProgressReached?.Invoke();
        }
    }
}
