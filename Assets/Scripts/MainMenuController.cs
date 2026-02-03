using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject mainMenuPanel;
    public GameObject classSelectionPanel;
    public GameObject settingsPanel;

    [Header("Main Menu Buttons")]
    public Button playButton;
    public Button settingsButton;
    public Button quitButton;

    [Header("Class Selection")]
    public Button knightButton;
    public Button confirmButton;
    public Button backToSelectionButton;
    public Button backToMainMenuButton;
    public TextMeshProUGUI selectedClassText;

    [Header("Settings Panel")]
    public Button backFromSettingsButton;

    [Header("Confirm Panel")]
    public GameObject confirmPanel;
    public TextMeshProUGUI confirmText;
    public Button yesButton;
    public Button noButton;

    [Header("Background Video")]
    public RawImage videoBackground;
    public UnityEngine.Video.VideoPlayer videoPlayer;

    [Header("Audio")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;
    public AudioClip ambientMusic;

    private AudioSource audioSource;
    private string selectedClass = "";

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainMenu();
        SetupButtons();
        SetupVideoBackground();
        SetupAmbientMusic();
    }

    void SetupVideoBackground()
    {
        if (videoPlayer != null && videoBackground != null)
        {
            videoPlayer.isLooping = true;
            videoPlayer.playOnAwake = true;
            videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.RenderTexture;
            
            RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 0);
            videoPlayer.targetTexture = rt;
            videoBackground.texture = rt;
            
            videoPlayer.Play();
        }
    }

    void SetupButtons()
    {
        playButton.onClick.AddListener(ShowClassSelection);
        settingsButton.onClick.AddListener(ShowSettings);
        quitButton.onClick.AddListener(ShowQuitConfirmation);

        knightButton.onClick.AddListener(() => SelectClass("Rycerz"));
        confirmButton.onClick.AddListener(ConfirmClassSelection);
        backToSelectionButton.onClick.AddListener(ResetClassSelection);
        backToMainMenuButton.onClick.AddListener(ShowMainMenu);

        backFromSettingsButton.onClick.AddListener(ShowMainMenu);

        yesButton.onClick.AddListener(QuitGame);
        noButton.onClick.AddListener(ShowMainMenu);

        AddButtonEffects(playButton);
        AddButtonEffects(settingsButton);
        AddButtonEffects(quitButton);
        AddButtonEffects(knightButton);
        AddButtonEffects(confirmButton);
        AddButtonEffects(backFromSettingsButton);
        AddButtonEffects(yesButton);
        AddButtonEffects(noButton);
        AddButtonEffects(backToSelectionButton);
        AddButtonEffects(backToMainMenuButton);
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

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        classSelectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
        confirmPanel.SetActive(false);
    }

    public void ShowClassSelection()
    {
        mainMenuPanel.SetActive(false);
        classSelectionPanel.SetActive(true);
        settingsPanel.SetActive(false);
        confirmPanel.SetActive(false);

        selectedClass = "";
        confirmButton.gameObject.SetActive(false);
        backToSelectionButton.gameObject.SetActive(false);
        selectedClassText.text = "Wybierz swoją klasę";
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        classSelectionPanel.SetActive(false);
        settingsPanel.SetActive(true);
        confirmPanel.SetActive(false);
    }

    public void SelectClass(string className)
    {
        selectedClass = className;
        selectedClassText.text = "Wybrana klasa: " + className;
        confirmButton.gameObject.SetActive(true);
        backToSelectionButton.gameObject.SetActive(true);
    }

    public void ResetClassSelection()
    {
        selectedClass = "";
        selectedClassText.text = "Wybierz swoją klasę";
        confirmButton.gameObject.SetActive(false);
        backToSelectionButton.gameObject.SetActive(false);
    }

    public void ConfirmClassSelection()
    {
        if (string.IsNullOrEmpty(selectedClass)) return;
        StartGame();
    }

    public void ShowQuitConfirmation()
    {
        confirmPanel.SetActive(true);
        confirmText.text = "Czy na pewno chcesz opuścić grę?";
    }

    public void StartGame()
    {
        PlayerPrefs.SetString("SelectedClass", selectedClass);
        PlayerPrefs.Save();
        SceneManager.LoadScene("TestScene");
    }

    void SetupAmbientMusic()
    {
        if (ambientMusic != null && audioSource != null)
        {
            audioSource.clip = ambientMusic;
            audioSource.loop = true;
            audioSource.volume = 0.4f;
            audioSource.Play();
        }
    }

    public void QuitGame()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}