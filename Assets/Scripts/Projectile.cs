using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] float projectileSpeed;
    //private SpriteRenderer rend;
    [SerializeField] float damage;
    [SerializeField] private GameObject rotatedSprite;

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

    void Update()
    {
        if(rotatedSprite){
            rotatedSprite.transform.Rotate(0, 0, 80f * Time.deltaTime);
        }

        if(GameManager.instance.isPlaying){
            rb.linearVelocity = transform.up * projectileSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 2
        if(collision.gameObject.CompareTag("Enemy")){
            if(gameObject.CompareTag("Player Projectile"))
            {
                Destroy(gameObject);
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                enemy.Damage(damage);
            }
            else if(gameObject.CompareTag("Missile Projectile")){
                EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
                enemy.Damage(damage);
            }
            
            if(projectileParticle){ SpawnParticles(); }
        }
        else if(collision.gameObject.CompareTag("Player"))
        {
            if(gameObject.CompareTag("Enemy Projectile")){
                GameManager.instance.DamagePlayer(damage);
                Destroy(gameObject);
            }
        }
    }

    private void SpawnParticles()
    {
        
        Instantiate(projectileParticle, transform.position, Quaternion.identity);
    }
}
