using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{

    private PlayerInputActions playerInputActions;
    private InputAction movement;
    CharacterController cc;
    private Vector3 input;

    [Header("Fields")]

    [SerializeField] float speed;
    [SerializeField] float turnSpeed;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        cc = GetComponent<CharacterController>();
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        movement = playerInputActions.Player.Movement;
        movement.Enable();

    }

    private void OnDisable()
    {
        movement.Disable();
    }

    public override void OnNetworkSpawn()
    {
        gameObject.name = IsOwner ? "localPlayer" : "remotePlayer";
       // if (IsOwner) 
    }

    private void Update()
    {
        if (!IsOwner) return;
       // CameraFollow.Instance.SetTarget(transform);
        GetInput();
        Move();
    }

    private void GetInput()
    {
        input = new Vector3(movement.ReadValue<Vector2>().x, 0f, movement.ReadValue<Vector2>().y);
    }
    private void Move()
    {
        
        if (input != Vector3.zero)
        {
            var relative = input.ToIso();
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation,rot,turnSpeed*Time.deltaTime);
            if (transform.rotation != rot) return;//Until not rotated fully, don't move
        }
        cc.SimpleMove(transform.forward * input.magnitude * speed);
    }

}
