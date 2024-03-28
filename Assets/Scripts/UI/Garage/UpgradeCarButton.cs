using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class UpgradeCarButton : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _upgradeCostText;

    [Inject]
    private CarProgressionManager _progressionManager;

    private void Start()
    {
        if (_progressionManager.IsMaxProgress)
        {
            OnMaxProgressReached();
        }
        else
        {
            _upgradeCostText.text = _progressionManager.NextUpgradeCost.ToString();
            _progressionManager.OnMaxProgressReached += OnMaxProgressReached;
        }
    }

    public void Upgrade()
    {
        _progressionManager.Upgrade();
        if (!_progressionManager.IsMaxProgress)
            _upgradeCostText.text = _progressionManager.NextUpgradeCost.ToString();
    }

    public void OnMaxProgressReached()
    {
        GetComponent<Button>().interactable = false;
        _upgradeCostText.text = "Max";
    }
}
