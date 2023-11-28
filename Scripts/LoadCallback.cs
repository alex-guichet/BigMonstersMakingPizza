using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCallback : MonoBehaviour
{
    private bool _onFirstUpdate = true;
    
    private void Update()
    {
        if (_onFirstUpdate)
        {
            _onFirstUpdate = false;
            Loader.LoadCallback();
        }
    }
}
