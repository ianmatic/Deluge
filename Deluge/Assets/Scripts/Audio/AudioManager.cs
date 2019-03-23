using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        //only allow 1 instance of audiomanager (singleton)
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //allow transition between scenes
        DontDestroyOnLoad(gameObject);

        //setup each sound
        foreach (Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
        }
    }


    /// <summary>
    /// Plays a sound given a string
    /// </summary>
    /// <param name="name"></param>
    public void PlaySound(string name)
    {
        //find the sound
        Sound s = Array.Find(sounds, sound => sound.name == name);

        //play it
        s.source.Play();
    }
}
