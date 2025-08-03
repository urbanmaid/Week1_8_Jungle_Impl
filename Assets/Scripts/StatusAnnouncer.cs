using System.Collections;
using System.Linq; // LINQ를 사용하기 위해 추가
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

        // --- ★★★ 핵심 수정 부분 ★★★ ---
        if (statusSourceText != null)
        {
            // 1. \r과 \n을 기준으로 줄을 나누고, 빈 줄은 자동으로 제거합니다.
            // 2. 각 줄의 앞/뒤 공백과 보이지 않는 문자를 Trim()으로 완전히 제거합니다.
            statusKeys = statusSourceText.text
                .Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .ToArray();
        }
        else
        {
            Debug.LogError("StatusSourceText 에셋이 할당되지 않았습니다!");
            statusKeys = new string[0]; // Null 참조 오류를 방지하기 위해 빈 배열로 초기화
        }
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
            Debug.LogError($"키 '{entryKey}'의 번역을 찾지 못했습니다. 키가 정확한지, String Table에 존재하는지 확인하세요.");
            statusAnnouncerUI.text = entryKey; // 디버깅을 위해 키를 그대로 표시
        }
        
        yield return new WaitForSeconds(announceDuration);

        statusAnnouncerUI.text = "";
        activeAnnounceCoroutine = null;
    }
}