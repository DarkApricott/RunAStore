using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed;
    public float rotateSpeed;

    [SerializeField] private Vector2 maxVerticalRotateAnchor, horizontalRotateAnchors;

    [SerializeField] private Transform horizontalPivot;

    [SerializeField] private Transform desiredLocation;

    private int horizontalRotateInput, verticalRotateInput;

    private float oldRotAnchor;
    private float currentRotAnchor;

    float rotTime;

    private float cameraDistance;

    private Transform cam;

    private void Start()
    {
        cameraDistance = Vector3.Distance(transform.position, desiredLocation.position);
        cam = Camera.main.transform;
    }

    private void Update()
    {
        Move();

        //Vector3 direction = desiredLocation.position - transform.position;

        //RaycastHit ray;

        //if (Physics.Raycast(transform.position, direction, out ray, cameraDistance))
        //{
        //    cam.position = ray.point;
        //}
        //else cam.position = desiredLocation.position;
    }

    public void Move()
    {
        Vector3 MoveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if(MoveInput.z != 0)
        {
            transform.position += MoveInput.z * moveSpeed * Time.deltaTime * transform.forward;
        }
        if (MoveInput.x != 0)
        {
            transform.position += MoveInput.x * moveSpeed * Time.deltaTime * transform.right;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            horizontalRotateInput = 1;
        } else if (Input.GetKey(KeyCode.E))
        {
            horizontalRotateInput = -1;
        }
        else
        {
            horizontalRotateInput = 0;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            verticalRotateInput = -1;
        }
        else if (Input.GetKey(KeyCode.X))
        {
            verticalRotateInput = 1;
        }
        else
        {
            verticalRotateInput = 0;
        }


        transform.Rotate(0, horizontalRotateInput * rotateSpeed * Time.deltaTime, 0);

        if (Input.GetKeyDown(KeyCode.Space) && rotTime >= 1){
            if(horizontalPivot.localRotation.eulerAngles.x == horizontalRotateAnchors.x)
            {
                ChangeTopView(horizontalRotateAnchors.y);
            } 
            else if(horizontalPivot.localRotation.eulerAngles.x != horizontalRotateAnchors.x && rotTime >= 1)
            {
                ChangeTopView(0);
            }
        }

        horizontalPivot.localRotation = Quaternion.Lerp(Quaternion.Euler(oldRotAnchor, 0, 0), Quaternion.Euler(currentRotAnchor, 0, 0), rotTime);

        rotTime += Time.deltaTime * 2;
    }

    public void ChangeTopView(float localRotation)
    {
        oldRotAnchor = currentRotAnchor;
        currentRotAnchor = localRotation;

        rotTime = 0;
    }
}
