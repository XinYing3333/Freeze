using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AudioClipInfo
{
    public string name;
    public AudioClip clip;
}

public class ButtonFX : MonoBehaviour
{
    [Header("AudioSource")]
    public AudioSource myFx;

    [Header("Clips")]
    public List<AudioClipInfo> audioClips;

    [Header("FX Slider")]
    public Slider fxSlider;

    private void Awake()
    {
       // myFx.volume = PlayerPrefs.GetFloat("FXVolume");
       // fxSlider.value = PlayerPrefs.GetFloat("FXVolume");
    }

    private void Start()
    {
        GameObject sound = GameObject.Find("myFX");
        myFx = sound.GetComponent<AudioSource>();
    }

    private void Update()
    {
       // float fxVolume = myFx.volume;
       // PlayerPrefs.SetFloat("FXVolume", fxVolume);
       // PlayerPrefs.Save();
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
        myFx.PlayOneShot(clip);
    }
}