using TMPro;
using UnityEngine;

public class ChestItemButton : MonoBehaviour
{
    public TMP_Text text;

    private ChestItem item;
    private Chest chest;

    public void Setup(ChestItem item, Chest chest)
    {
        this.item = item;
        this.chest = chest;

        text.text = GetLabel(item);
    }

    string GetLabel(ChestItem item)
    {
        switch (item.type)
        {
            case ChestItemType.Bandage:
                return $"Banda¿ x{item.amount}";
            case ChestItemType.Antidote:
                return $"Odtrutka x{item.amount}";
            case ChestItemType.Coins:
                return $"Gold x{item.amount}";
            default:
                return "Item";
        }
    }

    public void OnClick()
    {
        chest.TakeItem(item);
    }
}