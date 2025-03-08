using System;
using System.Collections;
using UnityEngine;

public class RushBossEnemy : BossEnemy
{
    [Header("Characteristics")]
    [SerializeField] private float rushDistance = 5.0f;
    [SerializeField] private float rushSpeed = 10.0f;
    [SerializeField] private float rushDelay = 3.0f;
    private float rushDelayElapsed;
    private float moveSpeedBackup;
    private int rushStatus = 0;

    //private bool _isRushing;

    protected override void Start()
    {
        base.Start();
        moveSpeedBackup = moveSpeed;
        //StartCoroutine(RushV2());
        //InvokeRepeating(nameof(RushV2), 0f, rushDelay);
    }

    protected override void Update()
    {
        if (player != null)
        {
            if (gm.isPlaying)
            {
                rushDelayElapsed += Time.deltaTime;
                if (rushDelayElapsed >= rushDelay)
                {
                    rushDelayElapsed = 0;
                    rushStatus = 1;

                    bossSkillFX.SetActive(true);
                    Invoke(nameof(StartRush), bossSkillFXDuration);
                    Invoke(nameof(StopRush), (rushDistance/rushSpeed) + bossSkillFXDuration);
                }

                if(isSteerable)
                {
                    Steer();
                }

                enemyRb.linearVelocity = moveDir.normalized * moveSpeed;
            }
        }
    }

    private void StartRush()
    {
        isSteerable = false;
        moveSpeed = rushSpeed;
        bossSkillFX.SetActive(false);
    }

    private void StopRush()
    {
        isSteerable = true;
        moveSpeed = moveSpeedBackup;
        rushStatus = 0;
    }
}
