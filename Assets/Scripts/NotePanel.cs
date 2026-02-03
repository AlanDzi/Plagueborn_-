using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [TextArea(3, 10)]
    public string noteText;

    public string promptText = "Naciœnij E, aby przeczytaæ";

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

        UIManager.Instance.ShowNote(noteText);
    }
}