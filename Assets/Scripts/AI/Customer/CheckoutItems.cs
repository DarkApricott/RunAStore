using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class CheckoutItems : ICustomerTask
{
    private CheckoutObject checkout;

    public CustomerBrain cBrain;

    private bool inPaymentProcess = false;

    public void StartTask(CustomerBrain brain)
    {
        cBrain = brain;

        PickCheckout(brain);
    }

    public void PickCheckout(CustomerBrain brain)
    {
        if (brain.oManager.containers[(int)ObjectTypes.checkout].objects.Count != 0 && brain.products.Count != 0) // check if there are any checkouts and if the customer has any items in their inventory
        {
            checkout = brain.oManager.ShortestCheckOut();
            checkout.queue.Add(this);

            checkout.setQueuePositions();
        }
        else
        {
            brain.SetTask(new LeaveStore());
        }
    }

    public void Execute(CustomerBrain brain)
    {
        if(checkout == null)
        {
            PickCheckout(brain);
        }

        if (checkout.queue.First() == this && brain.agent.remainingDistance <= .5f && inPaymentProcess == false && checkout.isOutOfOrder == false)
        {
            brain.StartCoroutine(PayItems(brain));
        }
    }

    public IEnumerator PayItems(CustomerBrain brain)
    {
        if(checkout != null) // check if checkout still exists
        {
            inPaymentProcess = true;


            foreach (int item in brain.products)
            {
                //checkout.SetProductPreview(brain.oManager.productLibrary.products[item].itemModel.GetComponent<MeshFilter>(), brain.oManager.productLibrary.products[item].itemModel.GetComponent<MeshRenderer>());

                brain.uiManager.GainMoney(brain.oManager.productLibrary.products[item].itemSellPrice());

                yield return new WaitForSeconds(2f);
            }

            checkout.ClearPreview();

            checkout.queue.Remove(this);

            checkout.setQueuePositions();

            if(checkout.occupiedWorker != null)
            {
                brain.StartCoroutine(checkout.occupiedWorker.GetComponent<WorkerBrain>().SayMessage("Have a nice day! :D", 1f));
            }

            brain.SetTask(new LeaveStore());
        }
        else
        {
            Debug.Log("No checkout!");
            brain.SetTask(new LeaveStore());
        }
    }
    
    public void SetQueuePos(Vector3 pos)
    {
        cBrain.agent.SetDestination(pos);
    }

    public void EndTask(CustomerBrain brain)
    {

    }
}
