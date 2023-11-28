using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public enum LookAtMode
{
    LookAt,
    LookAtInverted,
    CameraForward,
    CameraForwardInverted
}

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private LookAtMode lookAtMode;
    
    private void LateUpdate()
    {
        switch (lookAtMode)
        {
            case LookAtMode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case LookAtMode.LookAtInverted:
                Vector3 direction = transform.position - Camera.main.transform.position;
                transform.LookAt(direction);
                break;
            case LookAtMode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case LookAtMode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
}
