using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Windows;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource BackgroundSource;
    public AudioSource MenuSource;
    public AudioSource BoostSource;
    public AudioSource CoinSource;
    public AudioSource RiverSource;
    public AudioSource CollisionStoneSource;
    //public AudioSource CollisionLogSource;
    //public AudioSource AmbientEnviromentSource;
    //public AudioSource CountdownSource;
    public AudioSource CompletedLevelSource;
    public AudioSource RowingSource;

    //in game location
    public AudioSource BirdsSource;
    public AudioSource WaterfallSource;
    
    public float InGameVolume, MenuVolume, BoostVolume, CoinsVolumme, RiverVolume, CollisionVolume, OarVolumme;
    
    [Space(25)]
    public AudioClip BackgroundMusic;
    public AudioClip MenuAudio;
    public AudioClip BoostSoundAudio;
    public AudioClip CoinsCollectedAudio;
    
    //special
    public AudioClip RiverAudio; 
    public AudioClip CollisionStonAudio; 
    //public AudioClip ColisionLogAudio;
    
    //public AudioClip AmbientEnviromentAudio; 
    
    //special
    //public AudioClip CountdownAudio; 
    
    public AudioClip CompletedLevelAudio;    
    public AudioClip RowingAudio1, RowingAudio2, RowingAudio3, RowingAudio4, RowingAudio5, RowingAudio6 ;
    
    //same place as background
    //public AudioClip BirdsAudio;
    //public AudioClip WaterfallAudio;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    bool play = false;
    public void Update()
    {
        /*
        if (Time.time > 3 && play == false )
        {
            
            StopBackgroundAudio2(1);
            //PlayCoinstAudio();
            play = true;
            Debug.Log("playerfwre");
        }
        

        if (UnityEngine.Input.GetKeyDown(KeyCode.L))
        {
            PlayMenuAudio();
        }
        */
    }
    

    void Start()
    {
        UIManager.Instance.OnGameStart += StopMenuAudio;

        UIManager.Instance.OnGameStart += PlayBackgroundAudio;

        UIManager.Instance.OnGameStart += PlayRiverAudio;
    }

    //Play audio methods (first a general that can be used for everynone and then all specifics)//
    public void PlayAudio(AudioSource source, AudioClip clip, float volume)
    {
        if (source.isPlaying)
        {
            source.Stop();
        }
        source.clip = clip;
        source.volume = volume;
        source.Play();
        
        Debug.Log("Custom Sound plays");
    }
    public void PlayBackgroundAudio()
    {
        if (!BackgroundMusic)
        {
            Debug.Log("No Soundclip");
            return;
        }

        BackgroundSource.volume = InGameVolume;
        BackgroundSource.clip = BackgroundMusic;
        BackgroundSource.loop = true;
        BackgroundSource.Play();
        
        //Debug.Log("background");
        //Debug.Log("ingame volume is: " + InGameVolume);
    }

    
    public void PlayBackgroundAudio2()
    {
        if (!BackgroundMusic)
        {
            Debug.Log("No Soundclip");
            return;
        }
        BackgroundSource.volume = InGameVolume;
        BackgroundSource.clip = BackgroundMusic;
        BackgroundSource.loop = true;
        BackgroundSource.Play();
    }
    
    public void PlayMenuAudio()
    {
        if (!MenuAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }
        MenuSource.volume = MenuVolume;
        MenuSource.clip = MenuAudio;
        MenuSource.loop = true;
        MenuSource.Play();
        Debug.Log(MenuSource.clip);
    }
    
    public void PlayMenuAudio2()
    {
        if (!MenuAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }

        MenuSource.volume = MenuVolume;
        MenuSource.clip = MenuAudio;
        MenuSource.loop = true;
        MenuSource.Play();
    }
    public void StopBackgroundAudio()
    {
        BackgroundSource.Stop();
    }
    public void StopBackgroundAudio2()
    {
        Debug.Log("ongemaended?");
        BackgroundSource.Stop();
    }
    public void StopMenuAudio()
    {
        MenuSource.Stop();
    } 
    public void StopMenuAudio2()
    {
        MenuSource.Stop();
    }
    public void StopRiverAudio()
    {
        RiverSource.Stop();
    }
    public void StopRiverAudio2()
    {
        RiverSource.Stop();
    }
    public void PlayBoostAudio()
    {
        if (!BoostSoundAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }

        BoostSource.volume = BoostVolume;
        BoostSource.clip = BoostSoundAudio;
        BoostSource.Play();
    }

    public void PlayCoinAudio()
    {
        if (!CoinsCollectedAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }

        CoinSource.volume = CoinsVolumme;
        CoinSource.clip = CoinsCollectedAudio;
        CoinSource.Play();
    }
    public void PlayRiverAudio()
    {
        if (!RiverAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }

        RiverSource.volume = RiverVolume;
        RiverSource.clip = RiverAudio;
        RiverSource.loop = true;
        RiverSource.Play();
    }
    public void PlayCollisionStoneAudio()
    {
        if (!CollisionStonAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }

        CollisionStoneSource.volume = CollisionVolume;
        CollisionStoneSource.clip = CollisionStonAudio;
        CollisionStoneSource.Play();
    }
    /*
    public void PlayCollisionLogAudio()
    {
        if (!ColisionLogAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }
        CollisionLogSource.clip = ColisionLogAudio;
        CollisionLogSource.Play();
    }
    */
    /*
    public void PlayAmbientEnviromentAudio()
    {
        if (!AmbientEnviromentAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }
        AmbientEnviromentSource.clip = AmbientEnviromentAudio;
        AmbientEnviromentSource.loop = true;
        AmbientEnviromentSource.Play();
    }
    */
    /*
    public void PlayCountdownAudio()
    {
        if (!CountdownAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }
        CountdownSource.clip = CountdownAudio;
        CountdownSource.Play();
    }
    */
    
    public void PlayCompletedLevelAudio()
    {
        if (!CompletedLevelAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }
        CompletedLevelSource.clip = CompletedLevelAudio;
        CompletedLevelSource.Play();
    }
    
    public void PlayRowingAudio()
    {
        
        if (!RowingAudio1)
        {
            Debug.Log("No Soundclip");
            return;
        }
        RowingSource.volume = OarVolumme;
        float random;
        random = UnityEngine.Random.Range(0.0f, 12.0f);
        if (random < 2 )
        {
            RowingSource.clip = RowingAudio1;
            RowingSource.Play();
        }
        else if(random < 4 && random > 2 )
        {
            RowingSource.clip = RowingAudio2;
            RowingSource.Play();
        }
        else if (random < 6 && random > 4 )
        {
            RowingSource.clip = RowingAudio3;
            RowingSource.Play();
        }
        else if (random < 8 && random > 6)
        {
            RowingSource.clip = RowingAudio4;
            RowingSource.Play();
        }
        else if (random < 10 && random > 8)
        {
            RowingSource.clip = RowingAudio5;
            RowingSource.Play();
        }
        else if (random < 12 && random > 10)
        {
            RowingSource.clip = RowingAudio6;
            RowingSource.Play();
        }
    }
    /*
    public void PlayBirdsAudio()
    {
        if (!BirdsAudio)
        {
            Debug.Log("No Soundclip");
            return;
        }

        BirdsSource.clip = BirdsAudio;
        BirdsSource.Play();
    }
    */
    /*
    public void PlayWaterfallAudio()
    {
        WaterfallSource.clip = WaterfallAudio;
        WaterfallSource.loop = true;
        WaterfallSource.Play();
    }
    */
    //Other settings
    public void SetVloume(AudioSource source, AudioClip clip, float volume)
    {
        source.clip = clip;
        source.volume = volume;
        source.Play();
    }


}

/*
    //Probably not used sice all buttons are triggered differently
    public void PlayButtonPressedAudio()
    {
        effectsSource.clip = ButtonPressedClip;
        effectsSource.Play();
    }

    //Stop audio methods//

    public void StopBackgroundAudio()
    {
        BackgroundSource.Stop();
    }
    public void StopMenuAudio()
    {
        BackgroundSource.Stop();
    }
    public void StopWaterAudio()
    {
        MenuSource.Stop();
    }
    public void StopObstacleAudio()
    {
        MenuSource.Stop();
    }
    public void StopButtonPressedAudio()
    {
        MenuSource.Stop();
    }
    */
