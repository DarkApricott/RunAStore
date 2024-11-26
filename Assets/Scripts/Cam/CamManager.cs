using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamManager : MonoBehaviour
{
    private float mouseX;
    private float mouseY;
    private Transform parent;
    private float startYPos;
    [SerializeField] int maxClamp;
    void Start()
    {
        parent = transform.parent;
        startYPos = transform.position.y;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(2))
        {
            SetMouseInputs();
            Vector3 _newPos = Vector3.ClampMagnitude(new(transform.position.x, startYPos, transform.position.z), maxClamp);
            transform.position = _newPos += (parent.right * -mouseX) + (parent.forward * -mouseY);
        }
        else if (Input.GetMouseButton(1))
        {
            SetMouseInputs();
            parent.rotation = Quaternion.Euler(0, parent.rotation.eulerAngles.y + mouseX * 3, 0);
        }
        else
        {  
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SetMouseInputs()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");
        Cursor.lockState = CursorLockMode.Locked;
    }
}
