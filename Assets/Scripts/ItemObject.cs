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
    [SerializeField] int missileAmount = 8;

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
            itemSpawnConditionManager.SetIsSpawned(false); // Reset status of isSpawned
            Destroy(gameObject);

            // Disable item notifier due to item has been picked up
            playerInterfaceController.SetItemNotifier(false);
        }
    }

    public void ItemEffect()
    {
        switch (itemCode)
        {
            case 0:
                //Debug.Log("Health has been increased.");
                gm.DamagePlayer(-1 * healMount);
                UIManager.instance.ActivateAnnoucer(2);
                break;
            case 1:
                gm.missileAmount += this.missileAmount;
                //Debug.Log("Missile amount has been increased into " + missileAmount);
                UIManager.instance.UpdateMissile();
                UIManager.instance.ActivateAnnoucer(3);
                break;
            case 2:
                //Debug.Log("Signal jammmer.");
                CallRequestCooltimeOnAllSpawners();
                UIManager.instance.ActivateAnnoucer(4);
                break;
            case 10:
                Debug.Log("Achived 1 Rush Item");
                break;
            case 11:
                Debug.Log("Achived 1 Blackhole Item");
                break;
            case 12:
                Debug.Log("Achived 1 Shield Item");
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

    public void UseSkill()
    {
        // Skill usage
        int skillMode = Random.Range(0, 3);
       
        // Use skill based on skillMode
        switch (skillMode)
        {
            case 0:
                Debug.Log("Gravity Shot has been used.");
                playerController.FXGravityShot();
                break;
            case 1:
                // Shield
                Debug.Log("Shield has been used.");
                playerController.FXShield();
                break;
            case 2:
                // Charge
                Debug.Log("Charge has been used.");
                playerController.FXRush();
                break;
            default:
                Debug.LogError("Invalid skill mode detected, no effect applied.");
                break;
        }
    }
}
