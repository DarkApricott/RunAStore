using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnBoxes : MonoBehaviour
{
    public GameObject Box;
    public Transform TruckTransform;
    private UIManager uiMan;

    [SerializeField] private ProductLibrary productLib;

    private int amountDiffTypes;

    // List of products sorted on catagories
    private List<List<int>> ListCats = new();
    [SerializeField] private List<int> Cat1 = new();
    [SerializeField] private List<int> Cat2 = new();
    [SerializeField] private List<int> Cat3 = new();
    [SerializeField] private List<int> Cat4 = new();
    [SerializeField] private List<int> Cat5 = new();
    [SerializeField] private List<int> Cat6 = new();

    private float AmountBoxesSpawned;
    public float boxlocal;

    public List<int> ListNewProducts = new();

    [SerializeField] private Transform boxSpawnPoint;

    void Start()
    {
        ListCats.Add(Cat1);
        ListCats.Add(Cat2);
        ListCats.Add(Cat3);
        ListCats.Add(Cat4);
        ListCats.Add(Cat5);
        ListCats.Add(Cat6);

        uiMan = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        if (uiMan.HasCheckedOut == true)
        {
            SortProducts(ref uiMan.NewlyBoughtProducts, uiMan.AmountItemsBought);
            uiMan.HasCheckedOut = false;
        }
    }

    // Puts products in seperate lists based on catagories
    public void SortProducts(ref List<int> _newProductList, int _amountProducts)
    {
        amountDiffTypes = 0;
        AmountBoxesSpawned = 0;

        _newProductList.Sort();
        ListNewProducts = _newProductList;

        for (int i = 0; i < _amountProducts; i++)
        {
            if (i >= 1)
            {
                if (productLib.products[_newProductList[i - 1]].storageType != productLib.products[_newProductList[i]].storageType)
                {
                    amountDiffTypes++;
                }
            }

            ListCats[amountDiffTypes].Add(_newProductList[i]);
        }

        if (_newProductList.Count <= 10 && _newProductList.Count > 0 && amountDiffTypes == 0)
        {
            AmountBoxesSpawned = 1;
        }

        SpawnBoxesOfCatagory(_newProductList);
    }

    // Calculates how many boxes must be spawned per catagory + 10 products per box
    private void SpawnBoxesOfCatagory(List<int> _newProductList)
    {
        int _totalBoxesSpawned = 0;

        if (amountDiffTypes > 0)
        {
            for (int i = 0; i < amountDiffTypes + 1; i++)
            {
                AmountBoxesSpawned = Mathf.Ceil((float)ListCats[i].Count / 10);
                _totalBoxesSpawned += (int)AmountBoxesSpawned;
                ListCats[i].Clear();
            }

            for (int i = 0; i < _totalBoxesSpawned; i++)
            {
                SpawnBox();
            }
        }
        else
        {
            AmountBoxesSpawned = Mathf.Ceil((float)_newProductList.Count / 10);

            for (int i = 0; i < AmountBoxesSpawned; i++)
            {
                SpawnBox();
            }
        }
    }

    private void SpawnBox()
    {
        Instantiate(Box, boxSpawnPoint.position, Quaternion.identity);
    }
}

