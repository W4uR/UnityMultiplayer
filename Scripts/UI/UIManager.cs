using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField]
    TMP_InputField joinCodeIpInput;
    [SerializeField]
    Button HostButton;
    [SerializeField]
    Button JoinButton;
    [SerializeField]
    Toggle ToggleLan;
    [SerializeField]
    Toggle ToggleFullscreen;
    [SerializeField]
    TMP_Dropdown ResolutionDropdown;
    [SerializeField]
    Button QuitGame;
    [SerializeField]
    GameObject Overlay;
    [SerializeField]
    Button CancelButton;

    private void Start()
    {
        Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
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
            if (string.IsNullOrEmpty(joinCodeIpInput.text)) return;
            Overlay.SetActive(true);
            try
            {
                if (RelayManager.Instance.IsRelayEnabled)
                {
                    await RelayManager.Instance.JoinRelay(joinCodeIpInput.text);
                }
                else
                {
                    string ip = joinCodeIpInput.text;
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
            joinCodeIpInput.placeholder.GetComponent<TMP_Text>().text = isLan ? "IP Address" : "Join Code";
            joinCodeIpInput.text = "";
            NetworkManager.Singleton.NetworkConfig.NetworkTransport = TransportPicker.Instance.PickTransport(isLan);

        });
        ToggleFullscreen.onValueChanged.AddListener((bool isFullsc) => {

            Screen.fullScreen = isFullsc;
        });
        ResolutionDropdown.onValueChanged.AddListener((int index) =>
        {
            string[] res = ResolutionDropdown.options[index].text.Split('x');

            Screen.SetResolution(int.Parse(res[0]), int.Parse(res[1]), ToggleFullscreen.isOn);
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
            Overlay.GetComponent<Image>().color = Helper.TransformH(Overlay.GetComponent<Image>().color,Time.deltaTime *-16f);
        }
    }

    //Move this at somepoint
   


}
