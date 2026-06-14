using UnityEngine;
public class PotionObject : MonoBehaviour
{
    public BrewedPotionData Data { get; private set; }  

    [SerializeField] private Color perfectColor = new Color(1f, 0.6f, 0.3f);
    [SerializeField] private Color goodColor = new Color(1f, 0.45f, 0.2f);
    [SerializeField] private Color failColor = Color.red;

    [SerializeField] private Renderer liquidRenderer;
    public PotionData data;
    public void Init(BrewedPotionData data)
    {
        Data = data;

        LogPotionData();



        ApplyColor(data.result);

    }
    void ApplyColor(BrewResult result)
    {
        Color c = result switch
        {
            BrewResult.Perfect => perfectColor,
            BrewResult.Good => goodColor,
            BrewResult.Fail => failColor,
            _ => Color.gray
        };

        var block = new MaterialPropertyBlock();

        liquidRenderer.GetPropertyBlock(block);

        block.SetColor("_BaseColor", c);
        block.SetColor("_Color", c);
        block.SetColor("_EmissionColor", c * 0.5f);

        liquidRenderer.SetPropertyBlock(block);

        Debug.Log($"[PotionObject] Liquid color applied: {c}");
    }

    void LogPotionData()
    {
        if (Data.name == null)
        {
            Debug.LogWarning("[PotionObject] Data is NULL");
            return;
        }

        Debug.Log(
            $"[PotionObject]\n" +
            $"Name: {Data.name}\n" +
            $"Result: {Data.result}\n" +
            $"Purpose: {Data.purpose}\n" +
            $"Stability: {Data.stability:0.00}\n" +
            $"Toxicity: {Data.toxicity:0.00}"
        );
    }

}