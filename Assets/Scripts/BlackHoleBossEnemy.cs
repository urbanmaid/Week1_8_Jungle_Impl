    using UnityEngine;

public class BlackHoleBossEnemy : BossEnemy
{
    [SerializeField] PointEffector2D pointEffector;
    protected override void Start()
    {
        base.Start();
    }

    public void ActivateBlackHole(bool isActive)
    {
        pointEffector.enabled = isActive;
    }

    protected override void SetDestroy()
    {
        base.SetDestroy();

        ActivateBlackHole(false);
    }
}