using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class VIPInteraction : MonoBehaviour
{
    [Space]
    [SerializeField] private TextMeshProUGUI[] texts;
    [SerializeField] private Image image;
    [SerializeField] private VIPLibrary vipLib;

    public int VIPID;

    void Start()
    {
        texts[0].text = vipLib.VIPs[VIPID].VipName;
        texts[1].text = vipLib.VIPs[VIPID].Title;
        image.sprite = vipLib.VIPs[VIPID].VipImage;
    }

  
}
