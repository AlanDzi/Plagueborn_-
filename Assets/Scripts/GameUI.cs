using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameUI : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthBar;
    public TextMeshProUGUI healthText;

    [Header("Stamina UI")]
    public Slider staminaBar;
    public TextMeshProUGUI staminaText;

    [Header("Infection UI")]
    public Slider infectionBar;
    public TextMeshProUGUI infectionText;

    [Header("Level UI")]
    public Slider experienceBar;
    public TextMeshProUGUI levelText;

    [Header("Notifications")]
    public GameObject levelUpPanel;
    public TextMeshProUGUI levelUpText;
    public AudioClip levelUpSound;

    [Header("Death Screen")]
    public GameObject deathPanel;
    public TextMeshProUGUI deathText;
    public Button retryButton;
    public AudioClip deathSound;

    [Header("Colors")]
    public Color healthColor = Color.green;
    public Color lowHealthColor = Color.red;
    public Color staminaColor = Color.blue;
    public Color lowStaminaColor = Color.cyan;
    public Color infectionColor = Color.yellow;
    public Color highInfectionColor = Color.red;

    private PlayerStats playerStats;
    private PlayerController playerController;
    private AudioSource audioSource;
    private int lastLevel = 1;

    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        playerController = FindFirstObjectByType<PlayerController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (healthBar != null)
            healthBar.fillRect.GetComponent<Image>().color = healthColor;

        if (staminaBar != null)
            staminaBar.fillRect.GetComponent<Image>().color = staminaColor;

        if (infectionBar != null)
            infectionBar.fillRect.GetComponent<Image>().color = infectionColor;

        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (deathPanel != null) deathPanel.SetActive(false);

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RestartGame);
        }
    }

    void Update()
    {
        if (playerStats == null) return;

        UpdateHealthUI();
        UpdateStaminaUI();
        UpdateInfectionUI();
        UpdateExperienceUI();
        CheckLevelUp();
        CheckPlayerDeath();
    }

    void CheckLevelUp()
    {
        if (playerStats.level > lastLevel)
        {
            ShowLevelUpNotification();
            lastLevel = playerStats.level;
        }
    }

    void CheckPlayerDeath()
    {
        if (playerStats.currentHealth <= 0 && deathPanel != null && !deathPanel.activeSelf)
        {
            ShowDeathScreen();
        }
    }

    void ShowLevelUpNotification()
    {
        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(true);
            if (levelUpText != null)
                levelUpText.text = "NOWY POZIOM!\nPoziom " + playerStats.level;

            if (levelUpSound != null && audioSource != null)
                audioSource.PlayOneShot(levelUpSound);

            StartCoroutine(HideLevelUpAfterDelay());
        }
    }

    System.Collections.IEnumerator HideLevelUpAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }

    void ShowDeathScreen()
    {
        if (deathPanel != null)
        {
            deathPanel.SetActive(true);

            if (deathSound != null && audioSource != null)
                audioSource.PlayOneShot(deathSound);

            Cursor.lockState = CursorLockMode.None;

            if (playerController != null) playerController.enabled = false;

            WeaponController weaponController = playerStats.GetComponent<WeaponController>();
            if (weaponController != null) weaponController.enabled = false;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            float healthPercent = (float)playerStats.currentHealth / playerStats.maxHealth;
            healthBar.value = healthPercent;

            Image fillImage = healthBar.fillRect.GetComponent<Image>();
            fillImage.color = Color.Lerp(lowHealthColor, healthColor, healthPercent);
        }

        if (healthText != null)
        {
            healthText.text = playerStats.currentHealth + " / " + playerStats.maxHealth;
        }
    }

    void UpdateStaminaUI()
    {
        if (playerController == null) return;

        if (staminaBar != null)
        {
            float staminaPercent = playerController.currentStamina / playerController.maxStamina;
            staminaBar.value = staminaPercent;

            Image fillImage = staminaBar.fillRect.GetComponent<Image>();
            fillImage.color = Color.Lerp(lowStaminaColor, staminaColor, staminaPercent);
        }

        if (staminaText != null)
        {
            staminaText.text = "Stamina: " + Mathf.RoundToInt(playerController.currentStamina) + " / " + Mathf.RoundToInt(playerController.maxStamina);
        }
    }

    void UpdateInfectionUI()
    {
        if (infectionBar != null)
        {
            float infectionPercent = (float)playerStats.currentInfection / playerStats.maxInfection;
            infectionBar.value = infectionPercent;

            Image fillImage = infectionBar.fillRect.GetComponent<Image>();
            fillImage.color = Color.Lerp(infectionColor, highInfectionColor, infectionPercent);
        }

        if (infectionText != null)
        {
            infectionText.text = "Infection: " + playerStats.currentInfection + "%";
        }
    }

    void UpdateExperienceUI()
    {
        if (experienceBar != null)
        {
            float expPercent = (float)playerStats.experience / playerStats.experienceToNext;
            experienceBar.value = expPercent;
        }

        if (levelText != null)
        {
            levelText.text = "Level " + playerStats.level;
        }
    }
}