using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerMotor : NetworkBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Dashing
    }

    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    [SerializeField] float dashDistance;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    [SerializeField] float mass = 10;
    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();


    CharacterController cc;
    Animator animator;
    float yVel;
    float timeLeft2Dash = 0;
    float dashScaler;
    private Vector2 latestInput;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        dashScaler = 2 * (dashDistance / dashTime);
    }

    private void Update()
    {
        if (timeLeft2Dash > 0) timeLeft2Dash -= Time.deltaTime;
        if (isDashing)
        {
            //MovePlayer
            float timeLeftDashing = dashTime - (dashCooldown - timeLeft2Dash);

            var relativeDir = new Vector3(latestInput.x, 0f, latestInput.y).ToIso();
            var rot = Quaternion.LookRotation(relativeDir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
            cc.Move(relativeDir * timeLeftDashing * dashScaler * Time.deltaTime);
        }

    }

    private bool isDashing => timeLeft2Dash > dashCooldown - dashTime;
    public void Move(Vector2 input)
    {
        ApplyGravity();
        //Animation
        if (isDashing)
        {
            UpdatePlayerStateServerRpc(PlayerState.Dashing);
            return;
        }
        else if (input == Vector2.zero)
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
            return;//If we don't want to move let's not move
        }
            UpdatePlayerStateServerRpc(PlayerState.Walk);

        //Movement
        
        var relativeDir = new Vector3(input.x, 0f, input.y).ToIso();
        var rot = Quaternion.LookRotation(relativeDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        cc.Move(relativeDir * speed * Time.deltaTime);
    }

    public void Dash(Vector2 input)
    {
        if (timeLeft2Dash > 0) return;
        timeLeft2Dash = dashCooldown;
        latestInput = input;

    }

    void ApplyGravity()
    {
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, cc.height / 2 + .1f, LayerMask.NameToLayer("Player"));
        if (cc.collisionFlags == CollisionFlags.Below || isGrounded)
        {
            yVel = 0;
            if (isGrounded) return;
        }

        yVel += -Physics.gravity.magnitude * Time.deltaTime * mass;

        cc.Move(yVel * Vector3.up * Time.deltaTime);
    }

    public void ApplyAnimation()
    {
        if (networkPlayerState.Value == PlayerState.Idle)
        {
            animator.SetBool("isWalking",false);
        }else if (networkPlayerState.Value == PlayerState.Walk)
        {
            animator.SetBool("isWalking", true);
        }
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
    }
}
