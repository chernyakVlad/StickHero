using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    [SerializeField]
    private AudioClip bonus;
    [SerializeField]
    private AudioClip btnClick;
    [SerializeField]
    private AudioClip death;
    [SerializeField]
    private AudioClip fallStick;
    [SerializeField]
    private AudioClip kickStick;
    [SerializeField]
    private AudioClip score;
    [SerializeField]
    private AudioClip stickGrow;

    AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();       
    }    


    public void Play(AudioState state)
    {
        if(PlayerPrefs.GetString("Music") != "no")
        {
            switch ((int)state)
            {
                case 0:
                    {
                        audioSource.clip = bonus;
                        audioSource.Play();
                        break;
                    }
                case 1:
                    {
                        audioSource.clip = kickStick;
                        audioSource.Play();
                        break;
                    }
                case 2:
                    {
                        audioSource.loop = true;
                        audioSource.clip = stickGrow;
                        audioSource.Play();
                        break;
                    }
                case 3:
                    {
                        audioSource.clip = fallStick;
                        audioSource.Play();
                        break;
                    }
                case 4:
                    {
                        audioSource.clip = death;
                        audioSource.Play();
                        break;
                    }
                case 5:
                    {
                        audioSource.clip = score;
                        audioSource.Play();
                        break;
                    }
                case 6:
                    {
                        audioSource.Stop();
                        audioSource.loop = false;
                        break;
                    }
                case 7:
                    {
                        audioSource.clip = btnClick;
                        audioSource.Play();
                        break;
                    }
            }
        }
    }
}

public enum AudioState
{
    Bonus,
    KickStick,
    StickGrow,
    FallStick,
    Death,
    Score,
    Stop,
    BtnClick
}
