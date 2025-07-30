using UnityEngine;

public class EpiloguePresenter : MonoBehaviour
{
    public static EpiloguePresenter instance;

    [SerializeField] GameObject epilogueTarget;
    [SerializeField] float distFromPlayer = 200f;
    private GameObject player;

    [SerializeField] GameObject[] disabledWhenEpilogue;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        player = GameObject.Find("Player");
        PrepareEpilogue();
    }

    void PrepareEpilogue()
    {
        SpawnEpilogueTarget();
        setDisabledOff();
    }

    void SpawnEpilogueTarget()
    {
        Instantiate(epilogueTarget, SetSpawnLocation(), Quaternion.Euler(0, 0, Random.Range(0f, 360f)));
        UIManager.instance.ActivateAnnoucer(20);
    }

    void setDisabledOff()
    {
        for(int i = 0; i < disabledWhenEpilogue.Length; i++)
        {
            disabledWhenEpilogue[i].SetActive(false);
        }
    }

    public void SetDistOfMothership(float dist)
    {
        distFromPlayer = dist;
    }

    private Vector3 SetSpawnLocation()
    {
        // Set new area of spawnning bullet box
        Vector3 playerPosition = player.transform.position;
        Vector2 randomDirection = Random.insideUnitCircle.normalized * distFromPlayer;
        return playerPosition + (Vector3)randomDirection;
    }
}
