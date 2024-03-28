using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CarSelectManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _allCars;

    public IReadOnlyList<GameObject> AllCars => _allCars;

    [Inject]
    private DataPersistenceManager _dataPersistenceManager;

    public event Action<GameObject> OnCarSelected;

    public event Action<int> OnCarSelectedIndex;

    private void Awake()
    {
        _allCars = LoadCars();
    }

    private void Start()
    {
        SelectCar(_dataPersistenceManager.PlayerData.SelectedCarIndex);
    }

    public void SelectCar(int index)
    {
        _dataPersistenceManager.PlayerData.SelectedCarIndex = index;
        OnCarSelected?.Invoke(_allCars[index]);
        OnCarSelectedIndex?.Invoke(index);
    }

    private static GameObject[] LoadCars()
    {
        return Resources.LoadAll<GameObject>("Prefabs/Cars");
    }

    public static GameObject[] GetCars()
    {
        return LoadCars();
    }
}
