using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class StatusAnnouncer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusAnnouncerUI;
    [SerializeField] private TextAsset statusSourceText;
    private string[] statusKeys;
    private float announceDuration = 6f;
    private Coroutine activeAnnounceCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        statusAnnouncerUI.text = "";
        statusKeys = statusSourceText.text.Split('\n');
    }

    // Update is called once per frame
    public void ActivateAnnoucer(int code)
    {
        // If there are activated coroutine
        if (activeAnnounceCoroutine != null)
        {
            StopCoroutine(activeAnnounceCoroutine);
        }

        activeAnnounceCoroutine = StartCoroutine(ShowAnnouncementRoutine(code));
    }

    private IEnumerator ShowAnnouncementRoutine(int code)
    {
        if (code < 0 || code >= statusKeys.Length)
        {
            Debug.LogError($"잘못된 상태 코드({code})가 전달되었습니다.");
            yield break;
        }

        string entryKey = statusKeys[code];

        var announcementString = new LocalizedString
        {
            TableReference = LocalizationSettings.StringDatabase.DefaultTable,
            TableEntryReference = entryKey
        };

        // Wait until result has released
        var handle = announcementString.GetLocalizedStringAsync();
        yield return handle;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            statusAnnouncerUI.text = handle.Result;
        }
        else // If there are no value
        {
            statusAnnouncerUI.text = entryKey;
        }
        
        yield return new WaitForSeconds(announceDuration);

        statusAnnouncerUI.text = "";
        activeAnnounceCoroutine = null;
    }
}