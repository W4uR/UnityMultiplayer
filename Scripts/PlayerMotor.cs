using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMotor : NetworkBehaviour
{

    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    CharacterController cc;
    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }


    public void Move(Vector3 input)
    {
        print(input);


        if (input != Vector3.zero)
        {
            var relative = input.ToIso();
            var rot = Quaternion.LookRotation(relative, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, turnSpeed * Time.deltaTime);
            cc.SimpleMove(relative * speed);
        }
    }
}
