using System;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestData questData;
    public static event Action<string> OnTaskProgress;
    private Objective[] objectives;
    public QuestUI questUI;

    private void Start()
    {
        objectives = questData.objectives
            .Select(o => new Objective(o))
            .ToArray();

        questUI.BuildUI(objectives[0]); 
    }

    public void ProgressTask(string taskId, int amount = 1)
    {
        foreach (var obj in objectives)
        {
            foreach (var step in obj.steps)
            {
                if (step.data.id == taskId)
                {
                    step.Progress(amount);

                    OnTaskProgress?.Invoke(taskId);

                    Debug.Log($"Progress: {step.data.description} {step.currentAmount}/{step.data.requiredAmount}");
                }
            }
        }
    }
}