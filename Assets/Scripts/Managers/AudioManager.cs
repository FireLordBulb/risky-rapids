using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public AudioClip gameplayMusic;
    public AudioClip menuMusic;
    public AudioClip riverAudio; 
    public AudioClip boostAudio;
    public AudioClip coinAudio;
    public AudioClip collisionAudio; 
    public AudioClip levelCompletedAudio;    
    public AudioClip[] rowingAudios;
    public float rowingVolume;
    
    private AudioSource gameplaySource;
    private AudioSource menuSource;
    private AudioSource riverSource;
    private AudioSource boostSource;
    private AudioSource coinSource;
    private AudioSource collisionSource;
    private AudioSource levelCompletedSource;
    private AudioSource rowingSource;
    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject);
            return;
        } 
        Instance = this;

        gameplaySource = gameObject.AddComponent<AudioSource>();
        menuSource = gameObject.AddComponent<AudioSource>();
        riverSource = gameObject.AddComponent<AudioSource>();
        boostSource = gameObject.AddComponent<AudioSource>();
        coinSource = gameObject.AddComponent<AudioSource>();
        collisionSource = gameObject.AddComponent<AudioSource>();
        levelCompletedSource = gameObject.AddComponent<AudioSource>();
        rowingSource = gameObject.AddComponent<AudioSource>();

        gameplaySource.loop = true;
        menuSource.loop = true;
        riverSource.loop = true;
        rowingSource.volume = rowingVolume;
    }
    public void PlayGameplayMusic()
    {
        menuSource.Stop();
        PlayAudio(gameplaySource, gameplayMusic);
    }
    public void PlayMenuMusic()
    {
        gameplaySource.Stop();
        PlayAudio(menuSource, menuMusic);
    }
    public void PlayRiverAudio()
    {
        PlayAudio(riverSource, riverAudio);
    }
    public void StopRiverAudio()
    {
        riverSource.Stop();
    }
    public void PlayBoostAudio()
    {
        PlayAudio(boostSource, boostAudio);
    }
    public void PlayCoinAudio()
    {
        PlayAudio(coinSource, coinAudio);
    }
    public void PlayCollisionStoneAudio()
    {
        PlayAudio(collisionSource, collisionAudio);
    }
    public void PlayCompletedLevelAudio()
    {
        PlayAudio(levelCompletedSource, levelCompletedAudio);
    }
    public void PlayRowingAudio()
    {
        PlayAudio(rowingSource, rowingAudios[Random.Range(0, rowingAudios.Length)]);
    }
    public void PlayAudio(AudioSource source, AudioClip clip)
    {
        if (!clip)
        {
            Debug.Log($"Missing AudioClip for {source}");
            return;
        }
        if (source.isPlaying && source.clip != null)
        {
            return;
        }
        source.clip = clip;
        source.Play();
    }
}