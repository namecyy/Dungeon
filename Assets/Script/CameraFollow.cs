    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    private Vector3 targetPos;

    private Vector3 velocity = Vector3.zero;
    private void LateUpdate()
    {
        targetPos = target.position;
        targetPos.z = transform.position.z;
        transform.position =  Vector3.SmoothDamp(transform.position, targetPos, ref velocity, 0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
