using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [HideInInspector]
    public float volume = 0.0f;
    [HideInInspector]
    public float pitch = 1.0f;
    public bool loop;


    [HideInInspector]
    public AudioSource source;
}
