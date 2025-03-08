using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class BossEnemy : EnemyController
{
    [Header("Boss Enemy")]
    public float bossMoveSpeed = 1;
    private float initHealth = 0;
    private readonly float initHealthAnnounceCriterion = 50;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        gm.player.GetComponent<PlayerInterfaceController>().SetBossObject(gameObject);
    }

    protected virtual void Update()
    {

        if (player == null)
        {
            return;
        }
        if (gm.isPlaying)
        {
            Vector2 targetDir = (player.transform.position - transform.position).normalized;
            var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            transform.position =
                Vector2.MoveTowards(transform.position, player.transform.position, bossMoveSpeed * Time.deltaTime);
        }
        // Move toward player

    }

    public override void Damage(float dmgAmount)
    {
        health -= dmgAmount;
        if (health <= 0)
        {
            Destroy(gameObject);

            //interaction with game manager
            GameManager.instance.bossSpawnManager.enableBossSpawn();
            GameManager.instance.IncreaseScore(enemyScore);

            GameManager.instance.AddPhase();

            UIManager.instance.Upgrade();
            UIManager.instance.ActivateAnnoucer(13);

            gm.player.GetComponent<PlayerInterfaceController>().SetBossNotifier(false);
        }
        else if ((health <= 20) && (initHealth > initHealthAnnounceCriterion))
        {
            UIManager.instance.ActivateAnnoucer(12);
        }

        if (!GameManager.instance.isPlaying)
        {
            return;
        }

        if (!player)
        {
            return;
        }

        // Move toward player
        Vector2 targetDir = (player.transform.position - transform.position).normalized;
        var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        transform.position =
            Vector2.MoveTowards(transform.position, player.transform.position, bossMoveSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            gm.DamagePlayer(collisionDamage);
            //StartCoroutine("WaitCo");            
        }
    }

    internal void SetLife(float healthInput)
    {
        health = healthInput;
        initHealth = healthInput;
        Debug.Log("Boss life: " + health);
    }
}
