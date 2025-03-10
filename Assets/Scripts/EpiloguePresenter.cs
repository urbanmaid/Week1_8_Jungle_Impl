using System.Linq;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class EpiloguePresenter : MonoBehaviour
{
    [SerializeField] GameObject epilogueTarget;
    [SerializeField] float distFromPlayer = 200f;
    private GameObject player;

    [SerializeField] GameObject[] disabledWhenEpilogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        PrepareEpilogue();
        //Invoke(nameof(PrepareEpilogue), 10f);
    }

    void PrepareEpilogue()
    {
        SpawnEpilogueTarget();
        setDisabledOff();
    }

    void SpawnEpilogueTarget()
    {
        Instantiate(epilogueTarget, SetSpawnLocation(), Quaternion.identity);
        UIManager.instance.ActivateAnnoucer(20);
    }

    void setDisabledOff()
    {
        for(int i = 0; i < disabledWhenEpilogue.Length; i++)
        {
            disabledWhenEpilogue[i].SetActive(false);
        }
    }

    private Vector3 SetSpawnLocation()
    {
        // Set new area of spawnning bullet box
        Vector3 playerPosition = player.transform.position;
        Vector2 randomDirection = Random.insideUnitCircle.normalized * distFromPlayer;
        return playerPosition + (Vector3)randomDirection;
    }
}
