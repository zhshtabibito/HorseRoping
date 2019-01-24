﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioClip Throw;
    public AudioClip Dizz;

    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayThrow()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(Throw);
        }
    }
    public void PlayDize()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(Dize);
        }
    }
}
