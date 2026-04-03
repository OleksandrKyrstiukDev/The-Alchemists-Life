using UnityEngine;

public class PotionReactiveObject : MonoBehaviour, IPotionReactive
{
    private PotionReaction[] reactions;

    void Awake()
    {
        reactions = GetComponents<PotionReaction>();
    }

    public void ApplyPotion(BrewedPotionData potion)
    {
        foreach (var reaction in reactions)
        {
            if (reaction.purpose == potion.purpose)
            {
                reaction.React(potion.result);
            }
        }
    }
}
