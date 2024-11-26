using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutObject : MonoBehaviour
{
    [HideInInspector] public bool isOutOfOrder;

    public bool isSelfCheckout;
    public GameObject occupiedWorker;
    public Transform cashierPosition;
    [Space]
    public List<CheckoutItems> queue;

    [SerializeField] private float queueBaseOffset;
    [SerializeField] private float queueWaitOffset;

    [SerializeField] private MeshFilter previewFilter;
    [SerializeField] private MeshRenderer previewRenderer;

    public void setQueuePositions()
    {
        for (int i = 0; i < queue.Count; i++)
        {
            queue[i].SetQueuePos(transform.position + (transform.forward * queueWaitOffset * i) + (transform.forward * queueBaseOffset));
        }
    }

    public void SetProductPreview(MeshFilter filter, MeshRenderer renderer)
    {
        previewRenderer.enabled = true;

        previewFilter.mesh = filter.sharedMesh;
        previewRenderer.materials = renderer.sharedMaterials;
    }

    public void ClearPreview()
    {
        previewRenderer.enabled = false;
    }

    private void LateUpdate()
    {
        if(isSelfCheckout == false)
        {
            if(occupiedWorker == null)
            {
                isOutOfOrder = true;
            } else if(occupiedWorker != null)
            {
                isOutOfOrder = false;
            }
        }
        else
        {
            isOutOfOrder = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position + (transform.forward * queueBaseOffset), .5f);
    }
}
