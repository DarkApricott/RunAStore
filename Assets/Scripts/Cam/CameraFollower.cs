using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    private Transform cam;
    [SerializeField] private Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        cam = Camera.main.transform;
        transform.LookAt(cam.position + offset);
    }
}
