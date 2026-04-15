using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Quest")]
public class QuestData : ScriptableObject
{
    public string title;
    public ObjectiveData[] objectives;
}