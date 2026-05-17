using UnityEngine;
using UnityEngine.InputSystem;

public class StorageZone : MonoBehaviour
{
    [SerializeField] private PanelController panelController;

    private bool playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        playerInside = false;
    }

    public void OnPanel(InputValue value)
    {
        if (!value.isPressed) return;

        if (!playerInside) return;

        panelController.OpenStorage();
    }
}