using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected GameManager gm;
    protected Rigidbody2D enemyRb;
    protected GameObject player;
    [SerializeField] bool isShootable = true;
    [SerializeField] protected bool isSteerable = true;

    [Header("Enemy Info")]
    public float health;
    public float moveSpeed;
    protected bool inRange;
    protected float angle;
    protected float distance;
    protected Vector2 moveDir;
    protected readonly float availableRange = 26f;

    [SerializeField] protected float collisionDamage;

    [Header("Projectile")]
    [SerializeField] GameObject projectile;
    private float fireRateInterval;
    [SerializeField] float fireRate;
    private bool canDamage;

    [SerializeField] float range;
    [SerializeField] bool isShootingBeforeRange;


    [Header("Score")]
    [SerializeField] protected int enemyScore = 1;

    private ItemSpawnConditionManager itemSpawnConditionManager;

    protected virtual void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        itemSpawnConditionManager = ItemSpawnConditionManager.instance;
        if(itemSpawnConditionManager == null){
            Debug.LogError("ItemSpawnConditionManager is not found.");
        }
        gm = GameManager.instance;
        canDamage = true;

        // For someone which is not steerable
        Steer();
    }

    void Update()
    {
        // Check if game is playing
        if (gm.isPlaying)
        {
            //Enemy movement and rotation
            if(isSteerable)
            {
                Steer();
            }
            GetDistance();

            // Shoot Control
            if (isShootable)
            {
                if (!inRange) // enemy will move towards player if player is out of range
                {
                    enemyRb.linearVelocity = moveDir.normalized * moveSpeed;
                    if (isShootingBeforeRange)
                    {
                        DoRepeativeShoot();
                    }
                }
                else // enemy will stop moving and shoot player if player is in range
                {
                    enemyRb.linearVelocity = Vector2.zero;
                    DoRepeativeShoot();
                }
            }
            else {
                enemyRb.linearVelocity = moveDir.normalized * moveSpeed;
            }

            // Destroy enemy if it is out of available range
            if(distance > availableRange)
            {
                Destroy(gameObject);
            }

        }
        else
        {
            enemyRb.linearVelocity = Vector2.zero;
        }
    }

    protected void Steer()
    {
        if(player){
            moveDir = player.transform.position - transform.position;
        }
        angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    protected void GetDistance()
    {
        distance = Vector2.Distance(player.transform.position, transform.position);
        inRange = distance < range;
    }

    private void DoRepeativeShoot()
    {
        if (fireRateInterval > 0)
        {
            fireRateInterval -= Time.deltaTime;
        }
        else 
        {
            Shoot();
            fireRateInterval = fireRate;
        }
    }

    public virtual void Damage(float dmgAmount)
    {
        health -= dmgAmount;
        if (health <= 0)
        {
            Destroy(gameObject);
            InstantiateItem(transform.position);
            GameManager.instance.IncreaseScore(enemyScore);
        }
    }

    private void InstantiateItem(Vector3 targetPosition)
    {
        if (itemSpawnConditionManager.IsAbleToSpawnItem())
        {
            // Spawn one instance of item and disable the ability to spawn item
            Instantiate(itemSpawnConditionManager.SpawnItem(), targetPosition, Quaternion.identity);
            itemSpawnConditionManager.SetAbleToSpawnItem(false);
        }
    }

    //shoots enemy projectile
    //used object pooling for projectile spawning and despawning
    private void Shoot()
    {
        Instantiate(projectile, transform.position, transform.rotation);        
    }

   protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && canDamage && !player.GetComponent<PlayerController>().isShielded){
            canDamage = false;
            gm.DamagePlayer(collisionDamage);
            //StartCoroutine("WaitCo");
            Destroy(gameObject);
        }
    }

    IEnumerator WaitCo(){
        yield return new WaitForSeconds(1f);
        canDamage = true;
    }
}

