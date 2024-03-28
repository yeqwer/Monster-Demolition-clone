using KiYandexSDK;

public class CloudSaver : IDataPersistenceService
{
    public void Delete(PlayerData playerData)
    {
        playerData = PlayerData.Default;
        Save(playerData);
    }

    public PlayerData Load()
    {
        PlayerData playerData = new PlayerData();

        PlayerData defaultData = PlayerData.Default;

        playerData.CurrentCurrency = (int)YandexData.Load(nameof(playerData.CurrentCurrency), defaultData.CurrentCurrency);
        playerData.SelectedCarIndex = (int)YandexData.Load(nameof(playerData.SelectedCarIndex), defaultData.SelectedCarIndex);
        playerData.CurrentLevelIndex = (int)YandexData.Load(nameof(playerData.CurrentLevelIndex), defaultData.CurrentLevelIndex);
        playerData.CurrentMonsterIndex = (int)YandexData.Load(nameof(playerData.CurrentMonsterIndex), defaultData.CurrentMonsterIndex);
        playerData.CurrentCarProgression = (int)YandexData.Load(nameof(playerData.CurrentCarProgression), defaultData.CurrentCarProgression);

        int equipmentInventoryItemsCount = (int)YandexData.Load(nameof(playerData.EquipmentInventory.ItemsCount), defaultData.EquipmentInventory.ItemsCount);
        string equipmentInventoryKey = nameof(playerData.EquipmentInventory);
        for (int i = 0; i < equipmentInventoryItemsCount; i++)
        {
            playerData.EquipmentInventory.AddItemToEnd((EquipmentType)(int)YandexData.Load(equipmentInventoryKey + $"_{i}", null));
        }

        return playerData;
    }

    public void Save(PlayerData playerData)
    {
        YandexData.Save(nameof(playerData.CurrentCurrency), playerData.CurrentCurrency);
        YandexData.Save(nameof(playerData.SelectedCarIndex), playerData.SelectedCarIndex);
        YandexData.Save(nameof(playerData.CurrentLevelIndex), playerData.CurrentLevelIndex);
        YandexData.Save(nameof(playerData.CurrentMonsterIndex), playerData.CurrentMonsterIndex);
        YandexData.Save(nameof(playerData.CurrentCarProgression), playerData.CurrentCarProgression);

        YandexData.Save(nameof(playerData.EquipmentInventory.ItemsCount), playerData.EquipmentInventory.ItemsCount);
        string equipmentInventoryKey = nameof(playerData.EquipmentInventory);
        for (int i = 0; playerData.EquipmentInventory.ItemsCount > 0; i++)
        {
            playerData.EquipmentInventory.TakeFirstItem(out EquipmentType equipmentType);
            YandexData.Save(equipmentInventoryKey + $"_{i}", (int)equipmentType);
        }
    }
}
