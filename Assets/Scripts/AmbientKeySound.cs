using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientKeySound : MonoBehaviour
{
    public int KeyNumber;
    private int BossNumber;
    private static Transform player;
    AudioSource ambient;

    void Start()
    {
        player = GameObject.Find("Player").transform;
        ambient = GetComponent<AudioSource>();
        BossNumber = PlayerState.Bosses[KeyNumber - 1];
        ambient.clip = (AudioClip)Resources.Load("Audio/Ambient/" + BossHandler.BossNames[BossNumber - 1]);
        ambient.Play();
        
    }

    void Update()
    {
        ambient.volume = Mathf.Clamp(1 - (Vector3.Distance(player.position, transform.position) / 6.5f), 0, 1); 
    }
}
