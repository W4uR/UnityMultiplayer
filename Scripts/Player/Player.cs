using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{

    string playerName;
    [SerializeField]
    GameObject playerObjectPrefab;

    NetworkObject myCharacer;


}
