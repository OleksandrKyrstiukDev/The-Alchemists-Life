using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SleepSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup sleepPanel;
    [SerializeField] private TextMeshProUGUI dayText;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float sleepDuration = 2f;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerController;

    private bool sleeping;

    private void Start()
    {
        RefreshUI();
    }
    public void StartSleep()
    {
        if (sleeping)
            return;

        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        sleeping = true;

        if (playerController != null)
            playerController.enabled = false;

        yield return Fade(0f, 1f);

        Sleep();

        dayText.text = $"Δενό {GameManager.Instance.CurrentDay}";

        yield return new WaitForSeconds(sleepDuration);

        yield return Fade(1f, 0f);

        if (playerController != null)
            playerController.enabled = true;

        sleeping = false;
    }

    public void Sleep()
    {
        Debug.Log("[SLEEP] Sleep() called");

        if (GameManager.Instance == null)
        {
            Debug.LogError("[SLEEP] GameManager.Instance == NULL");
            return;
        }

        if (sleepPanel == null)
        {
            Debug.LogError("[SLEEP] sleepPanel is NULL");
            return;
        }

        if (dayText == null)
        {
            Debug.LogError("[SLEEP] dayText is NULL");
            return;
        }

        Debug.Log("[SLEEP] All references OK");

        DayManager.Instance.BeginNight();

        GameManager.Instance.NextDay();

        DayManager.Instance.StartNewDay();
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;

            sleepPanel.alpha = Mathf.Lerp(
                from,
                to,
                t / fadeDuration
            );

            yield return null;
        }

        sleepPanel.alpha = to;
    }

    public void RefreshUI()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[UI] GameManager missing");
            return;
        }

        dayText.text = $"Δενό {GameManager.Instance.CurrentDay}";
    }
}