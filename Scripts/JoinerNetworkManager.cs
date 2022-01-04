using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using TMPro;
using System;
using System.Text;
using UnityEngine.SceneManagement;

public class JoinerNetworkManager : MonoBehaviour
{

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () => {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        };
    }

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }


    public void Client()
    {
        NetworkManager.Singleton.StartClient();
    }

}
