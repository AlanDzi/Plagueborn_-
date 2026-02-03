using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Note UI")]
    public CanvasGroup noteGroup;
    public TMP_Text noteText;

    [Header("Interaction Prompt")]
    public GameObject interactionPrompt;
    public TMP_Text interactionPromptText;

    [Header("Chest UI")]
    public GameObject chestPanel;
    public Transform chestContent;
    public GameObject chestItemButtonPrefab;

    private bool isNoteOpen = false;
    private bool isChestOpen = false;

    public bool IsAnyUIOpen => isNoteOpen || isChestOpen;

    // blokada jednej klatki po zamknięciu UI
    public bool BlockInteractThisFrame { get; private set; }

    private void Awake()
    {
        Instance = this;

        if (noteGroup != null)
        {
            noteGroup.alpha = 0f;
            noteGroup.interactable = false;
            noteGroup.blocksRaycasts = false;
        }

        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);

        if (chestPanel != null)
            chestPanel.SetActive(false);
    }

    private void Update()
    {
        if (isNoteOpen && Input.GetKeyDown(KeyCode.E))
        {
            CloseNote();
            return;
        }

        if (isChestOpen && Input.GetKeyDown(KeyCode.E))
        {
            CloseChest();
            return;
        }
    }

    private void LateUpdate()
    {
        BlockInteractThisFrame = false;
    }

    // ================= NOTE =================

    public void ShowNote(string text)
    {
        isNoteOpen = true;

        noteGroup.alpha = 1f;
        noteGroup.interactable = true;
        noteGroup.blocksRaycasts = true;

        noteText.text = text;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseNote()
    {
        isNoteOpen = false;
        BlockInteractThisFrame = true;

        noteGroup.alpha = 0f;
        noteGroup.interactable = false;
        noteGroup.blocksRaycasts = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ================= CHEST =================

    public void ShowChest(Chest chest)
    {
        isChestOpen = true;

        chestPanel.SetActive(true);
        interactionPrompt.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        RefreshChest(chest);
    }

    public void CloseChest()
    {
        isChestOpen = false;
        BlockInteractThisFrame = true;

        chestPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RefreshChest(Chest chest)
    {
        foreach (Transform child in chestContent)
            Destroy(child.gameObject);

        foreach (var item in chest.items)
        {
            var btn = Instantiate(chestItemButtonPrefab, chestContent);
            btn.GetComponent<ChestItemButton>().Setup(item, chest);
        }
    }

    // ================= PROMPT =================

    public void ShowInteractionPrompt(bool show)
    {
        if (!IsAnyUIOpen && interactionPrompt != null)
            interactionPrompt.SetActive(show);
    }

    public void ShowInteractionPrompt(bool show, string text)
    {
        if (!IsAnyUIOpen && interactionPrompt != null)
        {
            interactionPrompt.SetActive(show);
            interactionPromptText.text = text;
        }
    }
}