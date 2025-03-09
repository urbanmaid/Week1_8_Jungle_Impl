using TMPro;
using UnityEngine;

public class StatusAnnouncer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusAnnouncerUI;
    [SerializeField] private TextAsset statusSourceText;
    private string[] statusText;
    private float annouceDuration = 6f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusAnnouncerUI.text = "";
        statusText = statusSourceText.text.Split('\n');
    }

    // Update is called once per frame
    public void ActivateAnnoucer(int code){
        if(statusText != null){
            statusAnnouncerUI.text = statusText[code];
        }
        Invoke(nameof(ResetAnnoucer), annouceDuration);
    }

    void ResetAnnoucer()
    {
        statusAnnouncerUI.text = "";
    }
}
