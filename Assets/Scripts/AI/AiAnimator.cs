using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiAnimator : MonoBehaviour
{
    private Animator anim;
    private WorkerBrain workerBrain;

    public Transform AiTransform;
    float idleTimer = 0;

    private void Start()
    {
        workerBrain = GetComponentInParent<WorkerBrain>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            anim.SetBool("IsWalking", AiTransform.hasChanged);

            AiTransform.hasChanged = false;

            if (workerBrain != null && !anim.GetBool("IsWalking"))
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= Random.Range(10, 30))
                {
                    anim.SetTrigger("IdleExtra");
                    idleTimer = 0;
                }
            }
        }
    }
}
