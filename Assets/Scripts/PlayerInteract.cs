using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 3f;
    public LayerMask interactableLayer;

    private Camera playerCamera;
    private IInteractable currentTarget;

    void Start()
    {
        playerCamera = Camera.main;

        //  AUTOMATYCZNE USTAWIENIE WARSTWY
        if (interactableLayer.value == 0)
        {
            int layer = LayerMask.NameToLayer("Interactable");
            if (layer >= 0)
            {
                interactableLayer = 1 << layer;
                Debug.Log("Interactable layer set automatically");
            }
            else
            {
                Debug.LogWarning("Layer 'Interactable' does not exist!");
            }
        }
    }

    void Update()
    {
        if (UIManager.Instance == null)
            return;

        if (UIManager.Instance.BlockInteractThisFrame)
            return;

        if (UIManager.Instance.IsAnyUIOpen)
            return;

        CheckForInteractable();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentTarget = interactable;
                UIManager.Instance.ShowInteractionPrompt(true, interactable.GetPromptText());
                return;
            }
        }

        currentTarget = null;
        UIManager.Instance.ShowInteractionPrompt(false);
    }
}