using UnityEngine;

public static class SaveSystem
{
    private const string LEVEL_KEY = "PlayerLevel";
    private const string EXP_KEY = "PlayerExperience";
    private const string EXP_TO_NEXT_KEY = "PlayerExpToNext";
    private const string HEALTH_KEY = "PlayerHealth";
    private const string MAX_HEALTH_KEY = "PlayerMaxHealth";
    private const string INFECTION_KEY = "PlayerInfection";
    private const string DAMAGE_KEY = "PlayerDamage";

    private const string ANTIDOTES_KEY = "Antidotes";
    private const string BANDAGES_KEY = "Bandages";
    private const string COINS_KEY = "Coins";

    private const string FIRST_LEVEL_KEY = "FirstLevel";

    public static void SavePlayerData()
    {
        PlayerStats playerStats = Object.FindFirstObjectByType<PlayerStats>();
        InventoryManager inventoryManager = Object.FindFirstObjectByType<InventoryManager>();

        if (playerStats != null)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, playerStats.level);
            PlayerPrefs.SetInt(EXP_KEY, playerStats.experience);
            PlayerPrefs.SetInt(EXP_TO_NEXT_KEY, playerStats.experienceToNext);
            PlayerPrefs.SetInt(HEALTH_KEY, playerStats.currentHealth);
            PlayerPrefs.SetInt(MAX_HEALTH_KEY, playerStats.maxHealth);
            PlayerPrefs.SetInt(INFECTION_KEY, playerStats.currentInfection);
            PlayerPrefs.SetInt(DAMAGE_KEY, playerStats.currentDamage);
        }

        if (inventoryManager != null)
        {
            PlayerPrefs.SetInt(ANTIDOTES_KEY, inventoryManager.antidotes);
            PlayerPrefs.SetInt(BANDAGES_KEY, inventoryManager.bandages);
            PlayerPrefs.SetInt(COINS_KEY, inventoryManager.coins);
        }

        PlayerPrefs.SetInt(FIRST_LEVEL_KEY, 0);
        PlayerPrefs.Save();
    }

    public static void LoadPlayerData()
    {
        bool isFirstLevel = PlayerPrefs.GetInt(FIRST_LEVEL_KEY, 1) == 1;

        if (isFirstLevel)
        {
            return;
        }

        PlayerStats playerStats = Object.FindFirstObjectByType<PlayerStats>();
        InventoryManager inventoryManager = Object.FindFirstObjectByType<InventoryManager>();

        if (playerStats != null)
        {
            playerStats.level = PlayerPrefs.GetInt(LEVEL_KEY, 1);
            playerStats.experience = PlayerPrefs.GetInt(EXP_KEY, 0);
            playerStats.experienceToNext = PlayerPrefs.GetInt(EXP_TO_NEXT_KEY, 100);
            playerStats.currentHealth = PlayerPrefs.GetInt(HEALTH_KEY, 100);
            playerStats.maxHealth = PlayerPrefs.GetInt(MAX_HEALTH_KEY, 100);
            playerStats.currentInfection = PlayerPrefs.GetInt(INFECTION_KEY, 0);
            playerStats.currentDamage = PlayerPrefs.GetInt(DAMAGE_KEY, 20);
        }

        if (inventoryManager != null)
        {
            inventoryManager.antidotes = PlayerPrefs.GetInt(ANTIDOTES_KEY, 1);
            inventoryManager.bandages = PlayerPrefs.GetInt(BANDAGES_KEY, 1);
            inventoryManager.coins = PlayerPrefs.GetInt(COINS_KEY, 0);
        }
    }

    public static void ResetData()
    {
        PlayerPrefs.DeleteKey(LEVEL_KEY);
        PlayerPrefs.DeleteKey(EXP_KEY);
        PlayerPrefs.DeleteKey(EXP_TO_NEXT_KEY);
        PlayerPrefs.DeleteKey(HEALTH_KEY);
        PlayerPrefs.DeleteKey(MAX_HEALTH_KEY);
        PlayerPrefs.DeleteKey(INFECTION_KEY);
        PlayerPrefs.DeleteKey(DAMAGE_KEY);
        PlayerPrefs.DeleteKey(ANTIDOTES_KEY);
        PlayerPrefs.DeleteKey(BANDAGES_KEY);
        PlayerPrefs.DeleteKey(COINS_KEY);
        PlayerPrefs.SetInt(FIRST_LEVEL_KEY, 1);
        PlayerPrefs.Save();
    }
}