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
    [SerializeField] int scoreInterval = 30;
    private int scoreIntervalInit = 0;
    [SerializeField] int scoreIntervalAddition = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        scoreCurrent = 0;
        scoreIntervalInit = scoreInterval;
    }

    internal void AddScore(int score){
        scoreCurrent += score;
        if(scoreCurrent >= scoreInterval){
            itemSpawnedAmount++;
            scoreCurrent = 0;
            scoreInterval += scoreIntervalAddition * itemSpawnedAmount;

            SetAbleToSpawnItem(true);
            Debug.Log("Item is able to spawn.");
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
        isSpawned = true;
        return itemList[Random.Range(0, itemList.Count)];
    }

}
