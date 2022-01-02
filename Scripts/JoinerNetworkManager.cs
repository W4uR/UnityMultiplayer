using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using System.Text;

public class JoinerNetworkManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private GameObject codeEntryUI;
    [SerializeField] private GameObject leaveButton;


    private void Start()
    {
       // NetworkManager.Singleton.OnServerStarted += HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
    }


    private void OnDestroy()
    {
        if (NetworkManager.Singleton == null) return;

       // NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
    }

    public void Host()
    {

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        NetworkManager.Singleton.StartHost();
    }


    public void Client()
    {
        NetworkManager.Singleton.NetworkConfig.ConnectionData = Encoding.ASCII.GetBytes(codeInputField.text);
        NetworkManager.Singleton.StartClient();
    }

    public void Leave()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        }
        NetworkManager.Singleton.Shutdown();
        codeEntryUI.SetActive(true);
        leaveButton.SetActive(false);
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId) // is us
        {
            codeEntryUI.SetActive(false);
            leaveButton.SetActive(true);
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        if (clientId == NetworkManager.Singleton.LocalClientId) // is us
        {
            codeEntryUI.SetActive(true);
            leaveButton.SetActive(false);
        }
    }



    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        string code = Encoding.ASCII.GetString(connectionData);

        bool approveConnection = code == codeInputField.text;

        callback(true, null, approveConnection, null, null);
    }
}
