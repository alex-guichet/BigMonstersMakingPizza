using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    private Transform _targetTransform;
    

    public void SetTargetTransform(Transform targetTransform)
    {
        _targetTransform = targetTransform;
    }

    private void LateUpdate()
    {
        if (_targetTransform == null)
            return;

        var followTransform = transform;
        followTransform.position = _targetTransform.position;
        followTransform.rotation = _targetTransform.rotation;
    }
}
