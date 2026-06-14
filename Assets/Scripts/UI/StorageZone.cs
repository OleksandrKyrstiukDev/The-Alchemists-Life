using UnityEngine;

public class StorageZone : MonoBehaviour
{
    [SerializeField] private PanelController panelController;

    public bool PlayerInside { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInside = true;
        Debug.Log("[Storage] Enter");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInside = false;
        Debug.Log("[Storage] Exit");
    }

    public void Open()
    {
        panelController.OpenStorage();
        Debug.Log(panelController);
    }
}