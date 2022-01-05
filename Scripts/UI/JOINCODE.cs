using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class JOINCODE : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TMP_Text>().text = RelayManager.Instance.joinCode;
    }
}