using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

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
    private Vector2 latestNot0Input;
    public Vector2 LatestInput => latestInput;
    public Vector2 LatestNot0Input => latestNot0Input; //latestInput == Vector2.zero ? new Vector2(transform.forward.x,transform.forward.z) : latestInput;
    public Vector3 relativeDir => new Vector3(LatestNot0Input.x, 0f, LatestNot0Input.y).ToIso();


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
            if (latestInput != Vector2.zero)
            {
                latestNot0Input = latestInput;
            }
        }
    }
    private void HandleDash(InputAction.CallbackContext obj)
    {
        motor.DashRequest();
    }


}
