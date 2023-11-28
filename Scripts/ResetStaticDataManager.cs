using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    public void Awake()
    {
        TrashCounter.ResetStaticData();
        CuttingCounter.ResetStaticData();
        PlayerSound.ResetStaticData();
        Player.ResetStaticData();
    }
}
