using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //specify in Unity
    public string targetScene;
    public bool needLoadScreen;

    public void LoadScene()
    {

        if (needLoadScreen)
        {
            //load the load screen and pass through the target scene
            SceneManager.LoadScene("LoadScene");
            GameData.TargetScene = targetScene;
        }
        else
        {
            //load the scene
            SceneManager.LoadScene(targetScene);
        }
    }
}
