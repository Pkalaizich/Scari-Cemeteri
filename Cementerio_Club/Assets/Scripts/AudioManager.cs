using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;
    [SerializeField] AudioSource walkSource;

    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;

    [SerializeField] private AudioClip purifyingSFX;
    [SerializeField] private AudioClip purifiedSFX;
    [SerializeField] private AudioClip fireSFX;

    #region Singleton
    static private AudioManager instance;
    static public AudioManager Instance
    {
        get { return instance; }
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameController.Instance.EndGame.AddListener(StopPurification);
    }

    public void StepStatus(bool on)
    {
        if(on)
        {
            walkSource.Play();
        }
        else
        {
            walkSource.Stop();
        }
    }

    public void PlaySFX(int effect)
    {
        if(effect== 0)
        {
            sfxSource.Stop();
            sfxSource.loop = true;
            sfxSource.clip = purifyingSFX;
            sfxSource.Play();
        }
        else
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = purifiedSFX;
            sfxSource.Play();
        }
    }
    public void StopPurification()
    {
        if(sfxSource.clip == purifyingSFX)
        {
            sfxSource.Stop();
        }
    }
    public void ChangeBackgroundMusic()
    {
        musicSource.clip = gameplayMusic;
        musicSource.Play();
    }
    #endregion
}
