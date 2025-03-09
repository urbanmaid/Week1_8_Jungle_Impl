using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public RandomEnemySpawner spawnManager;
    [SerializeField] private CameraController cameraController;
    public BossSpawner bossSpawnManager;

    
    [Header("Game Mechanic")]
    public GameObject player;
    public float maxHealth;
    public float curHealth;
    public int curPhase;
    public bool isPlaying;
    public int missileAmount;
    
    public GameObject managers;

    [HideInInspector] public int scoreTotal = 0;
    internal int scoreTotalTarget = 0;
    
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
        //missileAmount = 0;

        // Initialize the game UI with the current value of player
        UIManager.instance.UpdateMissile();
    }

    internal void AddPhase()
    {
        curPhase++;
        Invoke(nameof(NotifyPhase), 6.0f);
    }

    internal void NotifyPhase()
    {
        UIManager.instance.UpdatePhase();

        if(curPhase == 2){
            UIManager.instance.ActivateAnnoucer(11);
        }
        if(curPhase == 3){
            UIManager.instance.ActivateAnnoucer(12);
        }
    }

    public void DamagePlayer(float damage)
    {
        if(damage > 0 && !player.GetComponent<PlayerController>().isShielded){
            cameraController.ShakeCamera();
        }
        curHealth -= damage;
        
        if (curHealth <= 0)
        {
            isPlaying = false;
            player.SetActive(false);
            UIManager.instance.EndGame();
        }
        else if(curHealth < 25){
            UIManager.instance.ActivateAnnoucer(16);
        }
        else if(curHealth > maxHealth)
        {
            Debug.Log("Health Overflow detected, resetting health to max.");
            curHealth = maxHealth;
        }

        UIManager.instance.UpdateHealth();
    }

    public void IncreaseScore(int score)
    {
        scoreTotal += score;
        ItemSpawnConditionManager.instance.AddScore(score);
        UIManager.instance.UpdateScore();
    }

    internal void UpdateScoreTarget(int scoreTarget)
    {
        scoreTotalTarget = scoreTarget + scoreTotal;
        UIManager.instance.UpdateScore();
    }

    internal void NotifyMissileUsed()
    {
        missileAmount--;
        UIManager.instance.UpdateMissile();
    }
}
