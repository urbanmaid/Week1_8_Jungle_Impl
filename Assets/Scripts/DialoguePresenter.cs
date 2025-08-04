using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI dialogueDispText;
    private int diagContentIndex;
    private int diagContentLength;
    private string[] diagContentKeys;

    [Header("Content")]
    [SerializeField] TextAsset dialogueContent;
    [SerializeField] GameObject[] dialogueContentSprite;
    [SerializeField] UnityEvent actionNext;
    private GameObject dialogueContentSpriteNow;

    // Avoid Edit
    public void ShowDialogue()
    {
        startPanel.SetActive(false);
        dialoguePanel.SetActive(true);

        diagContentIndex = 0;
        if (dialogueContent != null)
        {
            // 1. \r과 \n을 기준으로 줄을 나누고, 빈 줄은 자동으로 제거합니다.
            // 2. 각 줄의 앞/뒤 공백과 보이지 않는 문자를 Trim()으로 제거합니다.
            diagContentKeys = dialogueContent.text
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .ToArray();
        }
        else
        {
            Debug.LogError("DialogueContent TextAsset이 할당되지 않았습니다!");
            diagContentKeys = new string[0];
        }

        diagContentLength = diagContentKeys.Length;

        Debug.Log("Length of dialogue: " + diagContentLength);

        // Set text and sprite
        StartCoroutine(UpdateDialogue());
        RefreshDialogueSprite();

        UIManager.instance.PlayUIConfirm();
    }

    public void DialogueControlPrev()
    {
        if (diagContentIndex > 0)
        {
            diagContentIndex -= 1;
            RefreshDialogueSprite();
            StartCoroutine(UpdateDialogue());
        }

        UIManager.instance.PlayUISelect();
    }

    public void DialogueControlNext()
    {
        if (diagContentIndex < diagContentLength - 1)
        {
            diagContentIndex += 1;
            RefreshDialogueSprite();
            StartCoroutine(UpdateDialogue());

            UIManager.instance.PlayUISelect();
        }
        else // End of Page
        {
            actionNext.Invoke();
            CloseDialogue();

            //UIManager.instance.PlayUIConfirm();
        }
    }

    // This Method applies translation
    IEnumerator UpdateDialogue()
    {
        string entryKey = diagContentKeys[diagContentIndex];

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
            dialogueDispText.text = handle.Result;
        }
        else // If there are no value
        {
            dialogueDispText.text = diagContentKeys[diagContentIndex];
        }
    }

    public void RefreshDialogueSprite()
    {
        Destroy(dialogueContentSpriteNow);
        if (dialogueContentSprite[diagContentIndex] != null)
        {
            dialogueContentSpriteNow = Instantiate(dialogueContentSprite[diagContentIndex], transform.position, Quaternion.identity);
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
