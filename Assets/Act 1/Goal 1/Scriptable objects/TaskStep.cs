using UnityEngine;

public class TaskStep
{
    public TaskStepData data;
    public int currentAmount;
    public bool IsComplete => currentAmount >= data.requiredAmount;

    public TaskStep(TaskStepData data)
    {
        this.data = data;
        currentAmount = 0;
    }

    public void Progress(int amount = 1)
    {
        currentAmount += amount;
    }
}