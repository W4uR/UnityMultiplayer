using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using System;

public class PlayerController : NetworkBehaviour
{

    private PlayerInputActions playerInputActions;
    private InputAction movement;
    PlayerMotor motor;

    [Header("Fields")]
    [SerializeField] Material remoteMaterial;
    [SerializeField] MeshRenderer GFX;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        motor = GetComponent<PlayerMotor>();
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
        if (!IsOwner)
        {
            GFX.material = remoteMaterial;
        }
    }

    private void Update()
    {
        if (!IsOwner) return;
        motor.Move(GetInput());
    }


    private Vector3 GetInput()
    {
       return new Vector3(movement.ReadValue<Vector2>().x, 0f, movement.ReadValue<Vector2>().y);
    }


}
