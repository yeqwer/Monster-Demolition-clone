using UnityEngine;

public class GarageItemsMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _equipmentActiveButton;

    [SerializeField]
    private GameObject _equipmentInactiveButton;

    [SerializeField]
    private GameObject _vehiclesActiveButton;

    [SerializeField]
    private GameObject _vehiclesInactiveButton;

    [SerializeField]
    private GameObject[] _equipmentsTransforms;

    [SerializeField]
    private GameObject[] _vehiclesTransforms;

    public void ShowEquipment()
    {
        SetActiveEquipmentButton(true);
        SetActiveVehiclesButton(false);
        SetActiveEquipment(true);
        SetActiveVehicles(false);
    }

    public void ShowVehicles()
    {
        SetActiveEquipmentButton(false);
        SetActiveVehiclesButton(true);
        SetActiveEquipment(false);
        SetActiveVehicles(true);
    }

    public void SetActiveEquipment(bool isActive)
    {
        foreach (var equipmentTransform in _equipmentsTransforms)
            equipmentTransform.SetActive(isActive);
    }

    public void SetActiveVehicles(bool isActive)
    {
        foreach (var vehicleTransform in _vehiclesTransforms)
            vehicleTransform.SetActive(isActive);
    }

    public void SetActiveEquipmentButton(bool isActive)
    {
        _equipmentActiveButton.SetActive(isActive);
        _equipmentInactiveButton.SetActive(!isActive);
    }

    public void SetActiveVehiclesButton(bool isActive)
    {
        _vehiclesActiveButton.SetActive(isActive);
        _vehiclesInactiveButton.SetActive(!isActive);
    }
}
