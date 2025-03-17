using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    // 0: 인트로, 1: 메인 ,2: 바둑 둘 때, 3: 게임 오버
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] audioClips;

    [Header("Slider")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    void Awake()
    {
        float saveBgmVolume = PlayerPrefs.GetFloat(Constants.BGMVolumeKey, 1f);
        float saveSfxVolume = PlayerPrefs.GetFloat(Constants.SFXVolumeKey, 1f);

        if (bgmSource != null)
            bgmSource.volume = saveBgmVolume;
        if (sfxSource != null)
            sfxSource.volume = saveSfxVolume;

        if (bgmSlider != null)
        {
            bgmSlider.value = saveBgmVolume;
            bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = saveSfxVolume;
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }
    
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    /// <summary>
    /// 인트로 BGM 재생
    /// </summary>
    public void PlayIntroBGM()
    {
        if (bgmSource != null && audioClips.Length > 0 && audioClips[0] != null)
        {
            bgmSource.clip = audioClips[0];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 메인 BGM 재생 
    /// </summary>
    public void PlayMainBGM()
    {
        if (bgmSource != null && audioClips.Length > 1 && audioClips[1] != null)
        {
            bgmSource.clip = audioClips[1];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    /// <summary>
    /// 효과음 재생 
    /// </summary>
    public void PlaySFXSound(int index)
    {
        if (sfxSource != null && index >= 2 && index < audioClips.Length && audioClips[index] != null)
        {
            sfxSource.PlayOneShot(audioClips[index]);
        }
    }

    /// <summary>
    /// 슬라이더로 BGM 볼륨 조절 
    /// </summary>
    /// <param name="value">슬라이더 값</param>
    public void OnBGMVolumeChanged(float value)
    {
        if (bgmSource != null)
        {
            bgmSource.volume = value;
            PlayerPrefs.SetFloat(Constants.BGMVolumeKey, value);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 슬라이더로 SFX 볼륨 조절 
    /// </summary>
    /// <param name="value">슬라이더 값</param>
    public void OnSFXVolumeChanged(float value)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = value;
            PlayerPrefs.SetFloat(Constants.SFXVolumeKey, value);
            PlayerPrefs.Save();
        }
    }
}
