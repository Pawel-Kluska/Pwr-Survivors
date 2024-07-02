using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    AudioManager audioManager;
    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    public void PlayButtonClick()
    {
        if (audioManager != null && audioManager.buttonClick != null)
            audioManager.PlaySFX(audioManager.buttonClick);
    }

    public void PlayMenuMusic()
    {
        if (audioManager != null && audioManager.backgroundMusic != null)
            audioManager.PlayBackgroundMusic(audioManager.backgroundMusic);
    }

    public void PlayGameMusic()
    {
        if (audioManager != null && audioManager.backgroundGameMusic != null)
            audioManager.PlayBackgroundMusic(audioManager.backgroundGameMusic);
    }

    public void PlayPowerUpSound()
    {
        if (audioManager != null && audioManager.levelUpSound != null)
            audioManager.PlaySFX(audioManager.levelUpSound);
    }
}
