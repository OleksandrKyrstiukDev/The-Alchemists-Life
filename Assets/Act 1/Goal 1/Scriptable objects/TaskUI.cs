using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TaskUI : MonoBehaviour
{
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI progressText;
    public Image checkmark;

    public void Setup(TaskStep step)
    {
        descriptionText.text = step.data.description;
        UpdateUI(step);
    }

    public void UpdateUI(TaskStep step)
    {
        progressText.text = $"{step.currentAmount}/{step.data.requiredAmount}";
        checkmark.gameObject.SetActive(step.IsComplete);
    }
}