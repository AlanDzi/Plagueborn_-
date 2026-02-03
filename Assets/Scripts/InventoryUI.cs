using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("Item UI - Bottom")]
    public Image antidoteIcon;
    public TextMeshProUGUI antidoteCount;
    public TextMeshProUGUI antidoteKey;

    public Image bandageIcon;
    public TextMeshProUGUI bandageCount;
    public TextMeshProUGUI bandageKey;

    [Header("Currency UI - Top Right")]
    public Image shopIcon;
    public TextMeshProUGUI shopKey;
    public Image coinIcon;
    public TextMeshProUGUI coinCount;

    [Header("Shop Panel (ShopCanvas)")]
    public Canvas shopCanvas;
    public GameObject shopPanel;
    public Button shopOpenButton;
    public Button shopCloseButton;
    
    [Header("Shop Items")]
    public Button buyAntidoteButton;
    public TextMeshProUGUI antidotePrice;
    public Button buyBandageButton;
    public TextMeshProUGUI bandagePrice;

    [Header("Prices")]
    public int antidoteCost = 60;
    public int bandageCost = 25;

    [Header("Audio")]
    public AudioClip buttonHoverSound;
    public AudioClip buttonClickSound;

    private AudioSource audioSource;
    private InventoryManager inventoryManager;

    void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (shopCanvas != null)
        {
            shopCanvas.gameObject.SetActive(false);
        }

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        if (antidotePrice != null) antidotePrice.text = antidoteCost + "$";
        if (bandagePrice != null) bandagePrice.text = bandageCost + "$";

        if (antidoteKey != null) antidoteKey.text = "Q";
        if (bandageKey != null) bandageKey.text = "E";
        if (shopKey != null) shopKey.text = "T";

        SetupButtons();
        UpdateInventoryDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleShop();
        }

        UpdateShopButtons();
    }

    void SetupButtons()
    {
        if (shopOpenButton != null)
        {
            shopOpenButton.onClick.AddListener(OpenShop);
            AddButtonEffects(shopOpenButton);
        }

        if (shopCloseButton != null)
        {
            shopCloseButton.onClick.AddListener(CloseShop);
            AddButtonEffects(shopCloseButton);
        }

        if (buyAntidoteButton != null)
        {
            buyAntidoteButton.onClick.AddListener(() => {
                BuyAntidote();
            });
            AddButtonEffects(buyAntidoteButton);
        }

        if (buyBandageButton != null)
        {
            buyBandageButton.onClick.AddListener(() => {
                BuyBandage();
            });
            AddButtonEffects(buyBandageButton);
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

    public void UpdateInventoryDisplay()
    {
        if (inventoryManager == null) return;

        if (antidoteCount != null)
        {
            antidoteCount.text = inventoryManager.antidotes + "/" + inventoryManager.maxItems;
        }

        if (bandageCount != null)
        {
            bandageCount.text = inventoryManager.bandages + "/" + inventoryManager.maxItems;
        }

        if (coinCount != null)
        {
            coinCount.text = inventoryManager.coins + "/" + inventoryManager.maxCoins;
        }
    }

    void UpdateShopButtons()
    {
        if (inventoryManager == null) return;

        if (buyAntidoteButton != null)
        {
            buyAntidoteButton.interactable = inventoryManager.CanBuyAntidote(antidoteCost);
        }

        if (buyBandageButton != null)
        {
            buyBandageButton.interactable = inventoryManager.CanBuyBandage(bandageCost);
        }
    }

    public void OpenShop()
    {
        if (shopCanvas != null)
        {
            shopCanvas.gameObject.SetActive(true);
        }
        
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }
        
        Time.timeScale = 0f;
        
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        WeaponController weaponController = FindFirstObjectByType<WeaponController>();
        
        if (playerController != null) playerController.enabled = false;
        if (weaponController != null) weaponController.enabled = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseShop()
    {
        if (shopCanvas != null)
        {
            shopCanvas.gameObject.SetActive(false);
        }
        
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
        
        Time.timeScale = 1f;
        
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        WeaponController weaponController = FindFirstObjectByType<WeaponController>();
        
        if (playerController != null) playerController.enabled = true;
        if (weaponController != null) weaponController.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ToggleShop()
    {
        if (shopPanel != null)
        {
            bool isActive = shopPanel.activeSelf;
            if (isActive)
            {
                CloseShop();
            }
            else
            {
                OpenShop();
            }
        }
    }

    public void BuyAntidote()
    {
        if (inventoryManager != null)
        {
            inventoryManager.BuyAntidote(antidoteCost);
        }
    }

    public void BuyBandage()
    {
        if (inventoryManager != null)
        {
            inventoryManager.BuyBandage(bandageCost);
        }
    }
}