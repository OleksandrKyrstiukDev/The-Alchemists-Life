using UnityEngine;

public class OrdersZone : MonoBehaviour
{
    [SerializeField] private PanelController panelController;

    public bool PlayerInside { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInside = true;
        Debug.Log("[Orders] Enter");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInside = false;
        Debug.Log("[Orders] Exit");
    }

    public void Open()
    {
        panelController.OpenOrders();
    }
}