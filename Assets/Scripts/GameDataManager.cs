using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Player Progress")]
    public int playerLevel = 1;
    public int playerExperience = 0;
    public int playerExperienceToNext = 100;
    public int playerHealth = 100;
    public int playerMaxHealth = 100;
    public int playerInfection = 0;
    public int playerDamage = 20;

    [Header("Inventory")]
    public int antidotes = 1;
    public int bandages = 1;
    public int coins = 0;

    [Header("First Level Setup")]
    public bool isFirstLevel = true;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SavePlayerData()
    {
        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (playerStats != null)
        {
            playerLevel = playerStats.level;
            playerExperience = playerStats.experience;
            playerExperienceToNext = playerStats.experienceToNext;
            playerHealth = playerStats.currentHealth;
            playerMaxHealth = playerStats.maxHealth;
            playerInfection = playerStats.currentInfection;
            playerDamage = playerStats.currentDamage;
        }

        if (inventoryManager != null)
        {
            antidotes = inventoryManager.antidotes;
            bandages = inventoryManager.bandages;
            coins = inventoryManager.coins;
        }

        isFirstLevel = false;
    }

    public void LoadPlayerData()
    {
        if (isFirstLevel)
        {
            return;
        }

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();

        if (playerStats != null)
        {
            playerStats.level = playerLevel;
            playerStats.experience = playerExperience;
            playerStats.experienceToNext = playerExperienceToNext;
            playerStats.currentHealth = playerHealth;
            playerStats.maxHealth = playerMaxHealth;
            playerStats.currentInfection = playerInfection;
            playerStats.currentDamage = playerDamage;
        }

        if (inventoryManager != null)
        {
            inventoryManager.antidotes = antidotes;
            inventoryManager.bandages = bandages;
            inventoryManager.coins = coins;
        }
    }

    public void ResetData()
    {
        playerLevel = 1;
        playerExperience = 0;
        playerExperienceToNext = 100;
        playerHealth = 100;
        playerMaxHealth = 100;
        playerInfection = 0;
        playerDamage = 20;
        
        antidotes = 1;
        bandages = 1;
        coins = 0;
        
        isFirstLevel = true;
    }
}