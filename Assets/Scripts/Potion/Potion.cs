using UnityEngine;

public class Potion : MonoBehaviour
{
    public BrewedPotionData data;
    [SerializeField] private Renderer liquidRenderer;

    public void SetLiquidColor(Color color)
    {
        if (liquidRenderer == null)
            return;

        Material mat = liquidRenderer.material;

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", color);

        mat.color = color;

        if (mat.HasProperty("_EmissionColor"))
            mat.SetColor("_EmissionColor", color * 0.5f);
    }
}