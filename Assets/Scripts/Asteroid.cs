using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private GameObject player;
    [SerializeField] GameObject projectileParticle;
    [SerializeField] AudioSource audioSource;
    private float distance = 26f;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
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
        if (collision.CompareTag("Enemy")
        || collision.CompareTag("Player Projectile")
        || collision.CompareTag("Missile Projectile")
        || collision.CompareTag("Enemy Projectile")
        )
        {
            if (collision.GetComponent<RushBossEnemy>() == null) // it is Rush Boss Pass
            {
                Destroy(collision.gameObject);
            }
        }
        else if (collision.CompareTag("Player"))
        {
            GameManager.instance.DamagePlayer(4f);
        }

        if (projectileParticle)
        {
            Instantiate(projectileParticle, collision.transform.position, Quaternion.identity);
        }

        if (audioSource && audioSource.clip)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }
}
