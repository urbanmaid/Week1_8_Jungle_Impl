using UnityEngine;

public class BlackHole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Player" 태그를 가진 객체가 들어오면
        if (other.CompareTag("Player"))
        {
            UIManager.instance.ActivateAnnoucer(15);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // "Player" 태그를 가진 객체가 나가면
        if (other.CompareTag("Player"))
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
}
