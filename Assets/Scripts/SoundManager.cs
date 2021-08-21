using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audioSource;

    public AudioClip WinJingle;
    public AudioClip LoseJingle;

    public AudioClip hit1;
    public AudioClip hit2;
    public AudioClip hit3;

    public AudioClip click1;
    public AudioClip click2;

    public AudioClip rotate;

    public AudioClip footstep1;
    public AudioClip footstep2;
    public AudioClip footstep3;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            Debug.Log("SoundManager already exists!");
        }
        else
        {
            instance = this.GetComponent<SoundManager>();
        }
    }

    public void PlayClick()
    {
        int index = Random.Range(0, 1);
        if (index == 0)
        {
            audioSource.PlayOneShot(click1);
        }
        else
        {
            audioSource.PlayOneShot(click2);
        }
    }

    public void PlayHit()
    {
        int index = Random.Range(0, 2);
        if (index == 0)
        {
            audioSource.PlayOneShot(hit1);
        }
        else if (index == 1)
        {
            audioSource.PlayOneShot(hit2);
        }
        else
        {
            audioSource.PlayOneShot(hit3);
        }
    }

    public void PlayRotate()
    {
        audioSource.PlayOneShot(rotate);
    }

    public void PlayWinJingle()
    {
        audioSource.PlayOneShot(WinJingle);
    }

    public void PlayLoseJingle()
    {
        audioSource.PlayOneShot(LoseJingle);
    }

    public void PlayFootstep()
    {
        int index = Random.Range(0, 2);
        if (index == 0)
        {
            audioSource.PlayOneShot(footstep1);
        }
        else if (index == 1)
        {
            audioSource.PlayOneShot(footstep2);
        }
        else
        {
            audioSource.PlayOneShot(footstep3);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
