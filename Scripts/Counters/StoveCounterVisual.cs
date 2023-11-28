using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StoveCounterVisual : MonoBehaviour
{
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject sizzlingParticles;
    [SerializeField] private GameObject stoveOnVisual;

    private bool _isTurnedOn;
    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounterOnStateChanged;
    }

    private void StoveCounterOnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        _isTurnedOn = e.state is StoveCounter.State.Fried or StoveCounter.State.Frying;
        sizzlingParticles.SetActive(_isTurnedOn);
        stoveOnVisual.SetActive(_isTurnedOn);
    }
}
