using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Manager Assists")]
    public static GameManager instance;
    public RandomEnemySpawner spawnManager;
    [SerializeField] private CameraController cameraController;
    public BossSpawner bossSpawnManager;

    
    [Header("Player Stat")]
    public GameObject player;
    public float maxHealth;
    public float curHealth;
    public int curPhase;
    public bool isPlaying;
    public bool isScorable;
    public bool isDamagable;
    public int missileAmount;

    public int skillRush = 1;
    public int skillShield = 1;
    public int skillGravityShot = 1;
    
    public GameObject managers;

    [HideInInspector] public int scoreTotal = 0;
    internal int scoreTotalTarget = 0;

    [Header("Epilogue")]
    [SerializeField] EpiloguePresenter epiloguePresenter;
    public bool isInEpilogue = false;
    [SerializeField] int epilogueTargetPhase = 10;
    internal Transform mothershipTransform;
    private float gameCompleteDist = 4f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
        curPhase = 0;
        isScorable = true;
        isDamagable = true;
        //missileAmount = 0;

        // Initialize the game UI with the current value of player
        UIManager.instance.UpdateMissile();
        UIManager.instance.UpdateRush();
        UIManager.instance.UpdateShield();
        UIManager.instance.UpdateGravityShot();
    }

    internal void AddPhase()
    {
        curPhase++;
        Invoke(nameof(NotifyPhase), 6.0f);
    }

    internal void NotifyPhase()
    {
        UIManager.instance.UpdatePhase();

        if(curPhase == 2)
        {
            UIManager.instance.ActivateAnnoucer(11);
        }
        else if(curPhase == 3)
        {
            UIManager.instance.ActivateAnnoucer(12);
        }
        else if(curPhase == epilogueTargetPhase)
        {
            isInEpilogue = true;
            epiloguePresenter.gameObject.SetActive(true);
        }
    }

    public void DamagePlayer(float damage)
    {
        if(isDamagable){
            if(damage > 0 && !player.GetComponent<PlayerController>().isShielded){
                cameraController.ShakeCamera();
            }
            curHealth -= damage;
            curHealth = (int)curHealth;
        }
        isDamagable = false;
        StartCoroutine(ResetDamagable());
        
        // Action when health is 0, or dangerous, or healing
        if (curHealth <= 0)
        {
            isPlaying = false;
            player.SetActive(false);
            StartCoroutine(UIManager.instance.EndGame());
        }
        else if(curHealth < 25){
            UIManager.instance.ActivateAnnoucer(16);
        }
        else if(curHealth > maxHealth)
        {
            //Debug.Log("Health Overflow detected, resetting health to max.");
            curHealth = maxHealth;
        }

        UIManager.instance.UpdateHealth();
    }

    internal IEnumerator ResetDamagable()
    {
        yield return new WaitForSeconds(0.5f);
        isDamagable = true;
    }

    // Skill Management
    internal void NotifyMissileUsed()
    {
        missileAmount--;
        UIManager.instance.UpdateMissile();
    }

    internal void AddSkillRush(int count)
    {
        skillRush += count;
        UIManager.instance.UpdateRush();
    }

    internal void AddSkillShield(int count)
    {
        skillShield += count;
        UIManager.instance.UpdateShield();
    }

    internal void AddSkillGravityShot(int count)
    {
        skillGravityShot += count;
        UIManager.instance.UpdateGravityShot();
    }

    internal void UseSkillRush()
    {
        if(skillRush > 0){
            skillRush--;
            UIManager.instance.UpdateRush();
        }
    }

    internal void UseSkillShield()
    {
        if(skillShield > 0){
            skillShield--;
            UIManager.instance.UpdateShield();
        }
    }

    internal void UseSkillGravityShot()
    {
        if(skillGravityShot > 0){
            skillGravityShot--;
            UIManager.instance.UpdateGravityShot();
        }
    }

    internal void ToggleShakeCamera(bool value)
    {
        cameraController.ToggleShake(value);
    }

    public void IncreaseScore(int score)
    {
        if(isScorable){
            scoreTotal += score;
            ItemSpawnConditionManager.instance.AddScore(score);
            UIManager.instance.UpdateScore();
        }
    }

    internal void UpdateScoreTarget(int scoreTarget)
    {
        scoreTotalTarget = scoreTarget + scoreTotal;
        UIManager.instance.UpdateScore();
    }

    internal void SetScoreable(bool value)
    {
        isScorable = value;
        Debug.Log(isScorable);
    }

    internal void NotifyMothership(GameObject mothership) // If mothership has been spawned this is called
    {
        mothershipTransform = mothership.transform;
        UIManager.instance.SetMothershipDist(true);
        StartCoroutine(UpdateMothershipDistance());
    }

    IEnumerator UpdateMothershipDistance() // Called in 1 sec and shows distance
    {
        while(true){
            yield return new WaitForSeconds(1f);
            float dist = Vector3.Distance(player.transform.position, mothershipTransform.position);
            //Debug.Log(dist);
            UIManager.instance.UpdateMothershipDist((int) dist);
            if(dist < gameCompleteDist)
            {
                NotifyGameComplete();
                break;
            }
        }
    }

    private void NotifyGameComplete()
    {
        isPlaying = false;
        Debug.Log("Game Complete!");
        StartCoroutine(UIManager.instance.SetCompleteScreen(true));
    }
}
