using System.Collections;
using UnityEngine;

public class LaserBossEnemy : BossEnemy
{
    [Header("Characteristics")]
    [SerializeField] private GameObject laserPrefab;
    private GameObject laser;

    [SerializeField] private float laserFiringSpeed = 1.8f;
    [SerializeField] private float laserDuration = 2.5f;
    [SerializeField] private float laserReachDistance = 35f;

    private Vector2 targetDirection;
    private float laserDistance;

    [SerializeField] private float laserPrepDuration = 1f;
    [SerializeField] private float firingDetectingRange = 12.5f;
    private bool canFire = true;

    protected override void Start()
    {
        base.Start();
    }
    
    protected override void Update()
    {
        base.Update();

        if (!GameManager.instance.isPlaying)
        {
            return;
        }
        
        if (canFire && player != null)
        {
            var distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= firingDetectingRange)
            {
                StartCoroutine(FireLaser());
            }
        }

        if (laser != null)
        {
            UpdateLaserPosition();
        }
    }

    IEnumerator FireLaser()
    {
        if (bossSkillFX) bossSkillFX.SetActive(true);
        yield return new WaitForSeconds(bossSkillFXDuration);
        if (bossSkillFX) bossSkillFX.SetActive(false);

        Vector2 targetPosition = player.transform.position;
        targetDirection = (targetPosition - (Vector2)transform.position).normalized;
        laserDistance = 0f;

        // Create laser
        if(canFire)
        {
            laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            canFire = false;
        }
        laser.transform.localScale = new Vector3(0.5f, 0f, 1f);

        // Increase laser
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * laserFiringSpeed;
            laserDistance = Mathf.Lerp(0, laserReachDistance, elapsed);
            UpdateLaserTransform();
            yield return null;
        }

        yield return new WaitForSeconds(laserDuration);

        // Decrease Laser
        elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * laserFiringSpeed;
            laserDistance = Mathf.Lerp(laserReachDistance, 0, elapsed);
            UpdateLaserTransform();
            yield return null;
        }

        // Delete laser
        Destroy(laser);
        laser = null;

        yield return new WaitForSeconds(laserPrepDuration);
        canFire = true;
    }

    void UpdateLaserPosition()
    {
        if (laser != null)
        {
            laser.transform.position = transform.position + (Vector3)targetDirection * (laserDistance * 0.5f);
        }
    }

    void UpdateLaserTransform()
    {
        if (laser != null)
        {
            laser.transform.localScale = new Vector3(0.5f, laserDistance, 1f);

            laser.transform.position = transform.position + (Vector3)targetDirection * (laserDistance * 0.5f);

            var angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            laser.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    protected override void SetDestroy()
    {
        base.SetDestroy();

        // Delete laser
        Destroy(laser);
        laser = null;
    }

    private void OnDestroy()
    {
        if (laser != null)
        {
            Destroy(laser);
        }
    }
}
