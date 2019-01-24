using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseAutio : MonoBehaviour
{
    public AudioClip Catching;
    public AudioClip Release;

    public AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCatching()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(Catching);
        }
    }
    public void PlayRelease()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(Release);
        }
    }
}
