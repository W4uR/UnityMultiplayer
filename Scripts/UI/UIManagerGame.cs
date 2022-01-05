using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UIManagerGame : MonoBehaviour
{
    [SerializeField] Button LeaveButton;

    private void Start()
    {
        LeaveButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene(0);
        });
    }
}
