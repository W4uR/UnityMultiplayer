using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;

public class EnemyController : NetworkBehaviour
{
    NavMeshAgent agent;

    Transform nexus;
    // Start is called before the first frame update
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        nexus = GameObject.FindGameObjectWithTag("NEXUS").transform;
    }

    private void Start()
    {
        agent.SetDestination(nexus.position);
    }
    // Update is called once per frame
    void Update()
    {
        if ((nexus.position - transform.position).sqrMagnitude <= 3f)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
