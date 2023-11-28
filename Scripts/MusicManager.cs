using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] musicArray;
    
    public static MusicManager Instance;

    private AudioSource _audioSource;

    private float _volume;
    
    private const string PLAYER_PREFS_VOLUME_MUSIC = "music_volume";
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        
        _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_VOLUME_MUSIC, 1f);
        _audioSource.volume = _volume;
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Loader.OnSceneLoaded += LoaderOnSceneLoaded;
        
        SceneName targetSceneName = Loader.GetSceneTarget();
        
        if (targetSceneName == SceneName.GameScene)
        {
            _audioSource.clip = musicArray[1];
            _audioSource.Play();
            
            GameManager.Instance.OnStateChanged += GameManagerOnStateChanged;
        }
    }

    private void LoaderOnSceneLoaded(object sender, EventArgs e)
    {
        if (Loader.GetSceneTarget() == SceneName.LobbyScene && KitchenGameMultiplier.isMultiplayer)
        {
            _audioSource.clip = musicArray[1];
            _audioSource.Play();
        }
    }

    private void GameManagerOnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsGameCountdown())
        {
            _audioSource.Stop();
        }
        
        if (GameManager.Instance.IsGamePlaying())
        {
            _audioSource.clip = musicArray[0];
            _audioSource.Play();
        }
    }


    public void ChangeVolume()
    {
        _volume += 0.1f;
        
        if (Math.Round(_volume, 2) > 1f)
        {
            _volume = 0f;
        }
        
        _audioSource.volume = _volume;
        PlayerPrefs.SetFloat(PLAYER_PREFS_VOLUME_MUSIC, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return Mathf.Round(_volume * 10f);
    }

    private void OnDestroy()
    {
        Loader.OnSceneLoaded -= LoaderOnSceneLoaded;
        GameManager.Instance.OnStateChanged -= GameManagerOnStateChanged;
    }
}
