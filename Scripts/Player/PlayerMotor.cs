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
    [SerializeField] float dashDuration;
    [SerializeField] float dashMaxCD;
    [SerializeField] float mass;


    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();


    PlayerController pc;
    CharacterController cc;
    Animator animator;
    float yVel;
    float dashCD = 0;
    float dashScaler;
    PlayerState currentState = PlayerState.Idle;


    private float timeLeftDashing => dashDuration - (dashMaxCD - dashCD);
    private bool isDashing => timeLeftDashing > 0;
    private bool canDash => dashCD <= 0 && (currentState == PlayerState.Idle || currentState == PlayerState.Walk) && isGrounded;
    private bool isGrounded => Physics.Raycast(transform.position, Vector3.down, cc.height / 2 + .2f, LayerMask.NameToLayer("Player"));
    private bool want2Move => pc.LatestInput != Vector2.zero;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        dashScaler = 2 * (dashDistance / dashDuration);

    }


    private void Update()
    {
        LowerCoolDowns();
        ApplyGravity();
        if (IsClient && IsOwner)
        {

            //Apply state-specific actions

            CancelStates();
            if (currentState == PlayerState.Idle || currentState == PlayerState.Walk)
            {
                var rot = Quaternion.LookRotation(pc.relativeDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
            }

            if (want2Move && currentState != PlayerState.Dash)
            {
                currentState = PlayerState.Walk;
                Walk();
            }
            else if (currentState == PlayerState.Dash)
            {              
                Dash();
            }
            else
            {
                currentState = PlayerState.Idle;
            }
            
            UpdatePlayerStateServerRpc(currentState);
        }

       ApplyAnimation(IsOwner ? currentState : networkPlayerState.Value);
    }


    public void Walk()
    {
        //Rotation
        /*
        var rot = Quaternion.LookRotation(pc.relativeDir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
        */
        //transform.rotation = Quaternion.LookRotation(pc.relativeDir, Vector3.up);
        //Movement
        cc.Move(pc.relativeDir * Time.deltaTime * speed);
    }

    // REMOVE BUG --- Stop input during dashing resets rotation
    public void Dash()
    {
        transform.rotation = Quaternion.LookRotation(pc.relativeDir, Vector3.up);
        //Movement
        cc.Move(pc.relativeDir * Time.deltaTime * timeLeftDashing * dashScaler);
    }

    internal void DashRequest()
    {
        if (canDash)
        {
            currentState = PlayerState.Dash;
            dashCD = dashMaxCD;        
        }
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

    private void LowerCoolDowns()
    {
        if (dashCD > 0) dashCD -= Time.deltaTime;
        /*
         * 
         */

    }

    private void CancelStates()
    {
        if (currentState == PlayerState.Dash && isDashing == false)
        {
            currentState = PlayerState.Idle;
        }
    }

    public void ApplyAnimation(PlayerState state)
    {
        if (state == PlayerState.Idle)
        {
            animator.SetBool("isWalking", false);
        }
        else if (state == PlayerState.Walk)
        {
            animator.SetBool("isWalking", true);
        }
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        if (networkPlayerState.Value == state) return;

        networkPlayerState.Value = state;
    }
}
