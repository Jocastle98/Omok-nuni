using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clips")]
    // 0: 인트로, 1: 메인, 2: 바둑 둘 때, 3: 게임 오버
    [SerializeField] private AudioClip[] _audioClips;

    [Header("Sliders")]
    [SerializeField] private Slider _bgmSlider;

    [SerializeField] private Slider _sfxSlider;

    private void Awake()
    {
        float bgmVolume = PlayerPrefs.GetFloat(Constants.BGMVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(Constants.SFXVolumeKey, 1f);

        if (_bgmSource != null)
        {
            _bgmSource.volume = bgmVolume;
        }

        if (_sfxSource != null)
        {
            _sfxSource.volume = sfxVolume;
        }

        if (_bgmSlider != null)
        {
            _bgmSlider.value = bgmVolume;
            _bgmSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        }

        if (_sfxSlider != null)
        {
            _sfxSlider.value = sfxVolume;
            _sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    /// <summary>
    /// 인트로 BGM 재생 (audioClips[0])
    /// </summary>
    public void PlayIntroBgm()
    {
        if (_bgmSource != null && _audioClips.Length > 0 && _audioClips[0] != null)
        {
            _bgmSource.clip = _audioClips[0];
            _bgmSource.loop = true;
            _bgmSource.Play();
        }
    }

    /// <summary>
    /// 메인 BGM 재생 (audioClips[1])
    /// </summary>
    public void PlayMainBgm()
    {
        if (_bgmSource != null && _audioClips.Length > 1 && _audioClips[1] != null)
        {
            _bgmSource.clip = _audioClips[1];
            _bgmSource.loop = true;
            _bgmSource.Play();
        }
    }

    /// <summary>
    /// 효과음 재생 (audioClips[2] 이상)
    /// </summary>
    /// <param name="index">오디오 클립 인덱스</param>
    public void PlaySfxSound(int index)
    {
        if (_sfxSource != null && index >= 2 && index < _audioClips.Length && _audioClips[index] != null)
        {
            _sfxSource.PlayOneShot(_audioClips[index]);
        }
    }

    /// <summary>
    /// 슬라이더로 BGM 볼륨 조절
    /// </summary>
    /// <param name="volume">슬라이더 값</param>
    private void OnBgmVolumeChanged(float volume)
    {
        if (_bgmSource != null)
        {
            _bgmSource.volume = volume;
            PlayerPrefs.SetFloat(Constants.BGMVolumeKey, volume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// 슬라이더로 SFX 볼륨 조절
    /// </summary>
    /// <param name="volume">슬라이더 값</param>
    private void OnSfxVolumeChanged(float volume)
    {
        if (_sfxSource != null)
        {
            _sfxSource.volume = volume;
            PlayerPrefs.SetFloat(Constants.SFXVolumeKey, volume);
            PlayerPrefs.Save();
        }
    }
}
