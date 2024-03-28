using UnityEngine;

public class LocalSaver : IDataPersistenceService
{
    public void Delete(PlayerData playerData)
    {
        PlayerPrefs.DeleteAll();

        playerData = PlayerData.Default;
    }

    public PlayerData Load()
    {
        PlayerData playerData = new PlayerData();

        PlayerData defaultData = PlayerData.Default;

        playerData.CurrentCurrency = PlayerPrefs.GetInt(nameof(playerData.CurrentCurrency), defaultData.CurrentCurrency);
        playerData.SelectedCarIndex = PlayerPrefs.GetInt(nameof(playerData.SelectedCarIndex), defaultData.SelectedCarIndex);
        playerData.CurrentLevelIndex = PlayerPrefs.GetInt(nameof(playerData.CurrentLevelIndex), defaultData.CurrentLevelIndex);
        playerData.CurrentMonsterIndex = PlayerPrefs.GetInt(nameof(playerData.CurrentMonsterIndex), defaultData.CurrentMonsterIndex);
        playerData.CurrentCarProgression = PlayerPrefs.GetInt(nameof(playerData.CurrentCarProgression), defaultData.CurrentCarProgression);

        int equipmentInventoryItemsCount = PlayerPrefs.GetInt(nameof(playerData.EquipmentInventory.ItemsCount), defaultData.EquipmentInventory.ItemsCount);
        string equipmentInventoryKey = nameof(playerData.EquipmentInventory);
        for (int i = 0; i < equipmentInventoryItemsCount; i++)
        {
            playerData.EquipmentInventory.AddItemToEnd((EquipmentType)PlayerPrefs.GetInt(equipmentInventoryKey + $"_{i}"));
        }

        return playerData;
    }

    public void Save(PlayerData playerData)
    {
        PlayerPrefs.SetInt(nameof(playerData.CurrentCurrency), playerData.CurrentCurrency);
        PlayerPrefs.SetInt(nameof(playerData.SelectedCarIndex), playerData.SelectedCarIndex);
        PlayerPrefs.SetInt(nameof(playerData.CurrentLevelIndex), playerData.CurrentLevelIndex);
        PlayerPrefs.SetInt(nameof(playerData.CurrentMonsterIndex), playerData.CurrentMonsterIndex);
        PlayerPrefs.SetInt(nameof(playerData.CurrentCarProgression), playerData.CurrentCarProgression);

        PlayerPrefs.SetInt(nameof(playerData.EquipmentInventory.ItemsCount), playerData.EquipmentInventory.ItemsCount);
        string equipmentInventoryKey = nameof(playerData.EquipmentInventory);
        for (int i = 0; playerData.EquipmentInventory.ItemsCount > 0; i++)
        {
            playerData.EquipmentInventory.TakeFirstItem(out EquipmentType equipmentType);
            PlayerPrefs.SetInt(equipmentInventoryKey + $"_{i}", (int)equipmentType);
        }

        PlayerPrefs.Save();
    }
}
