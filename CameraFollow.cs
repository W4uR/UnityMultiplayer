using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField][Range(0f,1f)] float smoothSpeed;
    [SerializeField] Vector3 offset;

    public void SetTarget(Transform target) { if (this.target == null) this.target = target; }
    public static CameraFollow Instance;
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        target = GameObject.Find("localPlayer").transform;
    }
    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed);
        transform.position = smoothedPos;
    }
}
