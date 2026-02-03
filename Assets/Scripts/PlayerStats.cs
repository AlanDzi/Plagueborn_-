using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStats : MonoBehaviour
{
    [Header("Stats")]
    public int level = 1;
    public int experience = 0;
    public int experienceToNext = 100;

    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Stamina System")]
    public int maxStamina = 100;
    public int currentStamina;

    [Header("Infection System")]
    public int maxInfection = 100;
    public int currentInfection = 0;
    public float infectionDamageRate = 1f;
    public int baseDamagePerTick = 1;

    [Header("Combat")]
    public int baseDamage = 20;
    public int currentDamage;

    [Header("Death Settings")]
    public float deathDelay = 2f;

    private bool isDead = false;
    private bool gameWon = false;
    private float lastInfectionDamage = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentDamage = baseDamage;

        StartCoroutine(LoadDataAfterFrame());
    }

    System.Collections.IEnumerator LoadDataAfterFrame()
    {
        yield return null;
        SaveSystem.LoadPlayerData();
    }

    void Update()
    {
        if (isDead || gameWon) return;

        if (currentInfection > 0)
        {
            ProcessInfection();
        }
    }

    public void OnGameWon()
    {
        gameWon = true;
    }

    void ProcessInfection()
    {
        float infectionPercent = (float)currentInfection / maxInfection;
        float damageInterval = infectionDamageRate * (1f - infectionPercent * 0.8f);

        if (Time.time >= lastInfectionDamage + damageInterval)
        {
            lastInfectionDamage = Time.time;

            int damageAmount = Mathf.RoundToInt(baseDamagePerTick * (1f + infectionPercent * 2f));

            currentHealth -= damageAmount;

            StartCoroutine(InfectionFlash());

            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Die();
            }
        }
    }

    public void AddInfection(int amount)
    {
        currentInfection = Mathf.Clamp(currentInfection + amount, 0, maxInfection);
    }

    public void ReduceInfection(int amount)
    {
        currentInfection = Mathf.Clamp(currentInfection - amount, 0, maxInfection);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || gameWon) return;

        currentHealth -= damage;

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    System.Collections.IEnumerator DamageFlash()
    {
        yield return null;
    }

    System.Collections.IEnumerator InfectionFlash()
    {
        yield return null;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    public void AddExperience(int amount)
    {
        experience += amount;
        CheckLevelUp();
    }

    void CheckLevelUp()
    {
        if (experience >= experienceToNext)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        experience -= experienceToNext;
        experienceToNext = Mathf.RoundToInt(experienceToNext * 1.2f);

        maxHealth = Mathf.RoundToInt(maxHealth * 1.05f);
        currentHealth = maxHealth;
        maxStamina = Mathf.RoundToInt(maxStamina * 1.05f);
        currentStamina = maxStamina;
        
        currentDamage = Mathf.RoundToInt(baseDamage * (1 + (level - 1) * 0.05f));
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        StartCoroutine(RestartGameAfterDelay());
    }

    System.Collections.IEnumerator RestartGameAfterDelay()
    {
        yield return new WaitForSeconds(deathDelay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}