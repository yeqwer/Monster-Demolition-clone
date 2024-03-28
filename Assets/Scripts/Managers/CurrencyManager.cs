using System;
using Zenject;

public class CurrencyManager
{
    [Inject]
    private DataPersistenceManager _dataPersistenceManager;

    private int _currentCurrencyAmount;

    public int CurrentCurrencyAmount
    {
        get => _currentCurrencyAmount;
        private set
        {
            if (_currentCurrencyAmount == value)
                return;

            _currentCurrencyAmount = value;
            OnCurrencyAmountChanged?.Invoke(value);
            _dataPersistenceManager.PlayerData.CurrentCurrency = value;
        }
    }

    public event Action<int> OnCurrencyAmountChanged;

    public event Action OnNotEnoughCurrency;

    [Inject]
    private void Construct(DataPersistenceManager dataPersistenceManager)
    {
        CurrentCurrencyAmount = dataPersistenceManager.PlayerData.CurrentCurrency;
    }

    public void Add(uint amount)
    {
        CurrentCurrencyAmount += (int)amount;
    }

    public bool Spend(uint amount)
    {
        if (CurrentCurrencyAmount < amount)
        {
            OnNotEnoughCurrency?.Invoke();
            return false;
        }

        CurrentCurrencyAmount -= (int)amount;
        return true;
    }
}

