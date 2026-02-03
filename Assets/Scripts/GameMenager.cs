using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Victory Settings")]
    public GameObject victoryPanel;
    public TextMeshProUGUI victoryText;
    public Button nextLevelButton;
    public Button mainMenuButton;

    [Header("Audio")]
    public AudioClip victorySound;

    [Header("Level Settings")]
    public string nextLevelScene = "TestScene2";
    public string mainMenuScene = "MainMenuScene";

    private Enemy[] allEnemies;
    private AudioSource audioSource;
    private bool victoryTriggered = false;
    private bool isLastLevel = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        FindAllEnemies();
        CheckPlayerClass();

        string currentScene = SceneManager.GetActiveScene().name;
        isLastLevel = (currentScene == "TestScene2");

        if (nextLevelButton != null)
        {
            TextMeshProUGUI buttonText = nextLevelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isLastLevel ? "Powrót do menu" : "Następny poziom";
            }
        }

        if (victoryText != null)
        {
            if (isLastLevel)
            {
                victoryText.text = "To Konic";
            }
            else
            {
                victoryText.text = "Zwycięstwo!";
            }
        }

        SetupButtons();
    }

    void SetupButtons()
    {
        if (nextLevelButton != null)
        {
            nextLevelButton.onClick.AddListener(() => {
                LoadNextLevel();
            });
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(() => {
                LoadMainMenu();
            });
        }
    }

    void Update()
    {
        if (!victoryTriggered)
        {
            CheckVictoryCondition();
        }
    }

    void FindAllEnemies()
    {
        allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
    }

    void CheckPlayerClass()
    {
        string selectedClass = PlayerPrefs.GetString("SelectedClass", "");
        if (!string.IsNullOrEmpty(selectedClass))
        {
            // Tutaj możesz dodać logikę dla różnych klas
        }
    }

    void CheckVictoryCondition()
    {
        int aliveEnemies = 0;
        
        foreach (Enemy enemy in allEnemies)
        {
            if (enemy != null)
            {
                aliveEnemies++;
            }
        }

        if (aliveEnemies == 0 && allEnemies.Length > 0)
        {
            TriggerVictory();
        }
    }

    void TriggerVictory()
    {
        victoryTriggered = true;

        PlayerStats playerStats = FindFirstObjectByType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.OnGameWon();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        WeaponController weaponController = FindFirstObjectByType<WeaponController>();
        if (weaponController != null)
        {
            weaponController.enabled = false;
        }

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }
    }

    public void LoadNextLevel()
    {
        SaveSystem.SavePlayerData();

        if (isLastLevel)
        {
            SceneManager.LoadScene(mainMenuScene);
        }
        else
        {
            SceneManager.LoadScene(nextLevelScene);
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    public void CheckForVictory()
    {
        if (!victoryTriggered)
        {
            CheckVictoryCondition();
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        System.Collections.Generic.List<Enemy> enemyList = new System.Collections.Generic.List<Enemy>(allEnemies);
        enemyList.Add(enemy);
        allEnemies = enemyList.ToArray();
    }
}