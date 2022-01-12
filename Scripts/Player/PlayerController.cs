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
    private bool myInstance = false;
    PlayerMotor motor;

    [Header("Fields")]
    [SerializeField] Material remoteMaterial;
    [SerializeField] MeshRenderer GFX;

    private Vector2 latestInput;

    public Vector2 LatestInput => latestInput;


    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        motor = GetComponent<PlayerMotor>();
    }

    private void OnEnable()
    {
        movement = playerInputActions.Player.Movement;
        movement.Enable();

        playerInputActions.Player.Dash.performed += HandleDash;
        playerInputActions.Player.Dash.Enable();

    }

  

    private void OnDisable()
    {
        movement.Disable();
        playerInputActions.Player.Dash.performed -= HandleDash;
        playerInputActions.Player.Dash.Disable();
    }

    public override void OnNetworkSpawn()
    {
        if ((IsOwner && IsClient))
        {
            myInstance = true;
            gameObject.name = "LocalPlayer";
            gameObject.tag = "LOCAL";
        }
        else
        {
            GFX.material = remoteMaterial;
            gameObject.name = "RemotePlayer";
        }        
    }

    private void Update()
    {
        if (myInstance)
        {
            latestInput = movement.ReadValue<Vector2>();
        }
    }
    private void HandleDash(InputAction.CallbackContext obj)
    {
        motor.DashRequest();
    }


}
