using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WarningUI : MonoBehaviour
{
    [FormerlySerializedAs("stoveCounter")] [SerializeField] private OvenCounter ovenCounter;
    [SerializeField] private float timeToShowWarning = 0.5f;

    private float _playSoundTime = 0.2f;
    private float _playSoundTimer;
    private bool _playSound;
    
    private void Start()
    {
        ovenCounter.OnProgressChanged += OvenCounterOnProgressChanged;
        Hide();
    }

    private void OvenCounterOnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        if (e.progressNormalized > timeToShowWarning && ovenCounter.GetCurrentState() == OvenCounter.State.Fried)
        {
            Show();
            _playSound = true;
        }
        else
        {
            Hide();
            _playSound = false;
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!_playSound)
            return;
        
        _playSoundTimer -= Time.deltaTime;
        if (_playSoundTimer < 0f)
        {
            _playSoundTimer = _playSoundTime;
            SoundManager.Instance.PlayWarningSound(transform.position);
        }
        
    }
}
