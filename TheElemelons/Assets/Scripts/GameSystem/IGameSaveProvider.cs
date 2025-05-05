public interface IGameSaveProvider
{
    void SaveGame(GameSaveData data);
    GameSaveData LoadGame();
    void ResetSave();
}