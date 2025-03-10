using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private GameObject player;
    [SerializeField] GameObject projectileParticle;
    private float distance = 26f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) > distance)
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") 
        || collision.CompareTag("Player Projectile")
        || collision.CompareTag("Missile Projectile")
        || collision.CompareTag("Enemy Projectile")
        )
        {
            Destroy(collision.gameObject);
        }
        else if(collision.CompareTag("Player"))
        {
            GameManager.instance.DamagePlayer(4f);
        }
                
        if(projectileParticle)
        { 
            Instantiate(projectileParticle, collision.transform.position, Quaternion.identity); 
        }
    }
}
