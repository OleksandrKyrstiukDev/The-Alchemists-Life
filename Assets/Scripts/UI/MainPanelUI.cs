using UnityEngine;

public class MainPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject[] panels;

    private GameObject currentPanel;

    public void OpenPanel(GameObject panelToOpen)
    {
        // Закрити всі
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        // Відкрити потрібну
        panelToOpen.SetActive(true);
        currentPanel = panelToOpen;
    }

    public void CloseAll()
    {
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        currentPanel = null;
    }

    public void TogglePanel(GameObject panel)
    {
        // якщо вже відкрита → закрити
        if (currentPanel == panel)
        {
            panel.SetActive(false);
            currentPanel = null;
            return;
        }

        OpenPanel(panel);
    }
}