using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FPS : MonoBehaviour
{
    private TMP_Text fpsText;
    private void Start()
    {
        fpsText = GetComponent<TMP_Text>();
    }
    void Update()
    {
        float fps = 1.0f / Time.deltaTime;
        fpsText.text = $"{Mathf.Ceil(fps)} FPS";
    }
}
