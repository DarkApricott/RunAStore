using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    private float Clean;
    private bool cleaning;
    private Transform CurrentCargo;
    public static bool Pickedup;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);

            for (int hitID = hits.Length - 1; hitID >= 0; hitID--)
            {
                RaycastHit hit = hits[hitID];
                if (hit.transform.CompareTag("Dirty")|| hit.transform.CompareTag("Trash"))
                {
                    cleaning = true;
                    if (Clean > 10)
                    {
                        Destroy(hit.transform.gameObject);
                        Clean = 0;
                        cleaning = false;
                    }
                }
                if (hit.transform.CompareTag("Cargo"))
                {
                    CurrentCargo = hit.transform;
                    Pickedup = true;
                }
                else
                {
                    Pickedup = false;
                }

                

                if (hit.transform.CompareTag("Ground"))
                {
                    if (CurrentCargo != null)
                    {
                        CurrentCargo.position = hit.point + new Vector3(0, 1.8f, 0);
                    }

                    break;
                }
            }
        }
        else
        {
            CurrentCargo = null;
        }
    }

    private void FixedUpdate()
    {
        if (cleaning)
        {
            Clean++;
        }
    }
}



