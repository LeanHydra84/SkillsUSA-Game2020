using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UpdateAudio();

public class AudioSourceHandler : MonoBehaviour
{

    public static UpdateAudio AudioDelegate;

    AudioSource player;

    void Start()
    {

        player = GetComponent<AudioSource>();
        AudioDelegate += AudioCheck;
        
    }

    private void OnEnable()
    {
        if(player == null) player = GetComponent<AudioSource>();
        AudioCheck();
    }

    void AudioCheck()
    {
        player.volume = ((float)Settings.Volume)/100;
    }

}
