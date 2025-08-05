using UnityEngine;

public class WorldBorder : MonoBehaviour
{
    private GameManager gm;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (gm.curPhase > 0)
            {
                UIManager.instance.ActivateAnnoucer(19);
            }
            gm.SetScoreable(true);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (!gm.isInEpilogue)
            {
                UIManager.instance.ActivateAnnoucer(14);
            }
            gm.SetScoreable(false);
        }
    }

    void OnDisable()
    {
        gm.SetScoreable(true);
    }
}
