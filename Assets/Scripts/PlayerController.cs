using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    private GameManager gm;

    [Header("Components")]
    private Collider2D coll;
    private SpriteRenderer rend;

    [Header("Player Movement")]
    private Rigidbody2D playerRb;
    public float moveSpeed;
    private float curSpeed;
    private Camera mainCam;
    public GameObject playerSprite;
    private Vector3 launchDirection = new Vector3(0, 0, 0);


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
    public float ChargeTime = 1.5f;
    [SerializeField] GameObject gravityShot;
    [SerializeField] bool isChargeing;
    public bool isShielded;

    public GameObject shield;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = GameManager.instance;
        coolDown = 0;
        playerRb = gameObject.GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
        isChargeing = false;
        curSpeed = moveSpeed;
        coll = GetComponent<Collider2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (gm.isPlaying)
        {
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCam.WorldToScreenPoint(transform.localPosition);
            Vector2 offset = new Vector2(mousePos.x - worldPos.x, mousePos.y - worldPos.y);
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            playerSprite.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

            Vector2 moveDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            transform.Translate(moveDir.normalized * curSpeed * Time.deltaTime, Space.World);
            
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

            // Skill Usage
            if(!isUsingSkill){
                if (Input.GetButtonDown("Skill1"))
                {
                    isUsingSkill = true;
                    FXRush();
                    Invoke(nameof(StopUsingSkill), ChargeTime);
                }
                if (Input.GetButtonDown("Skill2"))
                {
                    isUsingSkill = true;
                    FXShield();
                    Invoke(nameof(StopUsingSkill), ChargeTime);
                }
                if (Input.GetButtonDown("Skill3"))
                {
                    isUsingSkill = true;
                    FXGravityShot();
                    Invoke(nameof(StopUsingSkill), ChargeTime);
                }
            }
        } 
        else 
        {
            playerRb.linearVelocity = Vector2.zero;
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

    // Item FX
    public void FXRush()
    {
        if (!isChargeing)
        {
            isChargeing = true;
            curSpeed *= 4;
            StartCoroutine(FXRushCo());
        }
    }
    IEnumerator FXRushCo()
    {
        yield return new WaitForSeconds(1f * skillPower);
        isChargeing = false;
        curSpeed = moveSpeed;
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
        yield return new WaitForSeconds(3f * skillPower);
        isShielded = false;
        shield.SetActive(false);
    }

    public void FXGravityShot()
    {
        Debug.Log("shot gravity shot");
        GameObject gravShot = Instantiate(gravityShot, transform.position, playerSprite.transform.rotation);
        gravShot.transform.localScale *= skillPower;
    }

    private void StopUsingSkill()
    {
        isUsingSkill = false;
    }

    // Collision
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isChargeing && collision.gameObject.CompareTag("Enemy")) // FXRush effects
        {
            collision.gameObject.GetComponent<EnemyController>().Damage(10f);
        }
    }
}
