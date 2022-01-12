using System;
using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerMotor : NetworkBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk,
        Dash,
        Attack,
        Mine,
    }

    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    [SerializeField] float dashDistance;
    [SerializeField] float dashTime;
    [SerializeField] float dashCooldown;
    [SerializeField] float mass;


    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    PlayerController pc;
    CharacterController cc;
    Animator animator;
    float yVel;
    float timeLeft2Dash = 0;
    float dashScaler;


    private bool isDashing => timeLeft2Dash > dashCooldown - dashTime;
    private bool canDash => timeLeft2Dash <= 0;
    private bool isGrounded => Physics.Raycast(transform.position, Vector3.down, cc.height / 2 + .2f, LayerMask.NameToLayer("Player"));
    private bool canMove =>  pc.LatestInput != Vector2.zero && !isDashing;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();


        dashScaler = 2 * (dashDistance / dashTime);
    }

    private void Update()
    {
        ApplyGravity();
        PlayerState currentState = PlayerState.Idle;
        if (IsClient && IsOwner)
        {
            if (ApplyDash())
            {
                currentState = PlayerState.Dash;
            }
            else if (ApplyMovement())
            {
                currentState = PlayerState.Walk;
            }

            UpdatePlayerStateServerRpc(currentState);
        }

        ApplyAnimation(currentState);
    }

    
    public bool ApplyMovement()
    {
        if (!canMove) return false;
        Move(speed);
        return true;
    }

    // REMOVE BUG --- Stop input during dashing resets rotation
    public bool ApplyDash()
    {
        if (!canDash) timeLeft2Dash -= Time.deltaTime; //Lower Cooldown

        if (isDashing && pc.LatestInput != Vector2.zero)
        {
            //MovePlayer
            float timeLeftDashing = dashTime - (dashCooldown - timeLeft2Dash);
            Move(timeLeftDashing * dashScaler);
            return true;
        }
        return false;
    }

    internal void DashRequest()
    {
        if (canDash)
        timeLeft2Dash = dashCooldown;
    }

    private void Move(float speedAmp)
    {
        var relativeDir = new Vector3(pc.LatestInput.x, 0f, pc.LatestInput.y).ToIso();
        var rot = Quaternion.LookRotation(relativeDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        cc.Move(relativeDir * Time.deltaTime * speedAmp);
    }

    void ApplyGravity()
    {
        
        if (cc.collisionFlags == CollisionFlags.Below || isGrounded)
        {
            yVel = 0;
            if (isGrounded) return;
        }

        yVel += -Physics.gravity.magnitude * Time.deltaTime * mass;
        cc.Move(yVel * Vector3.up * Time.deltaTime);
    }

    public void ApplyAnimation(PlayerState state)
    {
        if (state == PlayerState.Idle)
        {
            animator.SetBool("isWalking",false);
        }
        else if (state == PlayerState.Walk)
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
