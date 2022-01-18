using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    [SerializeField] GameObject EnemyPrefab;
    [SerializeField] float enemySpawnerCD;

    private float time2Spawn;
    // Start is called before the first frame update
    public void Start()
    {
        NetworkObjectPool.Singleton.InitializePool();
    }
    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        time2Spawn -= Time.deltaTime;
        if (time2Spawn <= 0)
        {
            time2Spawn = enemySpawnerCD;
            //spawn Enemy;
            NetworkObject go = NetworkObjectPool.Singleton.GetNetworkObject(EnemyPrefab);
            go.transform.position = transform.position;
            go.Spawn();
        }
    }
}
