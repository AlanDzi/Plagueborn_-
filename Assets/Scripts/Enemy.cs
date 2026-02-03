using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 50;
    public int currentHealth;
    public int experienceReward = 25;

    [Header("Movement i AI")]
    public float moveSpeed = 3.5f;
    public float attackRange = 2f;
    public float detectionRange = 8f;
    public float attackCooldown = 2f;

    [Header("Audio")]
    public AudioClip attackSound;
    public AudioClip deathSound;

    private AudioSource audioSource;
    private Transform player;
    private PlayerStats playerStats;
    private NavMeshAgent navAgent;
    private GameManager gameManager;
    private bool isDead = false;
    private bool playerDetected = false;
    private float lastAttackTime = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        player = FindFirstObjectByType<PlayerController>().transform;
        playerStats = player.GetComponent<PlayerStats>();
        gameManager = FindFirstObjectByType<GameManager>();

        navAgent = GetComponent<NavMeshAgent>();
        if (navAgent != null)
        {
            navAgent.speed = moveSpeed;
            navAgent.stoppingDistance = attackRange - 0.2f;
            navAgent.acceleration = 12f;
            navAgent.angularSpeed = 360f;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.spatialBlend = 1f;
    }

    void Update()
    {
        if (isDead) return;

        if (playerStats.currentHealth <= 0)
        {
            if (navAgent != null && navAgent.enabled)
            {
                navAgent.ResetPath();
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!playerDetected && distanceToPlayer <= detectionRange)
        {
            playerDetected = true;
        }

        if (playerDetected)
        {
            if (distanceToPlayer <= attackRange)
            {
                if (navAgent != null && navAgent.enabled)
                {
                    navAgent.ResetPath();
                }

                if (Time.time >= lastAttackTime + attackCooldown)
                {
                    AttackPlayer();
                }
            }
            else if (distanceToPlayer <= detectionRange * 1.5f)
            {
                MoveTowardsPlayer();
            }
            else
            {
                playerDetected = false;
                if (navAgent != null && navAgent.enabled)
                {
                    navAgent.ResetPath();
                }
            }
        }
    }

    void MoveTowardsPlayer()
    {
        if (navAgent != null && navAgent.enabled && playerDetected)
        {
            navAgent.SetDestination(player.position);
        }
    }

    void AttackPlayer()
    {
        if (playerStats.currentHealth <= 0)
        {
            return;
        }

        lastAttackTime = Time.time;

        Vector3 lookDirection = (player.position - transform.position).normalized;
        lookDirection.y = 0;
        if (lookDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        if (attackSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(attackSound);
        }

        int infectionAmount = Random.Range(10, 25);
        playerStats.AddInfection(infectionAmount);

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            Vector3 pushDirection = (player.position - transform.position).normalized;
            playerRb.AddForce(pushDirection * 5f, ForceMode.Impulse);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            playerDetected = true;
        }
    }

    System.Collections.IEnumerator DamageFlash()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material originalMaterial = renderer.material;
            Color originalColor = originalMaterial.color;

            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.15f);

            if (renderer != null && !isDead)
            {
                renderer.material.color = originalColor;
            }
        }
    }

    void Die()
    {
        isDead = true;

        if (navAgent != null)
        {
            navAgent.enabled = false;
        }

        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        playerStats.AddExperience(experienceReward);

        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        if (inventoryManager != null)
        {
            int coinsDropped = Random.Range(5, 16);
            inventoryManager.AddCoins(coinsDropped);
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (gameManager != null)
        {
            StartCoroutine(CheckVictoryAfterDeath());
        }

        StartCoroutine(DeathAnimation());
    }

    System.Collections.IEnumerator CheckVictoryAfterDeath()
    {
        yield return new WaitForSeconds(0.5f);
        gameManager.CheckForVictory();
    }

    System.Collections.IEnumerator DeathAnimation()
    {
        float elapsedTime = 0f;
        float duration = 1.2f;
        Vector3 originalRotation = transform.eulerAngles;
        Vector3 originalPosition = transform.position;

        float fallDirection = Random.Range(0f, 1f) > 0.5f ? 90f : -90f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            Vector3 targetRotation = new Vector3(fallDirection, originalRotation.y, originalRotation.z);
            transform.rotation = Quaternion.Lerp(
                Quaternion.Euler(originalRotation),
                Quaternion.Euler(targetRotation),
                smoothProgress
            );

            transform.position = Vector3.Lerp(
                originalPosition,
                originalPosition + Vector3.down * 0.3f,
                smoothProgress
            );

            yield return null;
        }

        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}