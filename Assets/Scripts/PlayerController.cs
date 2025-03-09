using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Unity.VisualScripting;
using Unity.Mathematics;

public class PlayerController : MonoBehaviour
{
    private GameManager gm;

    [Header("Components")]
    private Collider2D coll;
    private SpriteRenderer rend;
    private Rigidbody2D playerRb;

    [Header("Player Movement")]
    public float moveSpeed;
    private float curSpeed;
    private Camera mainCam;
    public GameObject playerSprite;
    private Vector3 launchDirection = new Vector3(0, 0, 0);
    private Vector2 offset;
    [SerializeField] private GameObject projectileParticle;


    [Header("Basic Projectile")]
    public GameObject projectilePrefab;
    public float fireRate;
    private float coolDown;
    public float damage = 5;
    public float projectileSpeed = 7;
    public Color projectileColor;

    [Header("Missile")]
    public GameObject missilePrefab;
    public float spawnDistance;
    public GameObject missileGuide;
    public int missilePower;
    /*
    private GameObject _currentProjectile;
    private float rotationSpeed = 100f;
    private float _rotateAngle = 90f;
    private int _rotateDirection = 0;
    public float launchSpeed = 5f;
    */

    [Header("Skills")]
    [SerializeField] bool isUsingSkill;
    public int skillPower = 1;
    internal bool isRushing;
    [SerializeField] float rushingSpeed = 25f;
    [SerializeField] float rushingDistance = 15f;

    internal bool isShielded;
    [SerializeField] GameObject shield;
    [SerializeField] GameObject gravityShot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = GameManager.instance;
        coolDown = 0;
        playerRb = gameObject.GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        isRushing = false;
        curSpeed = moveSpeed;
        coll = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (gm.isPlaying)
        {
            Move();
            
            ProjectileUsage();
            SkillUsage();
        } 
        else 
        {
            playerRb.linearVelocity = Vector2.zero;
        }
    }

    void Move()
    {
        if(!isRushing)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCam.WorldToScreenPoint(transform.localPosition);
            offset = new Vector2(mousePos.x - worldPos.x, mousePos.y - worldPos.y);
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            playerSprite.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            Vector2 moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            transform.Translate(moveDir.normalized * curSpeed * Time.deltaTime, Space.World);
        }
    }

    void ProjectileUsage()
    {
        // Projectile
        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
        else
        {
            if (Input.GetButton("Fire1"))
            {
                LaunchProjectile();
            }
        }

        // Missile, Launch once for each click
        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(LaunchMissile());
        }
    }

    void LaunchProjectile()
    {
        Instantiate(projectilePrefab, transform.position, Quaternion.Euler(launchDirection + playerSprite.transform.rotation.eulerAngles));
        coolDown = fireRate;
    }

    private IEnumerator LaunchMissile()
    {
        if(gm.missileAmount <= 0)
        {
            yield break;
        }
        else // If you have missiles
        {
            // Show Missile Prep
            missileGuide.SetActive(true);
            yield return new WaitForSeconds(1f);

            // Hide Missile Prep
            missileGuide.SetActive(false);
            Instantiate(missilePrefab, transform.position, Quaternion.Euler(launchDirection + playerSprite.transform.rotation.eulerAngles));

            gm.NotifyMissileUsed();
        }
    }

    // Skill Item Usage
    void SkillUsage()
    {
        // Skill Usage
        if(!isUsingSkill){
            if (Input.GetButtonDown("Skill1") && gm.skillRush > 0)
            {
                UIManager.instance.ActivateAnnoucer(5);

                isUsingSkill = true;
                FXRush();
                gm.UseSkillRush();
            }
            if (Input.GetButtonDown("Skill2") && gm.skillShield > 0)
            {
                UIManager.instance.ActivateAnnoucer(6);

                isUsingSkill = true;
                FXShield();
                gm.UseSkillShield();
            }
            if (Input.GetButtonDown("Skill3") && gm.skillGravityShot > 0)
            {
                UIManager.instance.ActivateAnnoucer(7);

                isUsingSkill = true;
                FXGravityShot();
                gm.UseSkillGravityShot();
            }
        }
    }

    public void FXRush()
    {
        if (!isRushing)
        {
            isRushing = true;
            gm.ToggleShakeCamera(true);

            // Affecting physics
            playerRb.linearVelocity = offset.normalized * rushingSpeed;
            StartCoroutine(FXRushCo());
        }
    }
    IEnumerator FXRushCo()
    {
        yield return new WaitForSeconds((rushingDistance / rushingSpeed));

        // Reset physics
        isRushing = false;
        playerRb.linearVelocity = Vector2.zero;

        curSpeed = moveSpeed;
        isUsingSkill = false;
        gm.ToggleShakeCamera(false);
    }

    public void FXShield()
    {
        if (!isShielded)
        {
            isShielded = true;
            shield.SetActive(true);
            StartCoroutine(FXShieldCo());
        }
    }
    IEnumerator FXShieldCo()
    {
        yield return new WaitForSeconds(2.5f + (0.5f* skillPower));
        isShielded = false;
        shield.SetActive(false);

        isUsingSkill = false;
    }

    public void FXGravityShot()
    {
        Debug.Log("shot gravity shot");
        GameObject gravShot = Instantiate(gravityShot, transform.position, playerSprite.transform.rotation);
        //gravShot.transform.localScale *= skillPower;
        // Find child which has BlackHole component and set scale of radius
        Invoke(nameof(StopUsingSkill), 0.8f);

        gravShot.transform.GetChild(0).GetComponent<BlackHole>().SetGravityPower(skillPower);
    }

    private void StopUsingSkill()
    {
        isUsingSkill = false;
    }

    // Collision
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isRushing && collision.gameObject.CompareTag("Enemy")) // FXRush effects
        {
            collision.gameObject.GetComponent<EnemyController>().Damage(8f + (4f * skillPower));
            Instantiate(projectileParticle, transform.position, Quaternion.identity);
            Debug.Log(8f + (4f * skillPower));
        }
    }
}
