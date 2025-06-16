using System.Collections.Generic;

/// <summary>
<<<<<<< Updated upstream
/// Door in the castle hub that can only be opened or closed depending on the current game progression stage.
/// </summary>
public class Door_CastleHub_2 : Door
{
    public List<GameProgressionStage> canOpenCondition;

=======
/// Specialized door for the Castle Hub that can only be opened or closed depending on the current game progression stage.
/// </summary>
public class Door_CastleHub_2 : Door
{
    /// <summary>
    /// List of progression stages during which the door cannot be opened.
    /// </summary>
    public List<GameProgressionStage> canOpenCondition;

    // Start is called before the first frame update.
    // Initializes the door's components and state.
>>>>>>> Stashed changes
    void Start()
    {
        Initialize();
    }

<<<<<<< Updated upstream
=======
    // Update is called once per frame.
    // Updates the door's open/close condition based on the current progression stage.
>>>>>>> Stashed changes
    void Update()
    {
        if (canOpenCondition != null)
        {
            canOpen = !canOpenCondition.Contains(GameManager.Instance.CurrentProgressionStage);
        }
    }
}
