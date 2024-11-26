using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StockItemManager : MonoBehaviour
{
    [SerializeField] private string productName;
    private int product;
    private UIManager uiMan;
    [SerializeField] private TextMeshProUGUI amountText;
    [HideInInspector] public int Amount;
    bool isMouseDown;
    bool isMouseDown2;
    private float timer;
    [SerializeField] private Image sortIcon;

    [SerializeField] private GameObject locked;
    [SerializeField] private Button[] button;
    private TextMeshProUGUI lockedText;

    [SerializeField] private ProductLibrary pLibrary;
    [SerializeField] private BuildingLibrary bLibrary;
    private int id;
    void Start()
    {
        uiMan = FindObjectOfType<UIManager>();
        lockedText = locked.GetComponentInChildren<TextMeshProUGUI>();

        for (int i = 0; i < pLibrary.products.Length; i++)
        {
            if (pLibrary.products[i].productName == productName)
            {
                product = i;
                break;
            }
        }

        if (productName != null)
        {
            transform.GetChild(7).GetComponent<TextMeshProUGUI>().text = "€" + pLibrary.products[product].itemBuyPrice.ToString();
            sortIcon.sprite = pLibrary.products[product].sortIcon;
        }

     
        for (int i = 0; i < bLibrary.buildObjects.Length; i++)
        {
            if (bLibrary.buildObjects[i].TypeIcon == pLibrary.products[product].sortIcon)
            {
                id = i;
            }
        }

        locked.SetActive(bLibrary.buildObjects[id].IsLocked);
        button[0].enabled = !bLibrary.buildObjects[id].IsLocked;
        button[1].enabled = !bLibrary.buildObjects[id].IsLocked;

        LockedStockUpdater();
    }

    public void Reset()
    {
        Amount = 0;
        amountText.text = "0";
    }

    private void Update()
    {
        if (isMouseDown || isMouseDown2)
        {
            timer++;

            if (timer > 30)
            {
                if (isMouseDown && Amount < 99)
                {
                    AddItemToCart();
                }
                else if (isMouseDown2 && Amount > 0)
                {
                    DeleteItemFromCart();
                }
            }
        }
    }

    public void AddItemToCart()
    {
        if (Amount < 99 && uiMan.AmountItemsBought < 200)
        {
            Amount++;
            uiMan.AmountItemsBought++;
            amountText.text = Amount.ToString();

            uiMan.NewlyBoughtProducts.Add(product);
            uiMan.TotalStockPriceUpdater(pLibrary.products[product].itemBuyPrice);
        }
    }

    public void MouseDown()
    {
        isMouseDown = true;
    }

    public void MouseDown2()
    {
        isMouseDown2 = true;
    }

    public void MouseUp()
    {
        isMouseDown = false;
        isMouseDown2 = false;
        timer = 0;
    }

    public void DeleteItemFromCart()
    {
        if (Amount > 0)
        {
            Amount--;
            uiMan.AmountItemsBought--;
            amountText.text = Amount.ToString();

            uiMan.NewlyBoughtProducts.Remove(product);
            uiMan.TotalStockPriceUpdater(-pLibrary.products[product].itemBuyPrice);
        }
    }

    public void LockedStockUpdater()
    {
        if (uiMan.EarnedMoney < bLibrary.buildObjects[id].lockedPrice)
        {
            lockedText.text = "Earn €" + (bLibrary.buildObjects[id].lockedPrice - uiMan.EarnedMoney) + ",- more to unlock!";
        }
        else
        {
            locked.SetActive(false);
            button[0].enabled = true;
            button[1].enabled = true;
        }
    }
}
