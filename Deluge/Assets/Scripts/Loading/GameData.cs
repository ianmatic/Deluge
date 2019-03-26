using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//static class is used to handle data between scenes
public static class GameData
{
    private static string targetScene;

    public static string TargetScene
    {
        get { return targetScene; }
        set { targetScene = value; }
    }

    //pauses most gameplay, used for dialogue
    private static bool gameplayPaused;

    public static bool GameplayPaused
    {
        get { return gameplayPaused; }
        set { gameplayPaused = value; }
    }

    //hard pauses the game
    private static bool fullPaused;

    public static bool FullPaused
    {
        get { return fullPaused; }
        set { fullPaused = value; }
    }


    /// <summary>
    /// Toggles pausing, used to stop all events
    /// </summary>
    public static void ToggleGameplayPaused()
    {
        gameplayPaused = !gameplayPaused;
    }

    /// <summary>
    /// Toggles gameplay pause, used to stop player movement but allow background events
    /// </summary>
    public static void ToggleFullPaused()
    {
        fullPaused = !fullPaused;
    }
}
