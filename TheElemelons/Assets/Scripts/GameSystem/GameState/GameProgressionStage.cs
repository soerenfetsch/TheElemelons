/// <summary>
<<<<<<< Updated upstream
/// Represents the different stages of game progression for story, events, and world state.
/// Used to control NPC behavior, level transitions, and UI conditions.
=======
/// Enum representing the major progression stages of the game.
/// Used to control story flow, scene transitions, and feature availability.
>>>>>>> Stashed changes
/// </summary>
public enum GameProgressionStage
{
    /// <summary>
<<<<<<< Updated upstream
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
=======
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
>>>>>>> Stashed changes
    /// </summary>
    EARTH_LATE_BOSS,
}