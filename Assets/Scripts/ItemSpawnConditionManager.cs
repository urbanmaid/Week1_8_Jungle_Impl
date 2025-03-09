using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnConditionManager : MonoBehaviour
{
    public static ItemSpawnConditionManager instance;

    public List<GameObject> itemList;
    [SerializeField] bool isAbleToSpawn = false;
    [SerializeField] bool isSpawned = false;

    [SerializeField] int itemSpawnedAmount = 0;

    [Header("Timing")]
    [SerializeField] int scoreCurrent = 0;
    [SerializeField] int scoreTarget = 30;
    private int scoreIntervalInit = 0;
    [SerializeField] int scoreIntervalAddition = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        scoreCurrent = 0;
        scoreIntervalInit = scoreTarget;
        NotifyNextscoreTarget();
    }

    internal void AddScore(int score){
        scoreCurrent += score;

        while (scoreCurrent >= scoreTarget)
        {
            itemSpawnedAmount++;
            int scoreOver = scoreCurrent - scoreTarget;
            scoreCurrent = scoreOver;
            scoreTarget += scoreIntervalAddition * 1;

            NotifyNextscoreTargetWithGrant(scoreOver);

            SetAbleToSpawnItem(true);
            Debug.Log("Next target score: " + scoreTarget);
        }
    }

    public bool IsAbleToSpawnItem(){
        return isAbleToSpawn;
    }

    public void SetAbleToSpawnItem(bool value){
        isAbleToSpawn = value;
    }

    public void SetIsSpawned(bool value){
        isSpawned = value;
    }

    public GameObject SpawnItem(){
        //NotifyNextscoreTarget();

        isSpawned = true;
        return itemList[Random.Range(0, itemList.Count)];
    }

    void NotifyNextscoreTarget(){
        GameManager.instance.UpdateScoreTarget(scoreTarget);
    }

    void NotifyNextscoreTargetWithGrant(int scoreOver)
    {
        GameManager.instance.UpdateScoreTarget(scoreTarget - scoreOver);
    }

}
