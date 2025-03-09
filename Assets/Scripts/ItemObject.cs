using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int itemCode;

    //private float rotSpeed = 8f;
    private GameManager gm;
    private ItemSpawnConditionManager itemSpawnConditionManager;
    private PlayerController playerController;
    private PlayerInterfaceController playerInterfaceController;

    [SerializeField] float healMount = 10f;
    [SerializeField] int buffAmount = 4;

    private void Start()
    {
        gm = GameManager.instance;
        itemSpawnConditionManager = ItemSpawnConditionManager.instance;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerInterfaceController = GameObject.Find("Player").GetComponent<PlayerInterfaceController>();

        playerInterfaceController.SetItemObject(gameObject);
        UIManager.instance.ActivateAnnoucer(1);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Item has been picked up.");
            ItemEffect();
            
            if(itemSpawnConditionManager){
                itemSpawnConditionManager.SetIsSpawned(false); // Reset status of isSpawned
            }
            Destroy(gameObject);

            // Disable item notifier due to item has been picked up
            playerInterfaceController.SetItemNotifier(false);
        }
    }

    public void ItemEffect()
    {
        switch (itemCode)
        {
            case 0: // Activate when touched
                //Debug.Log("Health has been increased.");
                gm.DamagePlayer(-1 * healMount);
                UIManager.instance.ActivateAnnoucer(buffAmount);
                break;
            case 1:
                gm.missileAmount += this.buffAmount;
                //Debug.Log("Missile amount has been increased into " + missileAmount);
                UIManager.instance.UpdateMissile();
                UIManager.instance.ActivateAnnoucer(buffAmount);
                break;
            case 2:
                //Debug.Log("Signal jammmer.");
                CallRequestCooltimeOnAllSpawners();
                UIManager.instance.ActivateAnnoucer(buffAmount);
                break;
            case 10: // Not activated by touching but stored
                Debug.Log("Achived 1 Rush Item");
                gm.AddSkillRush(2);
                break;
            case 11:
                Debug.Log("Achived 1 Shield Item");
                gm.AddSkillShield(2);
                break;
            case 12:
                Debug.Log("Achived 1 GravityShot/Blackhole Item");
                gm.AddSkillGravityShot(2);
                break;
            default:
                Debug.LogError("Invalid item code detected, no effect applied.");
                break;
        }
    }

    private void CallRequestCooltimeOnAllSpawners()
    {
        foreach (RandomEnemySpawner spawner in RandomEnemySpawner.instances)
        {
            spawner.RequestCooltime();
        }
    }
}
