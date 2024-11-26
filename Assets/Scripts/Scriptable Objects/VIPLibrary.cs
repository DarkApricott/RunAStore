using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct VIP
{
    [Header("VIP Info")]
    public Sprite VipImage;
    public string VipName, Title;
}

[CreateAssetMenu(fileName = "VIP Library", menuName = "ScriptableObjects/VIPLibrary", order = 1)]
public class VIPLibrary : ScriptableObject
{
    public VIP[] VIPs;
}
