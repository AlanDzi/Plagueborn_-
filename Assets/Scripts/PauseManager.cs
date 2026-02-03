using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button resumeButton;
    public Button mainMenuButton;

    [Header("Confirm Panel")]
    public GameObject confirmPanel;
    public TextMeshProUGUI confirmText;
    public Button yesButton;
    public Button noButton;

    [Header("Audio")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;

    private bool isPaused = false;
    private AudioSource audioSource;
    private PlayerController playerController;
    private WeaponController weaponController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerController = FindFirstObjectByType<PlayerController>();
        weaponController = FindFirstObjectByType<WeaponController>();

        if (pausePanel != null) pausePanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);

        SetupButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (confirmPanel != null && confirmPanel.activeSelf)
            {
                HideConfirmPanel();
            }
            else if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    void SetupButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
            AddButtonEffects(resumeButton);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(ShowMainMenuConfirm);
            AddButtonEffects(mainMenuButton);
        }

        if (yesButton != null)
        {
            yesButton.onClick.AddListener(ReturnToMainMenu);
            AddButtonEffects(yesButton);
        }

        if (noButton != null)
        {
            noButton.onClick.AddListener(HideConfirmPanel);
            AddButtonEffects(noButton);
        }
    }

    void AddButtonEffects(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = button.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => {
            OnButtonHover(button);
        });
        trigger.triggers.Add(pointerEnter);

        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => {
            OnButtonExit(button);
        });
        trigger.triggers.Add(pointerExit);

        button.onClick.AddListener(() => PlayButtonClickSound());
    }

    void OnButtonHover(Button button)
    {
        if (buttonHoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonHoverSound, 0.5f);
        }

        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color originalColor = buttonImage.color;
            buttonImage.color = new Color(originalColor.r * 0.8f, originalColor.g * 0.8f, originalColor.b * 0.8f, originalColor.a);
        }
    }

    void OnButtonExit(Button button)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = Color.white;
        }
    }

    void PlayButtonClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (weaponController != null)
        {
            weaponController.enabled = false;
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null) pausePanel.SetActive(false);
        if (confirmPanel != null) confirmPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

        if (weaponController != null)
        {
            weaponController.enabled = true;
        }
    }

    public void ShowMainMenuConfirm()
    {
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
            if (confirmText != null)
            {
                confirmText.text = "Czy na pewno?";
            }
        }
    }

    public void HideConfirmPanel()
    {
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}