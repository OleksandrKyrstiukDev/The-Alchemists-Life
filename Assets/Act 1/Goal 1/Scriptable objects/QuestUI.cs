using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public Transform container;
    public TaskUI taskPrefab;

    private List<TaskUI> taskUIs = new List<TaskUI>();
    private Objective currentObjective;
    private void Start()
    {
        Debug.Log("QuestUI Start");
    }
    private void OnEnable()
    {
        QuestManager.OnTaskProgress += UpdateUI;
    }

    private void OnDisable()
    {
        QuestManager.OnTaskProgress -= UpdateUI;
    }
    public void BuildUI(Objective objective)
    {
        currentObjective = objective;
        taskUIs.Clear();

        foreach (Transform child in container)
            Destroy(child.gameObject);

        foreach (var step in objective.steps)
        {
            var ui = Instantiate(taskPrefab, container);
            ui.Setup(step);
            taskUIs.Add(ui);
        }
    }
    void UpdateUI(string taskId)
    {
        for (int i = 0; i < currentObjective.steps.Length; i++)
        {
            var step = currentObjective.steps[i];

            if (step.data.id == taskId)
            {
                taskUIs[i].UpdateUI(step);
            }
        }
    }
}