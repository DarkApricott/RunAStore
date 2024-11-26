using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class BuyItem : ICustomerTask
{
    private NavMeshAgent agent;
    private ObjectManager oManager;

    private GameObject destinationObj;

    private float targetTreshold = 2.5f;
    private bool hasReachedDestination;

    private Coroutine buyCoroutine;

    // wishlist
    private int minItems = 1;
    private int maxItems = 10;
    private int buyCheckList;

    private int buyChecker;

    public void StartTask(CustomerBrain brain)
    {
        agent = brain.agent;
        oManager = brain.oManager;

        if(brain.cType == CustomerType.normal)
        {
            buyCheckList = Random.Range(minItems, maxItems);
        }
        else
        {
            buyCheckList = maxItems;
        }
        buyChecker = buyCheckList;

        PickShelve(brain);
    }

    private void PickShelve(CustomerBrain brain)
    {
        if(buyCheckList > 0 && oManager.containers[(int)ObjectTypes.shelve].objects.Count != 0)
        {
            destinationObj = oManager.containers[(int)ObjectTypes.shelve].objects[Random.Range(0, oManager.containers[(int)ObjectTypes.shelve].objects.Count)];
            hasReachedDestination = false;
        }
        else
        {
            brain.SetTask(new CheckoutItems());
        }
    }

    public void Execute(CustomerBrain brain)
    {
        float dist = 555;

        if (destinationObj != null && hasReachedDestination == false)
        {
            agent.SetDestination(destinationObj.transform.position);
            dist = Vector3.Distance(brain.gameObject.transform.position, destinationObj.transform.position);
        }
        else
        {
            agent.SetDestination(agent.transform.position);
        }

        if(dist <= targetTreshold && hasReachedDestination == false)
        {
            if(buyCoroutine != null)
            {
                brain.StopCoroutine(buyCoroutine);
            }
            buyCoroutine = brain.StartCoroutine(Buy(brain));
        }

        if(destinationObj == null)
        {
            PickShelve(brain);
        }
    }

    public IEnumerator Buy(CustomerBrain brain)
    {
        hasReachedDestination = true;

        if (destinationObj != null)
        {
            destinationObj.GetComponent<ShelveObject>().GrabItem(brain, 0);

            yield return new WaitForSeconds(3f);

        }
        buyCheckList--;
        PickShelve(brain);

        buyCoroutine = null;
    }

    public void EndTask(CustomerBrain brain)
    {
        if(buyChecker == brain.products.Count)
        {
            brain.StartCoroutine(brain.ChangeCustomerExperience(CustomerExperience.positive));
        }

        if(brain.products.Count == 0)
        {
            brain.StartCoroutine(brain.ChangeCustomerExperience(CustomerExperience.negative));
        }
    }
}
