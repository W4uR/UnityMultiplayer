using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    TMP_InputField IN_joinCode_Ip;
    [SerializeField]
    Button HostButton;
    [SerializeField]
    Button JoinButton;
    [SerializeField]
    Toggle ToggleLan;
    [SerializeField]
    Toggle ToggleFullscreen;
    [SerializeField]
    Button QuitGame;
    [SerializeField]
    GameObject Overlay;
    [SerializeField]
    Button CancelButton;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += () => {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        };

        HostButton.onClick.AddListener(async () =>
        {
            Overlay.SetActive(true);
            try
            {
                if (RelayManager.Instance.IsRelayEnabled)
                    await RelayManager.Instance.SetupRelay();
            }
            catch
            {
                print("Something went wrong...");
                Overlay.SetActive(false);
                return;
            }
            


            if (!NetworkManager.Singleton.StartHost())
            {
                print("Couldn't start hosting");
                Overlay.SetActive(false);
                NetworkManager.Singleton.Shutdown();
            }
            
        });
        JoinButton.onClick.AddListener(async () =>
        {
            if (string.IsNullOrEmpty(IN_joinCode_Ip.text)) return;
            Overlay.SetActive(true);
            try
            {
                if (RelayManager.Instance.IsRelayEnabled)
                {
                    await RelayManager.Instance.JoinRelay(IN_joinCode_Ip.text);
                }
                else
                {
                    string ip = IN_joinCode_Ip.text;
                    if (!ip.IsIP())
                        throw new Exception();
                    TransportPicker.Instance.UNT.ConnectAddress = ip;
                }
            }
            catch
            {
                print("Invalid input");
                Overlay.SetActive(false);
                return;
            }
            

            if (!NetworkManager.Singleton.StartClient())
            {
                print("Something ain't right");
                Overlay.SetActive(false);
                NetworkManager.Singleton.Shutdown();
            }

            
        });
        ToggleLan.onValueChanged.AddListener((bool isLan) =>
        {
            IN_joinCode_Ip.placeholder.GetComponent<TMP_Text>().text = isLan ? "IP Address" : "Join Code";
            IN_joinCode_Ip.text = "";
            NetworkManager.Singleton.NetworkConfig.NetworkTransport = TransportPicker.Instance.PickTransport(isLan);

        });
        ToggleFullscreen.onValueChanged.AddListener((bool isFullsc) => {
            if(isFullsc)
                Screen.SetResolution(Screen.width*2, Screen.height*2, isFullsc);
            else
                Screen.SetResolution(Screen.width/2, Screen.height/2, isFullsc);
        });
        CancelButton.onClick.AddListener(() =>
        {
            Overlay.SetActive(false);
            NetworkManager.Singleton.Shutdown();
        });
        QuitGame.onClick.AddListener(() =>
        {
            Application.Quit(0);
        });
    }

    private void Update()
    {
        if (Overlay.activeSelf)
        {
            Overlay.GetComponent<Image>().color = TransformH(Overlay.GetComponent<Image>().color, Time.deltaTime * 6f);
        }
    }

    Color TransformH(Color col, float H)
    {
        float U = (float)Math.Cos(H * Math.PI / 180);
        float W = (float)Math.Sin(H * Math.PI / 180);

        return new Color(
            (.299f + .701f * U + .168f * W) * col.r
            + (.587f - .587f * U + .330f * W) * col.g
            + (.114f - .114f * U - .497f * W) * col.b,
            (.299f - .299f * U - .328f * W) * col.r
            + (.587f + .413f * U + .035f * W) * col.g
            + (.114f - .114f * U + .292f * W) * col.b,
            (.299f - .3f * U + 1.25f * W) * col.r
            + (.587f - .588f * U - 1.05f * W) * col.g
            + (.114f + .886f * U - .203f * W) * col.b
            );
    }


}
