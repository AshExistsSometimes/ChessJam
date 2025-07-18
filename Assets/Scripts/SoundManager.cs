using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SFX Clips")]
    public AudioClip pieceDropSFX;
    public AudioClip menuOpenSFX;
    public AudioClip menuCloseSFX;

    [Header("Audio Settings")]
    public float volume = 1f;
    public float pitchVariation = 0.2f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayPieceDropSound()
    {
        audioSource.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
        audioSource.PlayOneShot(pieceDropSFX, volume);
    }

    public void PlayMenuOpenSound()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(menuOpenSFX, volume);
    }

    public void PlayMenuCloseSound()
    {
        audioSource.pitch = 1f;
        audioSource.PlayOneShot(menuCloseSFX, volume);
    }
}
