using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Samples;

public class PlayerMotor : NetworkBehaviour
{


    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    [SerializeField] float mass = 10;
    CharacterController cc;
    float yVel;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }


    public void Move(Vector2 input)
    {
        ApplyGravity();
        if (input == Vector2.zero) return;
        var relativeDir = new Vector3(input.x, 0f, input.y).ToIso();
        var rot = Quaternion.LookRotation(relativeDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        cc.Move(relativeDir * speed * Time.deltaTime);
    }
    /*

    NetworkVariable<float> xVel = new NetworkVariable<float>();
    NetworkVariable<float> yVel = new NetworkVariable<float>();
    NetworkVariable<float> zVel = new NetworkVariable<float>();

    Vector2 prevInput;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    private void Update()
    {

        if (!IsServer) return;
            ApplyGravity();
        
        if (xVel.Value == 0 && zVel.Value == 0) return;
            var relative = new Vector3(xVel.Value,0f,zVel.Value).ToIso();
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
            cc.SimpleMove(relative * speed);
    }


    public void Move(Vector2 input)
    {
        if (prevInput != input)
            prevInput = input;
            MoveMeServerRpc(input);
        
    }

    [ServerRpc]
    public void MoveMeServerRpc(Vector2 input)
    {

        xVel.Value = input.x;
        zVel.Value = input.y;

    }

    */
    public void ApplyGravity()
    {
        if (cc.collisionFlags == CollisionFlags.Below){ yVel = 0; return; }

        yVel += -Physics.gravity.magnitude * Time.deltaTime * mass;

        cc.Move(yVel * Vector3.up * Time.deltaTime);
    }
}
