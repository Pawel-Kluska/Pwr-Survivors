using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource musicSource;

    [SerializeField]
    private AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip backgroundGameMusic;
    public AudioClip buttonClick;
    public AudioClip hurtSound;
    public AudioClip levelUpSound;
    public AudioClip deathSound;
    public AudioClip collectSound;

    public void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        PlayBackgroundMusic(backgroundMusic);
    }

    public void PlayBackgroundMusic(AudioClip backgroundMusic)
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip audioClip)
    {
        Debug.Log("Playing SFX 1");

        if (audioClip)
        {
            sfxSource.PlayOneShot(audioClip);
            Debug.Log("Playing SFX");
        }
    }

    public void onValueMusicChanged(float value)
    {
        musicSource.volume = value;
    }

    public void onValueSFXChanged(float value)
    {
        sfxSource.volume = value;
    }
}
