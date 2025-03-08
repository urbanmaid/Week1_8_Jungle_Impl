using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ProjectileExplosive : Projectile
{
    [Header("Explosion Debris")]
    [SerializeField] float explosionDelay;
    [SerializeField] GameObject debris;
    [SerializeField] int debrisCount;

    protected override void Start()
    {
        base.Start();
        Invoke(nameof(Explode), explosionDelay);
    }

    void Explode()
    {
        float angleStep = 360f / debrisCount;
        float angleOffset = Random.Range(0f, angleStep);

        for (int i = 0; i < debrisCount; i++)
        {
            Instantiate(debris, transform.position, Quaternion.Euler(0f, 0f, (i * angleStep + angleOffset)));
        }

        Destroy(gameObject);
    }
}