using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

    [HideInInspector]
    public Sound currentSong;

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

            sound.source.spatialBlend = 1.0f;
        }


        //play menuTheme on startup
        if (SceneManager.GetActiveScene().name == "menuScene")
        {
            PlaySound("menuTheme");
        }

    }


    /// <summary>
    /// Plays a sound given a string, PASS IN SFX
    /// </summary>
    /// <param name="name"></param>
    public void PlaySound(string name)
    {
        //find the sound
        Sound s = Array.Find(sounds, sound => sound.name == name);

        //for now hardcode volume
        s.source.volume = 1.0f;

        //play it
        s.source.Play();
    }

    /// <summary>
    /// Plays a sound given a string, PASS IN SONG
    /// </summary>
    /// <param name="name"></param>
    public void PlaySong(string name)
    {
        //find the sound
        Sound s = Array.Find(sounds, sound => sound.name == name);

        //set as current sound
        currentSong = s;

        //play it
        s.source.Play();
    }

    /// <summary>
    /// Returns true if a song is muted, false otherwise and reduces volume
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool FadeOut(string name)
    {
        //find the sound
        Sound song = Array.Find(sounds, sound => sound.name == name);

        //no song
        if (song == null)
        {
            return true;
        }

        //reduce volume
        if (song.source.volume > 0.0f)
        {
            //still have to keep fading
            song.source.volume -= 0.01f;
            return false;
        }
        else
        {
            //done fading
            song.source.Stop();
            return true;
        }
    }

    /// <summary>
    /// Returns true if a song is full volume, false otherwise and increases volume
    /// Used for songs
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool FadeIn(string name)
    {
        //find the song
        Sound song = Array.Find(sounds, sound => sound.name == name);

        //start playing the song
        if (song.source.volume == 0)
        {
            PlaySong(song.name);
        }

        //increase volume
        if (song.source.volume < 1.0f)
        {
            //still have to keep fading
            song.source.volume += 0.01f;
            return false;
        }
        else
        {
            //done fading
            return true;
        }
    }

    /// <summary>
    /// Uses fade in and fade out to transition from the currentSong to the inputed one
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public void TransitionToSong(string name)
    {
        //songs are different, so transition
        if (currentSong.name != name)
        {
            //fading out
            if (currentSong == null || FadeOut(currentSong.name))
            {
                //done fading out

                //find the song
                Sound song = Array.Find(sounds, sound => sound.name == name);

                //start playing the song
                if (song.source.volume == 0)
                {
                    //use sound as we don't want to set currentSong yet
                    PlaySound(song.name);
                }

                //increase volume
                if (song.source.volume < 1.0f)
                {
                    //still have to keep fading
                    song.source.volume += 0.01f;
                }
                else
                {
                    //done fading
                    currentSong = song;
                }
            }
        }

    }
}
