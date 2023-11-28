using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class OvenCounterVisual : MonoBehaviour
{
    [SerializeField] private OvenCounter ovenCounter;
    [SerializeField] private GameObject smokeParticles;

    private bool _isTurnedOn;
    private void Start()
    {
        ovenCounter.OnStateChanged += OvenCounterOnStateChanged;
    }

    private void OvenCounterOnStateChanged(object sender, OvenCounter.OnStateChangedEventArgs e)
    {
        _isTurnedOn = e.state is OvenCounter.State.Fried or OvenCounter.State.Frying;
        smokeParticles.SetActive(_isTurnedOn);
    }
}
