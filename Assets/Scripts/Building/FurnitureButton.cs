using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FurnitureButton : MonoBehaviour
{
    [SerializeField] private int id;
    [Space]
    [SerializeField] private TMP_Text objName;
    [SerializeField] private TMP_Text objPrice;
    [SerializeField] private Image icon, typeIcon;
    [SerializeField] private GameObject locked;

    private BuildingSystem system;
    private BuildingLibrary lib;
    private UIManager uiMan;

    private TextMeshProUGUI lockedText;
    private Button button;

    void Awake()
    {
        system = FindObjectOfType<BuildingSystem>();
        lib = system.buildingLibrary;
        uiMan = FindObjectOfType<UIManager>();

        lockedText = locked.GetComponentInChildren<TextMeshProUGUI>();
        button = GetComponent<Button>();

        icon.sprite = lib.buildObjects[id].Icon;

        if (lib.buildObjects[id].TypeIcon != null)
        {
            typeIcon.enabled = true;
            typeIcon.sprite = lib.buildObjects[id].TypeIcon;
        }

        objPrice.text = "€" + lib.buildObjects[id].buildPrice.ToString() + ",-";

        locked.SetActive(lib.buildObjects[id].IsLocked);
        button.enabled = !lib.buildObjects[id].IsLocked;
        LockedFurnitureUpdater();
    }

    public void LockedFurnitureUpdater()
    {
        if (uiMan.EarnedMoney < lib.buildObjects[id].lockedPrice)
        {
            lockedText.text = "Earn €" + (lib.buildObjects[id].lockedPrice - uiMan.EarnedMoney) +",- more to unlock!";
        }
        else
        {
            locked.SetActive(false);
            button.enabled = true;
        }
    }

    public void BuyItem()
    {
        system.SelectObject(id);
    }

}
