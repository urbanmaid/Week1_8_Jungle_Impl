using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] float projectileSpeed;
    //private SpriteRenderer rend;
    [SerializeField] float damage;
    
    [SerializeField] private GameObject projectileParticle;

    protected virtual void Start()
    {
        if(gameObject.CompareTag("Missile Projectile")){
            damage = damage + GameObject.Find("Player").GetComponent<PlayerController>().missilePower * 2;
            Debug.Log("Missile Damage: " + damage);
        }

        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.up * projectileSpeed;
        
        Invoke(nameof(OnBecameInvisible), 6f);
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1
        /*
        if (gameObject.CompareTag("Player Projectile") && collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            enemy.Damage(damage);
            SpawnParticles();
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Enemy Projectile") && collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.DamagePlayer(damage);
            Destroy(gameObject);
        }
        else if (gameObject.CompareTag("Missile Projectile") && collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            SpawnParticles();
            enemy.Damage(damage);
        }
        */

        // 2
        if(collision.gameObject.CompareTag("Enemy")){
            if(gameObject.CompareTag("Player Projectile")){
                Destroy(gameObject);
            }
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            SpawnParticles();
            enemy.Damage(damage);
        }
        else if(collision.gameObject.CompareTag("Player")){
            if(gameObject.CompareTag("Enemy Projectile")){
                GameManager.instance.DamagePlayer(damage);
                Destroy(gameObject);
            }
        }
    }

    private void SpawnParticles()
    {
        var particles = Instantiate(projectileParticle, transform.position, Quaternion.identity);
    }
}
