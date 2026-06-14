using System.Collections;
using TMPro;
using UnityEngine;

public class PotionPopupUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI potionNameText;

    [SerializeField] private float fadeInDuration = 0.4f;
    [SerializeField] private float visibleDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.4f;

    private Coroutine currentRoutine;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
    }

    public void Show(string potionName)
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(potionName));
    }

    private IEnumerator ShowRoutine(string potionName)
    {
        gameObject.SetActive(true);

        potionNameText.text = potionName;

        // Fade In
        float t = 0f;

        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(visibleDuration);

        // Fade Out
        t = 0f;

        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}