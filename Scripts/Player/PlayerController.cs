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
    private InputAction dash;
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

        dash = playerInputActions.Player.Dash;
        dash.Enable();

    }

    private void OnDisable()
    {
        movement.Disable();
    }

    public override void OnNetworkSpawn()
    {
        gameObject.name = IsOwner ? "localPlayer" : "remotePlayer";
        if (!(IsOwner && IsClient))
        {
            GFX.material = remoteMaterial;
        }
    }

    private void Update()
    {
        motor.ApplyAnimation();
        if (!(IsOwner && IsClient)) return;
        //motor.SetInput(movement.ReadValue<Vector2>());
        var input = movement.ReadValue<Vector2>();
        var dashRequest = dash.IsPressed();
        if (dashRequest) motor.Dash(input);
        motor.Move(input);
    }

    //Maybe change playerstate in this class

}
