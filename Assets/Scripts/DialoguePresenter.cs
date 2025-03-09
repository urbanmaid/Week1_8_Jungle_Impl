using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialoguePresenter : MonoBehaviour
{
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI dialogueDispText;
    private int diagContentIndex;
    private int diagContentLength;
    private string[] diagContentString;

    [Header("Content")]
    [SerializeField] TextAsset dialogueContent;
    [SerializeField] GameObject[] dialogueContentSprite;
    [SerializeField] UnityEvent actionNext;
    private GameObject dialogueContentSpriteNow;

    public void ShowDialogue()
    {
        startPanel.SetActive(false);
        dialoguePanel.SetActive(true);

        diagContentIndex = 0;
        diagContentString = dialogueContent.text.Split('\n');

        diagContentLength = diagContentString.Length;

        Debug.Log("Length of dialogue: " + diagContentLength);

        // Set text and sprite
        dialogueDispText.text = diagContentString[diagContentIndex];
        RefreshDialogueSprite();
    }

    public void DialogueControlPrev()
    {
        if(diagContentIndex > 0)
        {
            diagContentIndex -= 1;
            RefreshDialogueSprite();
            dialogueDispText.text = diagContentString[diagContentIndex];
        }
    }

    public void DialogueControlNext()
    {
        if(diagContentIndex < diagContentLength - 1)
        {
            diagContentIndex += 1;
            RefreshDialogueSprite();
            dialogueDispText.text = diagContentString[diagContentIndex];
        }
        else
        {
            actionNext.Invoke();
            CloseDialogue();
        }
    }

    public void RefreshDialogueSprite()
    {
        Destroy(dialogueContentSpriteNow);
        if(dialogueContentSprite[diagContentIndex] != null)
        {
            dialogueContentSpriteNow = Instantiate(dialogueContentSprite[diagContentIndex], transform.position, Quaternion.identity);
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
    }
}
