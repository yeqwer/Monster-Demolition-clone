using TMPro;
using UnityEngine;
using Zenject;

public class CurrencyDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _currencyText;

    [Inject]
    private void Construct(CurrencyManager currencyManager)
    {
        UpdateCurrencyText(currencyManager.CurrentCurrencyAmount);
        currencyManager.OnCurrencyAmountChanged += UpdateCurrencyText;
    }

    private void UpdateCurrencyText(int currency)
    {
        _currencyText.text = currency.ToString();
    }
}
