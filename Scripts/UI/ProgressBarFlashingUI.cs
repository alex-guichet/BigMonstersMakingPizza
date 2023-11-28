using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ProgressBarFlashingUI : MonoBehaviour
{
    [FormerlySerializedAs("stoveCounter")] [SerializeField] private OvenCounter ovenCounter;
    [SerializeField] private float timeToShowWarning = 0.5f;

    private Animator _flashingBarAnimator;
    private static readonly int IsFlashing = Animator.StringToHash("IsFlashing");

    private void Awake()
    {
        _flashingBarAnimator = GetComponent<Animator>();
        _flashingBarAnimator.SetBool(IsFlashing, false);
    }
    
    
    private void Start()
    {
        ovenCounter.OnProgressChanged += OvenCounterOnProgressChanged;
    }

    private void OvenCounterOnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
    {
        if (e.progressNormalized > timeToShowWarning && ovenCounter.GetCurrentState() == OvenCounter.State.Fried)
        {
            _flashingBarAnimator.SetBool(IsFlashing, true);
        }
        else
        {
            _flashingBarAnimator.SetBool(IsFlashing, false);
        }
    }
}
