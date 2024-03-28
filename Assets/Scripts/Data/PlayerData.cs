using UnityEngine;

public class PlayerData
{
    public static PlayerData Default
    {
        get
        {
            PlayerData playerData = new PlayerData();

            playerData.CurrentCurrency = 0;
            playerData.SelectedCarIndex = 0;
            playerData.CurrentLevelIndex = 0;
            playerData.CurrentMonsterIndex = 0;
            playerData.CurrentCarProgression = 0;
            playerData.EquipmentInventory = new EquipmentInventory();

            return playerData;
        }
    }

    public int CurrentCurrency;

    public int SelectedCarIndex;

    public int CurrentLevelIndex;

    public int CurrentMonsterIndex;

    public int CurrentCarProgression;

    public EquipmentInventory EquipmentInventory = new EquipmentInventory();

    public GameObject SelectedCarPrefab
    {
        get => CarSelectManager.GetCars()[SelectedCarIndex];
    }
}
