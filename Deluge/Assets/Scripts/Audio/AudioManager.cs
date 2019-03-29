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

    private bool transitioning = false;

    void Awake()
    {
        //only allow 1 instance of audiomanager (singleton)
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //more than 1 instance, so destroy the current one
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

            sound.source.loop = sound.loop;
        }


        //play menuTheme on startup
        if (SceneManager.GetActiveScene().name == "menuScene")
        {
            Sound s = Array.Find(sounds, sound => sound.name == "menuTheme");
            s.source.volume = 1.0f;
            PlaySong("menuTheme");
        }

    }

    void Update()
    {
        //song loaded in
        if (currentSong.source != null && !transitioning)
        {
            //pause music
            if (GameData.FullPaused)
            {
                currentSong.source.Pause();
            }
            //play music
            else
            {
                //resume music
                if (!currentSong.source.isPlaying && !GameData.FullPaused)
                {
                    currentSong.source.UnPause();
                }

                //reduce volume of music for dialogue
                if (GameData.GameplayPaused)
                {
                    TurnDownVolume(0.3f);
                }
                //turn back up
                else
                {
                    TurnUpVolume(1.0f);
                }
            }
        }

    }

    /// <summary>
    /// Decreases volume of currentSong to target value
    /// </summary>
    /// <param name="target"></param>
    public void TurnDownVolume(float target)
    {
        //reduce volume
        if (currentSong.source.volume > target)
        {
            currentSong.source.volume -= 0.01f;
        }
    }

    /// <summary>
    /// Increases volume of currentSong to target value
    /// </summary>
    /// <param name="target"></param>
    public void TurnUpVolume(float target)
    {
        //increase volume
        if (currentSong.source.volume < target)
        {
            currentSong.source.volume += 0.01f;
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
        if (s.source.time == 0.0f)
        {
            s.source.Play();
        }
        else
        {
            s.source.UnPause();
        }

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
            song.fading = true;
            return false;
        }
        else
        {
            //done fading
            song.source.Pause();
            song.fading = false;
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
        transitioning = true;

        //songs are different, so transition, or the song isn't at full volume
        if (currentSong.name != name)
        {
            //fading out
            if (currentSong == null || currentSong.source == null || FadeOut(currentSong.name))
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
                    transitioning = false;
                }
            }
        }
        //songs are the same, so just fade back in the song
        else if (currentSong.fading)
        {
            FadeIn(currentSong.name);
        }
        else
        {
            transitioning = false;
        }

    }

    public void Setup()
    {
        //setup each sound
        foreach (Sound sound in sounds)
        {
            if (sound.source == null)
            {
                sound.source = gameObject.AddComponent<AudioSource>();

                sound.source.clip = sound.clip;
                sound.source.volume = sound.volume;

                sound.source.spatialBlend = 1.0f;

                sound.source.loop = sound.loop;
            }
        }
    }
}
