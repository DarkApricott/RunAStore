using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerTexturePicker : MonoBehaviour
{
    private bool isVIP;

    [SerializeField] private GameObject infoCard;

    public List<Material> normalCustomerMats;

    public List<Material> vipCustomerMats;

    [SerializeField] private MeshRenderer[] renderers;

    public void pickMat(CustomerType type)
    {
        if(type == CustomerType.normal)
        {
            int customerIndex = Random.Range(0, normalCustomerMats.Count);

            foreach (MeshRenderer item in renderers)
            {
                item.material = normalCustomerMats[customerIndex];
            }
        } else if(type == CustomerType.vip)
        {
            isVIP = true;
            int customerIndex = Random.Range(0, vipCustomerMats.Count);

            GetComponentInChildren<VIPInteraction>().VIPID = customerIndex;

            foreach (MeshRenderer item in renderers)
            {
                item.material = vipCustomerMats[customerIndex];
            }
        }
    }

    private void OnMouseDown()
    {
        if (isVIP)
        {
            infoCard.SetActive(!infoCard.activeInHierarchy);
        }
    }
}
