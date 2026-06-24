using UnityEngine;

public class PanelController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject ordersPanel;
    [SerializeField] private GameObject storagePanel;

    private GameObject currentPanel;
    private bool isOpen;

    public void OpenOrders()
    {
        OpenPanel(ordersPanel);
    }

    public void OpenStorage()
    {
        OpenPanel(storagePanel);
    }

    private void OpenPanel(GameObject panel)
    {
        if (panel == null) return;

        if (currentPanel == panel && isOpen)
        {
            CloseAll();
            return;
        }

        CloseAll();

        currentPanel = panel;
        currentPanel.SetActive(true);
        isOpen = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 🔥 ADD THIS
        Canvas.ForceUpdateCanvases();
    }

    public void CloseAll()
    {
        ordersPanel?.SetActive(false);
        storagePanel?.SetActive(false);

        currentPanel = null;
        isOpen = false;

    }
}