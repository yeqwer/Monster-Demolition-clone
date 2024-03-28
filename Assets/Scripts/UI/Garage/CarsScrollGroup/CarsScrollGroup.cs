using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class CarsScrollGroup : MonoBehaviour
{
    private List<VehicleGarageCardItem> _vehicleGarageCardItems = new List<VehicleGarageCardItem>();

    [SerializeField]
    private Transform _contentHolder;

    [Inject]
    private void Construct(DiContainer container, CarSelectManager carSelectManager)
    {
        GameObject[] allCars = carSelectManager.AllCars.ToArray();
        for (int i = 0; i < allCars.Length; i++)
        {
            CarData carData = allCars[i].GetComponent<CarData>();

            VehicleGarageCardItem vehicleGarageCardItem = VehicleGarageCardItem.InstantiateCard(container, _contentHolder, carData);
            vehicleGarageCardItem.CarIndex = i;
            vehicleGarageCardItem.OnCardSelected += DeselectAllCards;
            _vehicleGarageCardItems.Add(vehicleGarageCardItem);
        }
    }

    private void DeselectAllCards(int carIndex)
    {
        foreach (var card in _vehicleGarageCardItems)
        {
            if (card.CarIndex != carIndex)
                card.DeselectCard();
        }
    }
}
