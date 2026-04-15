using UnityEngine;

public enum TaskType
{
    Collect,
    Interact,
    PlaceItem,
    Toggle
}

[CreateAssetMenu(menuName = "Quest/Task Step")]
public class TaskStepData : ScriptableObject
{
    public string id;
    public string description;
    public TaskType type;
    public int requiredAmount;
}