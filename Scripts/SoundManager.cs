using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClipListSO audioClipListSo;
    [SerializeField] private AudioListener audioListener;

    private float _volumeMultiplier = 1f;

    public static SoundManager Instance;

    private const string PLAYER_PREFS_VOLUME_SOUND_EFFECT = "sound_effect_volume";
    

    private void Awake()
    {
        Instance = this;
        _volumeMultiplier = PlayerPrefs.GetFloat(PLAYER_PREFS_VOLUME_SOUND_EFFECT, 1f);
    }
    
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeAdded += DeliveryManagerOnRecipeAdded;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManagerOnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManagerOnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounterOnAnyCut;
        TrashCounter.OnTrash += TrashCounterOnTrash;
        Player.OnAnyPickedSomething += PlayerOnPickUp;
        Player.OnAnyDroppedSomething += PlayerOnDrop;
        PlayerSound.OnWalk += PlayerSoundOnWalk;
    }

    private void DeliveryManagerOnRecipeAdded(object sender, EventArgs e)
    {
        PlaySound(audioClipListSo.orderBell, audioListener.transform.position);
    }

    private void PlayerSoundOnWalk(object sender, EventArgs e)
    {
        PlayerSound playerSound = sender as PlayerSound;
        PlaySound(audioClipListSo.footSteps, playerSound.transform.position);
    }

    private void PlayerOnDrop(object sender, EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(audioClipListSo.objectDrops, player.transform.position);
    }
    
    private void PlayerOnPickUp(object sender, EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(audioClipListSo.objectPickUps, player.transform.position);
    }

    private void TrashCounterOnTrash(object sender, EventArgs e)
    {
        var trashCounter = sender as TrashCounter;
        PlaySound(audioClipListSo.trash, trashCounter.transform.position);
    }

    private void CuttingCounterOnAnyCut(object sender, EventArgs e)
    {
        var cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipListSo.chopSounds, cuttingCounter.transform.position);
    }

    private void DeliveryManagerOnRecipeCompleted(object sender, EventArgs e)
    {
        PlaySound(audioClipListSo.deliverySuccesses, DeliveryCounter.instance.transform.position);
    }
    
    private void DeliveryManagerOnRecipeFailed(object sender, EventArgs e)
    {
        PlaySound(audioClipListSo.deliveryFails, DeliveryCounter.instance.transform.position);
    }

    public void PlayWarningSound(Vector3 position = default)
    {
        PlaySound(audioClipListSo.warnings, position);
    }
    
    public void PlayBellSound(Vector3 position = default)
    {
        PlaySound(audioClipListSo.bell, audioListener.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * _volumeMultiplier);
    }
    
    private void PlaySound(AudioClip[] audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip[Random.Range(0,audioClip.Length)], position, volume * _volumeMultiplier);
    }

    public void ChangeVolume()
    {
        _volumeMultiplier += 0.1f;
        if (Math.Round(_volumeMultiplier, 2) > 1f)
        {
            _volumeMultiplier = 0f;
        }
        PlayerPrefs.SetFloat(PLAYER_PREFS_VOLUME_SOUND_EFFECT, _volumeMultiplier);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return Mathf.Round(_volumeMultiplier * 10f);
    }
}
