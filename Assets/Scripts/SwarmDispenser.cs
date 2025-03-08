using System.Collections;
using UnityEngine;

public class SwarmDispenser : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private GameObject missileInitPos;
    [SerializeField] private GameObject missilePrefab;

    [SerializeField] int missileCount = 4;
    [SerializeField] float rotationMargin = 10f;
    private float rotationOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("Player");
        transform.position = player.transform.position;
        rotationOffset = Random.Range(0, 360);
        //transform.Rotate(0, 0, rotaltionOffset);

        StartCoroutine(FireMissilesWithDelay());
        //Destroy(gameObject);
    }

    private IEnumerator FireMissilesWithDelay()
    {
        for (int i = 0; i < (missileCount + GameManager.instance.curPhase / 2); i++)
        {
            transform.Rotate(0, 0, rotationMargin * i + rotationOffset);
            
            Instantiate(missilePrefab, missileInitPos.transform.position, Quaternion.identity);
            
            yield return new WaitForSeconds(0.35f);
        }

        Destroy(gameObject);
    }
}
