using UnityEngine;

public class QuestInteractable : MonoBehaviour
{
    public string taskId;
    public int amount = 1;

    [Header("Potion Requirement")]
    public PotionPurpose requiredPotion;


    public void Interact()
    {
        FindFirstObjectByType<QuestManager>()
            .ProgressTask(taskId, amount);
    }


    public bool CheckPotion(BrewedPotionData potion)
    {
        return potion.purpose == requiredPotion;
    }
}