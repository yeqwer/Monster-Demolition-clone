using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Button))]
public class VehicleGarageCardItem : MonoBehaviour
{
    public enum State
    {
        Locked,
        NotSelected,
        Selected
    }

    [SerializeField]
    private State _currentState;

    public State CurrentState
    {
        get
        {
            return _currentState;
        }
        private set
        {
            if (value == _currentState)
                return;

            _currentState = value;
            OnStateChanged(value);
        }
    }

    private CarData _carData;

    private Button _button;

    [Inject]
    private CarProgressionManager _progressionManager;

    [SerializeField]
    private TextMeshProUGUI _carNameText;

    [SerializeField]
    private Image _carSpriteImage;

    [Header("Card selection settings")]
    [SerializeField]
    private GameObject _cardSelectedMark;

    [SerializeField]
    private GameObject _cardDeselectedMark;

    [SerializeField]
    private GameObject _cardLockedMark;

    public int CarIndex = -1;

    public event Action<int> OnCardSelected;

    public static VehicleGarageCardItem InstantiateCard(DiContainer container, Transform parent, CarData carData)
    {
        GameObject vehicleGarageCardItemPrefab = Resources.Load<GameObject>("Prefabs/UI/Garage/VehicleGarageCardItem");

        //GameObject vehicleGarageCardItemInstance = Instantiate(vehicleGarageCardItemPrefab, parent);
        GameObject vehicleGarageCardItemInstance = container.InstantiatePrefab(vehicleGarageCardItemPrefab, parent);

        VehicleGarageCardItem vehicleGarageCardItem = vehicleGarageCardItemInstance.GetComponent<VehicleGarageCardItem>();

        vehicleGarageCardItemInstance.transform.name = $"{carData.CarObject.name}GarageCardItem";
        vehicleGarageCardItem._carNameText.text = carData.CarObject.Name;
        vehicleGarageCardItem._carSpriteImage.sprite = carData.CarObject.Icon;
        vehicleGarageCardItem._carData = carData;

        vehicleGarageCardItem.OnInitialized();
        return vehicleGarageCardItem;
    }

    [Inject]
    private void Construct(CarSelectManager carSelectManager)
    {
        carSelectManager.OnCarSelectedIndex += index =>
        {
            if (index == CarIndex)
                SelectCard();
        };
        OnCardSelected += carSelectManager.SelectCar;
    }

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => SelectCard());
        _button.onClick.AddListener(() => PlaySound());
    }

    private void OnInitialized()
    {
        if (_progressionManager.CurrentProgress >= _carData.CarObject.UnlockAtProgressionLevel)
        {
            UnlockCard();
        }
        else
        {
            CurrentState = State.Locked;
            _progressionManager.OnCurrentProgressChanged += (progress, upgradeTier) =>
            {
                if (progress == _carData.CarObject.UnlockAtProgressionLevel)
                    UnlockCard();
            };
        }
    }

    public void SelectCard()
    {
        if (CurrentState is not State.Locked)
            SwitchState(State.Selected);
    }

    public void DeselectCard()
    {
        if (CurrentState is not State.Locked)
            SwitchState(State.NotSelected);
    }

    public void UnlockCard()
    {
        SwitchState(State.NotSelected);
    }

    private void SwitchState(State state)
    {
        CurrentState = state;
    }
    private void PlaySound()
    {
        GetComponentInParent<GarageUI>().GetComponentInChildren<UiSoundCon>().FirstSound(); //changed by yeqwer
    }

    private void OnStateChanged(State state)
    {
        switch (state)
        {
            case State.Locked:
                _cardLockedMark.SetActive(true);
                _cardDeselectedMark.SetActive(false);
                _cardSelectedMark.SetActive(false);
                break;
            case State.NotSelected:
                _cardLockedMark.SetActive(false);
                _cardDeselectedMark.SetActive(true);
                _cardSelectedMark.SetActive(false);
                break;
            case State.Selected:
                _cardLockedMark.SetActive(false);
                _cardDeselectedMark.SetActive(false);
                _cardSelectedMark.SetActive(true);

                OnCardSelected?.Invoke(CarIndex);
                break;
        }
    }
}
