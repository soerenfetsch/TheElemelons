using UnityEngine;

/// <summary>
/// Stores persistent game data including dungeon progress, player stats, and the complete map layout.
/// This ScriptableObject is used both as a template for new games and as a container for the runtime game state.
/// </summary>
[CreateAssetMenu(fileName = "NewGameSaveData", menuName = "Game System/Game Save Data")]
public class GameSaveData : ScriptableObject
{
    // Core game data
    [SerializeField] private PlayerData playerData = new PlayerData();
    [SerializeField] private BuffsData buffsData = new BuffsData();

    // Persistent Fields
    public SerializableMapData mapData;
    public int mapSeed;
    public int currentDungeon;
    public bool[] dialogsCompleted;
    public bool[] bossFightsCompleted;
    public int collectedElemelons;
    public int maxHealth;
    public int maxMana;
    public Vector2 playerPosition;
    public string currentScene;

    // Optionally store default settings to reset later.
    [Header("Defaults (Optional)")]
    [SerializeField] private GameSaveData defaultValues;

    /// <summary>
    /// Resets the current instance to default values.
    /// If defaultValues is assigned, uses that; otherwise, resets manually.
    /// </summary>
    public void ResetToDefaults()
    {
        if (defaultValues != null)
        {
            CopyDataFrom(defaultValues);
        }
        else
        {
            // Provide a manual reset if defaults are not set.
            mapData = null;
            mapSeed = 0;
            currentDungeon = 0;
            dialogsCompleted = null;
            bossFightsCompleted = null;
            collectedElemelons = 0;
            maxHealth = 3;
            maxMana = 3;
            playerPosition = Vector2.zero;
            currentScene = "";
        }
    }

    /// <summary>
    /// Copies data from another GameSaveData instance.
    /// This method can be used for cloning or resetting.
    /// Implements deep copy for the arrays.
    /// </summary>
    public void CopyDataFrom(GameSaveData source)
    {
        if (source == null)
        {
            Debug.LogError("Source data is null. Cannot copy data.");
            return;
        }

        // Shallow copy simple types.
        mapData = source.mapData;  // Consider a deep copy if SerializableMapData becomes mutable.
        mapSeed = source.mapSeed;
        currentDungeon = source.currentDungeon;
        collectedElemelons = source.collectedElemelons;
        maxHealth = source.maxHealth;
        maxMana = source.maxMana;
        playerPosition = source.playerPosition;
        currentScene = source.currentScene;
        
        // Deep copy the arrays if they are not null.
        dialogsCompleted = source.dialogsCompleted != null ? (bool[])source.dialogsCompleted.Clone() : null;
        bossFightsCompleted = source.bossFightsCompleted != null ? (bool[])source.bossFightsCompleted.Clone() : null;
    }

    /// <summary>
    /// Updates current game data with new runtime values.
    /// This method can be invoked by the GameManager to keep the save data updated.
    /// </summary>
    public void UpdateFromRuntimeData(int dungeon, /* int elemelons, */ Vector2 position, /* string scene */)
    {
        // TODO:
        currentDungeon = dungeon;
        // collectedElemelons = elemelons;
        playerPosition = position;
        // currentScene = scene;
        // Add additional fields or validations if required.
    }

    /// <summary>
    /// Creates and returns a deep copy of this GameSaveData instance.
    /// </summary>
    public GameSaveData Clone()
    {
        GameSaveData clone = Instantiate(this);
        // Optionally, you can modify the clone here to decouple it from asset persistence.
        return clone;
    }
}
