using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShelveObject : MonoBehaviour
{
    SpawnBoxes spawnBoxesCS;
    
    public int storageCapacity;

    public List<int> products = new List<int>();

    public ShelveTypes Type;
    [SerializeField] private Sprite[] typeIcons;
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private Image iconRenderer;

    public bool occupied;
    public GameObject percent20;
    public GameObject percent40;
    public GameObject percent60;
    public GameObject percent80;
    public GameObject percent100;

    public double FilledPercentage()
    {
        return ((double)products.Count / storageCapacity) * 100;
    }

    public int percentageRounded;

    private void Start()
    {
        spawnBoxesCS = FindObjectOfType<SpawnBoxes>();

        FilledPercentage();

        iconRenderer.sprite = typeIcons[(int)Type];
    }

    public void GrabItem(CustomerBrain c, int index)
    {
        if(products.Count > 0)
        {
            if (products[index] != null)
            {
                c.products.Add(products[index]);
                products.Remove(products[index]);
            }
            else
            {
                products.Remove(products[index]);

                GrabItem(c, index += 1);
            }
        }
        else
        {
            c.StartCoroutine(c.ChangeCustomerExperience(CustomerExperience.negative));
        }

        FilledPercentage();
    }

    private void LateUpdate()
    {
        percentageRounded = (int)FilledPercentage();

        if(percentageRounded <= 20)
        {
            percentageText.gameObject.SetActive(true);
            percentageText.text = "<color=red>" + "!!" + "</color>";
        }
        else
        {
            percentageText.gameObject.SetActive(false);
        }
        
        if(percentageRounded > 20)
        {
            percent20.SetActive(true);
        }
        if (percentageRounded > 40)
        {
            percent40.SetActive(true);
            
        }
        if (percentageRounded > 60)
        {
            percent60.SetActive(true);
        }
        if (percentageRounded > 80)
        {
            percent80.SetActive(true);
        }
        if (percentageRounded >= 100)
        {
            percent100.SetActive(true);
        }

        if (percentageRounded < 20)
        {
            percent20.SetActive(false);
        }
        if (percentageRounded < 40)
        {
            percent40.SetActive(false);
        }
        if (percentageRounded < 60)
        {
            percent60.SetActive(false);
        }
        if (percentageRounded < 80)
        {
            percent80.SetActive(false);
        }
        if (percentageRounded < 100)
        {
            percent100.SetActive(false);
        }

    }

    private void OnDestroy()
    {
        spawnBoxesCS.ListNewProducts = products;
        spawnBoxesCS.SortProducts(ref spawnBoxesCS.ListNewProducts, products.Count);
    }

}
