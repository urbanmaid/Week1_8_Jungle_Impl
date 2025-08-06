using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] CircleCollider2D rangeCollider;
    [SerializeField] PointEffector2D effector2D;

    [Header("Player Blackhole Param")]
    [SerializeField] float effectorForceInit = -280;
    [SerializeField] float effectorScaleInit = 6;
    [SerializeField] float effectorForceInterval = -40;
    [SerializeField] float effectorScaleInterval = 2;

    [SerializeField] private string AffectedObjectTag = "Player";
    private void OnTriggerEnter2D(Collider2D other)
    {
        bool affectable = false;

        if (GameManager.instance)
        {
            affectable = GameManager.instance.isPlaying;
        }
        
        // "Player" 태그를 가진 객체가 들어오면
        if ((AffectedObjectTag == "Player") && other.CompareTag(AffectedObjectTag) && affectable)
        {
            UIManager.instance.ActivateAnnoucer(15);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // "Player" 태그를 가진 객체가 나가면
        
        if ((AffectedObjectTag == "Player") && other.CompareTag(AffectedObjectTag))
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // 관성 초기화
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }
    }

    internal void SetGravityPower(int power){
        effector2D.forceMagnitude = effectorForceInit + (power * effectorForceInterval);
        rangeCollider.radius = effectorScaleInit + (power * effectorScaleInterval);

        Debug.Log("Blackhole power " + power
         + ", Magnitude: " + effector2D.forceMagnitude
         + ", Radius: " + rangeCollider.radius);
    }
}
