using Unity.Netcode;
using Unity.Netcode.Samples;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerMotor : NetworkBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walk
    }

    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    [SerializeField] float mass = 10;
    [SerializeField]
    private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();


    CharacterController cc;
    Animator animator;
    float yVel;
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }


    public void Move(Vector2 input)
    {
        ApplyGravity();
        //Animation
        if (input == Vector2.zero) {
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
