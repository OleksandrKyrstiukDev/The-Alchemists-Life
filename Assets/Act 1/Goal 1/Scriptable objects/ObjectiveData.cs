using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Objective")]
public class ObjectiveData : ScriptableObject
{
    public string title;
    public TaskStepData[] steps;
}