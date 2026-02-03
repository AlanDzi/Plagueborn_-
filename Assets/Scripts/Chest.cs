using UnityEngine;
using System.Collections.Generic;

public class Chest : MonoBehaviour, IInteractable
{
    public string promptText = "Naciœnij E, aby otworzyæ";
    public List<ChestItem> items = new List<ChestItem>();

    private InventoryManager inventory;

    private void Start()
    {
        inventory = FindFirstObjectByType<InventoryManager>();
    }

    public string GetPromptText()
    {
        return promptText;
    }

    public void Interact()
    {
        if (UIManager.Instance == null)
            return;

        if (UIManager.Instance.IsAnyUIOpen)
            return;

        if (UIManager.Instance.BlockInteractThisFrame)
            return;

        UIManager.Instance.ShowChest(this);
    }

    public void TakeItem(ChestItem item)
    {
        if (inventory == null)
            return;

        bool added = false;

        switch (item.type)
        {
            case ChestItemType.Bandage:
                added = inventory.AddBandage(item.amount);
                break;

            case ChestItemType.Antidote:
                added = inventory.AddAntidote(item.amount);
                break;

            case ChestItemType.Coins:
                inventory.AddCoins(item.amount);
                added = true;
                break;
        }

        if (!added)
            return;

        items.Remove(item);
        UIManager.Instance.RefreshChest(this);
    }
}