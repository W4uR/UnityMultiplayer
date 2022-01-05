using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;

public class TransportPicker : Singleton<TransportPicker>
{
    public UNetTransport UNT => NetworkManager.Singleton.GetComponent<UNetTransport>();
    public UnityTransport UT => NetworkManager.Singleton.GetComponent<UnityTransport>();
    public NetworkTransport PickTransport(bool isLan)
    {
        return isLan ? UNT : UT;
    }
}
