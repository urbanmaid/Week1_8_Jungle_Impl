using System.Collections;
using UnityEngine;

public class MotherShip : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float turnInterval = 2f;
    [SerializeField] float turnAngle = 30f;
    [SerializeField] float turnDuration = 1f; // 회전 지속 시간

    private GameManager gm;
    private float turnTimer;
    [SerializeField] private GameObject projectileParticle;

    void Start()
    {
        turnTimer = turnInterval;
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        gm.NotifyMothership(gameObject);
    }

    void Update()
    {
        if(gm.isPlaying){
            transform.position += moveSpeed * Time.deltaTime * transform.up;

            turnTimer -= Time.deltaTime;

            if (turnTimer <= 0)
            {
                StartCoroutine(RotateSmoothly(Random.Range(-turnAngle, turnAngle)));
                turnTimer = turnInterval;
            }
        }
    }

    IEnumerator RotateSmoothly(float angle)
    {
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 0, angle);

        while (elapsedTime < turnDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / turnDuration);
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Destroy(collision.gameObject);
            if(projectileParticle)
            { 
                Instantiate(projectileParticle, collision.transform.position, Quaternion.identity); 
            }
        }
        else if(collision.CompareTag("Player"))
        {
            UIManager.instance.ActivateAnnoucer(21);
        }
    }
}
