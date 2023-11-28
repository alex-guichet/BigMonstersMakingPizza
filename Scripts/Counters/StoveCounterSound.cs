using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource _sizzlingSound;

    private void Awake()
    {
        _sizzlingSound = GetComponent<AudioSource>();
    }

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounterOnStateChanged;
    }
    
    private void StoveCounterOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        bool isTurnedOn = e.state is StoveCounter.State.Fried or StoveCounter.State.Frying;

        if (isTurnedOn)
        {
            _sizzlingSound.Play();
        }
        else
        {
            _sizzlingSound.Pause();
        }
    }
}
