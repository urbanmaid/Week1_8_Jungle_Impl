using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public static List<ItemObject> instances = new List<ItemObject>();
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
        instances.Add(this);

        gm = GameManager.instance;
        itemSpawnConditionManager = ItemSpawnConditionManager.instance;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerInterfaceController = GameObject.Find("Player").GetComponent<PlayerInterfaceController>();

        //playerInterfaceController.SetItemObject(gameObject);
        playerInterfaceController.SetItemObjectInInstances();
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
            instances.Remove(this);
            Destroy(gameObject);

            // Disable item notifier due to item has been picked up
            playerInterfaceController.SetItemObjectInInstances();
            //playerInterfaceController.SetItemNotifier(false);
        }
    }

    public void ItemEffect()
    {
        switch (itemCode)
        {
            case 0: // Activate when touched
                //Debug.Log("Health has been increased.");
                gm.DamagePlayer(-1 * healMount);
                UIManager.instance.ActivateAnnoucer(2 + itemCode);
                break;
            case 1:
                gm.missileAmount += this.buffAmount;
                //Debug.Log("Missile amount has been increased into " + missileAmount);
                UIManager.instance.UpdateMissile();
                UIManager.instance.ActivateAnnoucer(2 + itemCode);
                break;
            case 2:
                //Debug.Log("Signal jammmer.");
                CallRequestCooltimeOnAllSpawners();
                UIManager.instance.ActivateAnnoucer(2 + itemCode);
                break;
            case 10: // Not activated by touching but stored
                //Debug.Log("Achived 1 Rush Item");
                gm.AddSkillRush(buffAmount);
                break;
            case 11:
                //Debug.Log("Achived 1 Shield Item");
                gm.AddSkillShield(buffAmount);
                break;
            case 12:
                //Debug.Log("Achived 1 GravityShot/Blackhole Item");
                gm.AddSkillGravityShot(buffAmount);
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

    /*
    void OnDestroy()
    {
        instances.Remove(this);
    }
    */
}
