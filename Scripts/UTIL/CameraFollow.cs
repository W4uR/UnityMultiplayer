using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    [SerializeField] Transform target;
    [SerializeField][Range(0f,1f)] float smoothSpeed;
    [SerializeField] Vector3 offset;


    private void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("LOCAL").transform;
        }

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }
}
