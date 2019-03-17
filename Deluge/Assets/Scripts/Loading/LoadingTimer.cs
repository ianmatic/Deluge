using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingTimer : MonoBehaviour
{
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        time = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        time -= Time.deltaTime;

        //transition to game when timer reaches 0
        if (time < 0)
        {
            SceneManager.LoadScene(GameData.TargetScene);
        }
    }
}
