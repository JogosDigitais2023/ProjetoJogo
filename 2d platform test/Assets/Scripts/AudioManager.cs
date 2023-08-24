using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayMusic("Theme");
    }

    public void PlayMusic(string nome)
    {
        Sound s = Array.Find(musicSounds, x => x.nome == nome);

        if (s == null)
        {
            Debug.Log("Som não encontrado");
        }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }

    public void PlaySFX(string nome)
    {
        Sound s = Array.Find(sfxSounds, x => x.nome == nome);

        if (s == null)
        {
            Debug.Log("Som não encontrado");
        }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.PlayOneShot(s.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}
