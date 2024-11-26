using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

public class Box : MonoBehaviour
{
    private UIManager uiMan;
    private ObjectManager oManager;
    private SpawnBoxes spawnBoxes;
    private Tutorial tut;

    [SerializeField] private ProductLibrary productLib;
    [SerializeField] private List<int> products = new();
    [SerializeField] private TMP_Text boxText;

    private readonly int maxCapacity = 10;
    [SerializeField] private ShelveTypes type;
    [SerializeField] private Sprite[] iconSprites;
    [SerializeField] private Image iconImage;
    [SerializeField] private SpriteRenderer[] icons;

    void Start()
    {
        uiMan = FindObjectOfType<UIManager>();
        oManager = FindObjectOfType<ObjectManager>();
        spawnBoxes = FindObjectOfType<SpawnBoxes>();
        tut = FindObjectOfType<Tutorial>();

        FillBox(ref spawnBoxes.ListNewProducts);

        SetBoxText();

        oManager.boxes.Add(gameObject);
    }

    // Sets catagory based on first gathered item
    private void FillBox(ref List<int> _productsList)
    {
        while ((products.Count < maxCapacity) && (_productsList.Count > 0))
        {
            if (products.Count >= 1)
            {
                // Fills itself with products of set catagory
                if (productLib.products[_productsList[0]].storageType == type)
                {
                    products.Add(_productsList[0]);
                    _productsList.RemoveAt(0);
                }
                else
                {
                    break;
                }
            }
            else
            {
                products.Add(_productsList[0]);
                _productsList.RemoveAt(0);
                type = productLib.products[products[0]].storageType;

                iconImage.sprite = iconSprites[(int)type];

                foreach (SpriteRenderer item in icons)
                {
                    item.sprite = productLib.products[products[0]].itemIcon;
                }
            }
        }

        // Delete if box spawned empty (rare glitch)
        if (products.Count == 0)
        {
            Destroy(gameObject);
        }
    }

    void SetBoxText()
    {
        boxText.text = products.Count + "/" + maxCapacity;
    }
    // Checks if what box hit is shelf
    private void OnCollisionEnter(Collision _other)
    {
        if (_other.gameObject.TryGetComponent(out ShelveObject shelve))
        {
            // Is shelf stockable with current products?
            if (shelve.Type == type)
            {
                StartCoroutine(RestockShelf(shelve));
            }
        }

        // Add items to Stocker inventory
        if (_other.gameObject.TryGetComponent(out WorkerBrain worker))
        {
            StartCoroutine(SupplyStocker(worker));
        }
    }

    // Fills if shelf / box isn't empty, then adds to shelf inventory
    private IEnumerator RestockShelf(ShelveObject _shelf)
    {
        _shelf.occupied = true;
        int _stockDiff = _shelf.storageCapacity - _shelf.products.Count;

        for (int i = 0; i < _stockDiff; i++)
        {
            if (products.Count != 0)
            {
                _shelf.products.Add(products[0]);
                products.RemoveAt(0);

                _shelf.FilledPercentage();

                SetBoxText();
                tut.StepDone(2);
                tut.NextStep(4);
                yield return new WaitForSeconds(0.15f);
            }
        }

        _shelf.occupied = false;

        if (products.Count == 0)
        {
            oManager.boxes.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    // Add products to Stocker inventory
    private IEnumerator SupplyStocker(WorkerBrain worker)
    {
        if (worker.type == WorkerType.stocker)
        {
            for (int i = 0; i < maxCapacity; i++)
            {
                if (products.Count != 0)
                {
                    worker.products.Add(products[0]);
                    products.RemoveAt(0);

                    SetBoxText();
                    yield return new WaitForSeconds(0f);
                }
            }

            if (products.Count == 0)
            {
                oManager.boxes.Remove(this.gameObject);
                Destroy(gameObject);
            }
        }
    }
}
