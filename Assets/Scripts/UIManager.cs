using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Rendering;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Gameplay")]
    public static UIManager instance;
    private GameManager gm;
    [SerializeField] TextMeshProUGUI healthText, missileText, scoreText, timerText, phaseText;
    [SerializeField] TextMeshProUGUI moveLvlText, attackLvlText, skillLvlText;
    [SerializeField] TextMeshProUGUI skillRushText, skillShieldText, skillGravityShotText;
    [SerializeField] Slider healthSlider;
    [SerializeField] TextMeshProUGUI scoreTextGameOver;
    [SerializeField] TextMeshProUGUI upgradeConfirmText;
    [SerializeField] StatusAnnouncer statusAnnouncer;

    [Header("Menu Configuration")]
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject upgradePanel;
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject gameInfo; 
    
    [Header("Instruction")]
    [SerializeField] TextMeshProUGUI instructionText;
    [SerializeField] TextAsset instructionContent;
    [SerializeField] GameObject[] instructionContentSprite;

    private int instructionContentIndex = 0;
    private int instructionContentLength = 0;
    private string[] instructionContentString;
    private GameObject instructionContentSpriteNow;

    [Header("Status")]
    [SerializeField] int missileLvl;
    [SerializeField] int attackLvl;
    [SerializeField] int skillLvl;
    private int upgradeCode;
    private string upgradeStringInit;
    private float time;
    private int min;
    private int sec;

    [Header("Score Evaluation")]
    //[SerializeField] float scoreInterval = 1f;
    [SerializeField] int scoreCutAmateur = 250;
    [SerializeField] int scoreCutIntermediate = 500;
    [SerializeField] int scoreCutPro = 1000;
    [SerializeField] TextMeshProUGUI gameoverEvalText;
    [SerializeField] string quoteRookie;
    [SerializeField] string quoteAmateur;
    [SerializeField] string quoteIntermediate;
    [SerializeField] string quotePro;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    void Start()
    {
        gm = GameManager.instance;
        timerText.text = "00:00";
        time = 0;
        min = 0;
        sec = 0;

        upgradeStringInit = upgradeConfirmText.text;
    }

    void Update()
    {
        if (gm.isPlaying)
        {
            time += Time.deltaTime;
            UpdateTimer();
        }
    }

    // Update is called once per frame
    public void UpdateScore()
    {
        scoreText.text = gm.scoreTotal + " / " + gm.scoreTotalTarget;
    }
    public void UpdateHealth()
    {
        healthSlider.value = gm.curHealth;
        healthText.text = gm.curHealth.ToString();
    }
    public void UpdateMissile()
    {
        missileText.text = "" + gm.missileAmount;
    }

    public void UpdatePhase()
    {
        phaseText.text = "" + gm.curPhase;
    }

    public void UpdateRush()
    {
        skillRushText.text = "" + gm.skillRush;
    }
    public void UpdateShield()
    {
        skillShieldText.text = "" + gm.skillShield;
    }
    public void UpdateGravityShot()
    {
        skillGravityShotText.text = "" + gm.skillGravityShot;
    }

    void UpdateTimer()
    {
        min = (int)(time / 60f);
        sec = (int)Mathf.Ceil(time % 60f) - 1;
        timerText.text = min + ":" + sec.ToString("D2");
    }

    internal void ActivateAnnoucer(int code)
    {
        statusAnnouncer.ActivateAnnoucer(code);
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        //instructionPanel.SetActive(false);

        gm.isPlaying = true;
        gm.player.SetActive(true);
        gm.managers.SetActive(true);
        gameInfo.SetActive(true);
        Invoke(nameof(StartAnnouce), 2f);
    }

    private void StartAnnouce()
    {
        statusAnnouncer.ActivateAnnoucer(0);
    }

    public void Upgrade()
    {
        Invoke(nameof(ShowUpgradePanel), 2.25f);
    }

    public void ShowUpgradePanel()
    {
        if (gm.isPlaying)
        {
            gm.isPlaying = false;
            upgradePanel.SetActive(true);
            gameInfo.SetActive(false);
        }
    }

    public void SetUpgradeCode(int code)
    {
        upgradeCode = code;
    }

    public void SetUpgradeCodeText(string text)
    {
        upgradeConfirmText.text = text + upgradeStringInit;
    }

    public void ConfirmUpgrade()
    {
        switch (upgradeCode)
        {
            case 0:
                //upgrade missile spec
                gm.player.GetComponent<PlayerController>().missilePower++;
                missileLvl += 1;
                moveLvlText.text = "Lv. " + missileLvl;
                break;
            case 1:
                //upgrade attack speed
                gm.player.GetComponent<PlayerController>().fireRate *= 0.8f;
                attackLvl += 1;
                attackLvlText.text = "Lv. " + attackLvl;
                break;
            case 2:
                //upgrade skill power
                gm.player.GetComponent<PlayerController>().skillPower++;
                skillLvl += 1;
                skillLvlText.text = "Lv. " + skillLvl;
                break;
            default:
                Debug.Log("Wrong Upgrade Code");
                break;
        }
        gm.isPlaying = true;

        gameInfo.SetActive(true);
        upgradePanel.SetActive(false);

        statusAnnouncer.ActivateAnnoucer(8+upgradeCode);
        GameManager.instance.AddPhase();
    }

    public void EndGame()
    {
        gameInfo.SetActive(false);
        gm.isPlaying = false;
        EvaulateScore();
        endPanel.SetActive(true);
        scoreTextGameOver.text = "" + gm.scoreTotal;
    }

    private void EvaulateScore()
    {
        if (gm.scoreTotal < scoreCutAmateur)
        {
            gameoverEvalText.text = quoteRookie;
        }
        else if (gm.scoreTotal < scoreCutIntermediate)
        {
            gameoverEvalText.text = quoteAmateur;
        }
        else if (gm.scoreTotal < scoreCutPro)
        {
            gameoverEvalText.text = quoteIntermediate;
        }
        else
        {
            gameoverEvalText.text = quotePro;
        }
    }

    public void Restart()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    /*
    public void ShowInstruction()
    {
        startPanel.SetActive(false);
        instructionPanel.SetActive(true);

        instructionContentIndex = 0;
        instructionContentString = instructionContent.text.Split('\n');

        instructionContentLength = instructionContentString.Length;

        Debug.Log("Length of dialogue: " + instructionContentLength);

        // Set text and sprite
        instructionText.text = instructionContentString[instructionContentIndex];
        RefreshInstructionSprite();
    }

    public void InstructionControllPrev()
    {
        if(instructionContentIndex > 0)
        {
            instructionContentIndex -= 1;
            RefreshInstructionSprite();
            instructionText.text = instructionContentString[instructionContentIndex];
        }
    }

    public void InstructionControllNext()
    {
        if(instructionContentIndex < instructionContentLength - 1)
        {
            instructionContentIndex += 1;
            RefreshInstructionSprite();
            instructionText.text = instructionContentString[instructionContentIndex];
        }
        else
        {
            StartGame();
        }
    }

    public void RefreshInstructionSprite()
    {
        Destroy(instructionContentSpriteNow);
        if(instructionContentSprite[instructionContentIndex] != null)
        {
            instructionContentSpriteNow = Instantiate(instructionContentSprite[instructionContentIndex], transform.position, Quaternion.identity);
        }
    }

    public void CloseInstruction()
    {
        instructionPanel.SetActive(false);
        startPanel.SetActive(true);
    }
    */
}
