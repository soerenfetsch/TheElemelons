/// <summary>
/// Represents the different stages of game progression for story, events, and world state.
/// Used to control NPC behavior, level transitions, and UI conditions.
/// </summary>
public enum GameProgressionStage
{
    /// <summary>
    /// Invalid or uninitialized progression stage.
    /// </summary>
    INVALID,

    /// <summary>
    /// The introduction stage in the castle.
    /// </summary>
    CASTLE_INTRODUCTION,

    /// <summary>
    /// Early phase of the castle storyline.
    /// </summary>
    CASTLE_EARLY,

    /// <summary>
    /// Early phase of the earth region.
    /// </summary>
    EARTH_EARLY,

    /// <summary>
    /// Boss fight in the early earth region.
    /// </summary>
    EARTH_EARLY_BOSS,

    /// <summary>
    /// End phase of the castle storyline.
    /// </summary>
    CASTLE_END,

    /// <summary>
    /// Late phase of the castle storyline.
    /// </summary>
    CASTLE_LATE,

    /// <summary>
    /// Late phase of the earth region.
    /// </summary>
    EARTH_LATE,

    /// <summary>
    /// Boss fight in the late earth region.
    /// </summary>
    EARTH_LATE_BOSS,
}