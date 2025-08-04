using UnityEngine;

public class PlayerSoundController : MonoBehaviour
{
    private AudioSource audioSource;

    [SerializeField] AudioClip[] clipShooting;
    [SerializeField] AudioClip clipShootingMissile;
    [SerializeField] AudioClip[] clipDamaged;
    [Space]
    [SerializeField] AudioClip clipBoost;
    [SerializeField] AudioClip clipShield;
    [SerializeField] AudioClip clipGravityShot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayShooting()
    {
        if (audioSource == null || clipShooting.Length == 0) return;

        int i = Random.Range(0, clipShooting.Length);
        audioSource.PlayOneShot(clipShooting[i]);
    }

    public void PlayShootingMissile()
    {
        if (audioSource == null || clipShootingMissile == null) return;

        audioSource.PlayOneShot(clipShootingMissile);
    }

    public void PlayDamaged()
    {
        if (audioSource == null || clipDamaged.Length == 0) return;

        int i = Random.Range(0, clipDamaged.Length);
        audioSource.PlayOneShot(clipDamaged[i]);
    }

    public void PlaySkillBoost()
    {
        if (audioSource == null || clipBoost == null) return;
        audioSource.PlayOneShot(clipBoost);
    }

    public void PlaySkillShield()
    {
        if (audioSource == null || clipShield == null) return;
        audioSource.PlayOneShot(clipShield);
    }

    public void PlaySkillGravity()
    {
        if (audioSource == null || clipGravityShot == null) return;
        audioSource.PlayOneShot(clipGravityShot);
    }
}
