using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class AudioClipInfo
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    [Header("AudioSource")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource soundEffectSource;

    [Header("Clips")]
    public List<AudioClipInfo> audioClips;

    //[Header("FX Slider")]
    //[SerializeField] private Slider bgmSlider;
    //[SerializeField] private Slider soundEffectSlider;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;         
        }
        else
        {
            Destroy(gameObject);    
        }
    }
    
    private void Start()
    {
    }

    private void UpdateAudio()
    {
        bgmSource.volume = PlayerPrefs.GetFloat("bgmVolume");
        soundEffectSource.volume = PlayerPrefs.GetFloat("soundEffectVolume");
    }
    
    public void SetBGMVolume() //更改背景音樂聲量
    {
        //float bgmVolume = bgmSlider.value;
        //PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        //PlayerPrefs.Save();
    }
    
    public void SetSoundEffectVolume() //更新音效聲量
    {
        //float seVolume = soundEffectSlider.value;
        //PlayerPrefs.SetFloat("soundEffectVolume", seVolume);
        //PlayerPrefs.Save();
    }

    public void PlayFX(string fxName)
    {
        AudioClip clip = null;

        foreach (var audioClipInfo in audioClips)
        {
            if (audioClipInfo.name.Equals(fxName, StringComparison.OrdinalIgnoreCase))
            {
                clip = audioClipInfo.clip;
                break;
            }
        }
        soundEffectSource.PlayOneShot(clip);
    }
}