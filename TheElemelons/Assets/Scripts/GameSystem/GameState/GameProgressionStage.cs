/// <summary>
/// Enum representing the major progression stages of the game.
/// Used to control story flow, scene transitions, and feature availability.
/// </summary>
public enum GameProgressionStage
{
    /// <summary>
    /// Invalid or uninitialized stage (default value).
    /// </summary>
    INVALID,
    /// <summary>
    /// The introduction phase in the castle (tutorial or opening).
    /// </summary>
    CASTLE_INTRODUCTION,
    /// <summary>
    /// Early phase of the castle (pre-Earth dungeon).
    /// </summary>
    CASTLE_EARLY,
    /// <summary>
    /// Early phase of the Earth dungeon.
    /// </summary>
    EARTH_EARLY,
    /// <summary>
    /// Boss fight in the early Earth dungeon.
    /// </summary>
    EARTH_EARLY_BOSS,
    /// <summary>
    /// End phase of the castle (after returning from Earth dungeon).
    /// </summary>
    CASTLE_END,
    /// <summary>
    /// Late phase of the castle (post-Earth, new content unlocked).
    /// </summary>
    CASTLE_LATE,
    /// <summary>
    /// Late phase of the Earth dungeon.
    /// </summary>
    EARTH_LATE,
    /// <summary>
    /// Boss fight in the late Earth dungeon.
    /// </summary>
    EARTH_LATE_BOSS,
}