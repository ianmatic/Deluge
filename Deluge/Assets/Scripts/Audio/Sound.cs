using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    //set in inspector
    public string name;
    public bool loop;
    public AudioClip clip;


    [HideInInspector]
    public float volume = 0.0f;
    [HideInInspector]
    public float pitch = 1.0f;
    [HideInInspector]
    public AudioSource source;


    [HideInInspector]
    public bool fading = false;
}
