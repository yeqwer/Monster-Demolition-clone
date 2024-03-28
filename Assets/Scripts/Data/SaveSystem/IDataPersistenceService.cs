public interface IDataPersistenceService
{
    public void Save(PlayerData playerData);

    public PlayerData Load();

    public void Delete(PlayerData playerData);
}
