using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OvenCounterSound : MonoBehaviour
{
    [SerializeField] private OvenCounter ovenCounter;
    private AudioSource _ovenSound;

    private void Awake()
    {
        _ovenSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        ovenCounter.OnStateChanged += OvenCounterOnStateChanged;
    }
    
    private void OvenCounterOnStateChanged(object sender, OvenCounter.OnStateChangedEventArgs e)
    {
        bool isTurnedOn = e.state is OvenCounter.State.Fried or OvenCounter.State.Frying;

        if (isTurnedOn)
        {
            _ovenSound.Play();
        }
        else
        {
            _ovenSound.Pause();
        }
    }
}
