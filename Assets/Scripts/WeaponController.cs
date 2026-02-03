using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public float attackRange = 3f;
    public float attackCooldown = 0.8f;

    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip missSound;
    public AudioClip swingSound;

    private PlayerStats playerStats;
    private PlayerController playerController;
    private Camera playerCamera;
    private float lastAttackTime;
    private AudioSource audioSource;

    void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        playerController = GetComponent<PlayerController>();
        playerCamera = Camera.main;

        if (playerCamera == null)
        {
            playerCamera = FindFirstObjectByType<Camera>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
      
        if (UIManager.Instance != null && UIManager.Instance.IsAnyUIOpen)
            return;

        if (Input.GetMouseButtonDown(0) && CanAttack())
        {
            Attack();
        }
    }

    bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown &&
               playerController.CanUseStamina(playerController.attackStaminaCost);
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        playerController.UseStamina(playerController.attackStaminaCost);

        if (swingSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(swingSound);
        }

        RaycastHit hit;
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        Debug.DrawRay(rayOrigin, rayDirection * attackRange, Color.red, 1f);

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, attackRange))
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                int damageDealt = playerStats.currentDamage;
                enemy.TakeDamage(damageDealt);

                if (hitSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(hitSound);
                }
            }
            else
            {
                if (missSound != null && audioSource != null)
                {
                    audioSource.PlayOneShot(missSound);
                }
            }
        }
    }
}