using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEditor.Localization.Plugins.XLIFF.V12;

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
    [SerializeField] TextMeshProUGUI scoreTextComplete;
    [SerializeField] TextMeshProUGUI upgradeConfirmText;
    [SerializeField] StatusAnnouncer statusAnnouncer;

    [Header("Menu Configuration")]
    [SerializeField] GameObject startPanel;
    [SerializeField] GameObject upgradePanel;
    [SerializeField] GameObject endPanel;
    [SerializeField] GameObject completePanel; 
    [SerializeField] GameObject gameInfo; 

    [Header("Status")]
    [SerializeField] int missileLvl;
    [SerializeField] int attackLvl;
    [SerializeField] int skillLvl;
    [SerializeField] TextMeshProUGUI mothershipDistText;
    [SerializeField] GameObject mothershipDist;

    private int upgradeCode;
    private string upgradeStringInit;
    private LocalizedString upgradeStringEntry;
    private float time;
    private int min;
    private int sec;

    [Header("Score Evaluation")]
    //[SerializeField] float scoreInterval = 1f;
    [SerializeField] int scoreCutAmateur = 250;
    [SerializeField] int scoreCutIntermediate = 500;
    [SerializeField] int scoreCutPro = 1000;
    [SerializeField] TextMeshProUGUI gameoverEvalText;
    [SerializeField] LocalizedString quoteRookie;
    [SerializeField] LocalizedString quoteAmateur;
    [SerializeField] LocalizedString quoteIntermediate;
    [SerializeField] LocalizedString quotePro;
    
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

        // If localizer is set, get its text from localizer
        LocalizeStringEvent ls = upgradeConfirmText.GetComponent<LocalizeStringEvent>();
        if (ls)
        {
            upgradeStringEntry = ls.StringReference;
        }
        else
        {
            upgradeStringInit = upgradeConfirmText.text;
        }
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
        StartCoroutine(SetCombinedTextRoutine(text));
    }
    
    private IEnumerator SetCombinedTextRoutine(string itemEntryKey)
    {
        // 이 로직은 LocalizeStringEvent를 직접 사용하지 않고 수동으로 텍스트를 설정합니다.
        // 따라서 컴포넌트가 활성화되어 있다면 잠시 비활성화하여 충돌을 방지하는 것이 안전합니다.
        var localizeEvent = upgradeConfirmText.GetComponent<LocalizeStringEvent>();
        if (localizeEvent != null)
        {
            localizeEvent.enabled = false;
        }

        var baseStringHandle = upgradeStringEntry.GetLocalizedStringAsync();
        yield return baseStringHandle;

        if (baseStringHandle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            yield break;
        }
        string baseText = baseStringHandle.Result;

        var itemString = new LocalizedString
        {
            TableReference = LocalizationSettings.StringDatabase.DefaultTable,
            TableEntryReference = itemEntryKey
        };

        var itemStringHandle = itemString.GetLocalizedStringAsync();
        yield return itemStringHandle;

        if (itemStringHandle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"'{itemEntryKey}' 키를 로드하는 데 실패했습니다. 키가 테이블에 존재하는지 확인하세요.");
            upgradeConfirmText.text = $"{baseText}: {itemEntryKey}";
            yield break;
        }
        string itemText = itemStringHandle.Result;

        upgradeConfirmText.text = $"{baseText}: {itemText}";
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
        GameManager.instance.isDamagable = false;
        StartCoroutine(GameManager.instance.ResetDamagable());

        statusAnnouncer.ActivateAnnoucer(8 + upgradeCode);
        GameManager.instance.AddPhase();
    }

    public IEnumerator EndGame()
    {
        gameInfo.SetActive(false);
        gm.isPlaying = false;

        yield return new WaitForSeconds(1.5f);
        EvaulateScore();
        endPanel.SetActive(true);
        scoreTextGameOver.text = "" + gm.scoreTotal;
    }

    private void EvaulateScore()
    {
        LocalizedString evaluationString;

        if (gm.scoreTotal < scoreCutAmateur)
        {
            evaluationString = quoteRookie;
        }
        else if (gm.scoreTotal < scoreCutIntermediate)
        {
            evaluationString = quoteAmateur;
        }
        else if (gm.scoreTotal < scoreCutPro)
        {
            evaluationString = quoteIntermediate;
        }
        else
        {
            evaluationString = quotePro;
        }

        // Set evaluation text
        gameoverEvalText.text = evaluationString.GetLocalizedStringAsync().Result;
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

    internal void SetMothershipDist(bool value)
    {
        mothershipDist.SetActive(value);
    }

    internal void UpdateMothershipDist(float value)
    {
        mothershipDistText.text = value + "";
    }

    internal IEnumerator SetCompleteScreen(bool value)
    {
        yield return new WaitForSeconds(1.5f);
        completePanel.SetActive(value);
        scoreTextComplete.text = "" + gm.scoreTotal;
    }

    public void ContinueAfterComplete()
    {
        StartCoroutine(ContinueAfterCompleteCo());
    }

    private IEnumerator ContinueAfterCompleteCo()
    {
        completePanel.SetActive(false);
        mothershipDist.SetActive(false);

        Destroy(gm.mothershipTransform.gameObject);

        yield return new WaitForSeconds(1f);
        gm.isPlaying = true;
    }
}
