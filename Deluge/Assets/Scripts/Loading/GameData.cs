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
}
