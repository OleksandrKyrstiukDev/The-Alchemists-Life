using UnityEngine;

public class CauldronVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Cauldron cauldron;
    [SerializeField] private CauldronUI cauldronUI;

    [Header("Liquid Renderer")]
    [SerializeField] private Renderer liquidRenderer;

    [Header("Shader Properties")]
    [SerializeField] private string colorProperty = "_Color";
    [SerializeField] private string boarderColorProperty = "_Boarder_Color"; // Назва властивості обводки в шейдері
    [SerializeField] private float lerpSpeed = 5f; // Швидкість плавної зміни кольорів

    [Header("Liquid Base Colors")]
    [SerializeField] private Color idleColor = Color.gray;
    [SerializeField] private Color prepColor = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private Color greenZoneColor = Color.green;
    [SerializeField] private Color yellowZoneColor = Color.yellow;
    [SerializeField] private Color redZoneColor = Color.red;

    [Header("Liquid Boarder Colors")]
    [SerializeField] private Color idleBoarderColor = Color.darkGray;
    [SerializeField] private Color prepBoarderColor = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] private Color greenZoneBoarderColor = new Color(0f, 1f, 0.5f); // Світло-зелений або бірюзовий бортик
    [SerializeField] private Color yellowZoneBoarderColor = new Color(1f, 0.8f, 0f);  // Помаранчево-жовтий бортик
    [SerializeField] private Color redZoneBoarderColor = new Color(1f, 0f, 0.2f);     // Яскраво-червоний / палаючий бортик

    private Material liquidMat;
    private Color currentLiquidColor;
    private Color currentBoarderColor;

    private void Awake()
    {
        if (liquidRenderer != null)
        {
            liquidMat = liquidRenderer.material;

            // Задаємо початковий колір рідини
            if (liquidMat.HasProperty(colorProperty))
                currentLiquidColor = liquidMat.GetColor(colorProperty);
            else
                currentLiquidColor = idleColor;

            // Задаємо початковий колір обводки
            if (liquidMat.HasProperty(boarderColorProperty))
                currentBoarderColor = liquidMat.GetColor(boarderColorProperty);
            else
                currentBoarderColor = idleBoarderColor;
        }
    }

    private void Update()
    {
        if (cauldron == null || cauldronUI == null || liquidMat == null)
            return;

        UpdateCauldronVisuals();
    }

    private void UpdateCauldronVisuals()
    {
        Color targetLiquidColor = idleColor;
        Color targetBoarderColor = idleBoarderColor;

        float temp = cauldronUI.Temperature;

        switch (cauldron.CurrentPhase)
        {
            case CauldronPhase.Idle:
                targetLiquidColor = idleColor;
                targetBoarderColor = idleBoarderColor;
                break;

            case CauldronPhase.Prep:
                if (temp > 20f && temp < 60f)
                {
                    targetLiquidColor = prepColor;
                    targetBoarderColor = prepBoarderColor;
                }
                else if (temp >= 60f)
                {
                    targetLiquidColor = yellowZoneColor;
                    targetBoarderColor = yellowZoneBoarderColor;
                }
                else
                {
                    targetLiquidColor = idleColor;
                    targetBoarderColor = idleBoarderColor;
                }
                break;

            case CauldronPhase.Brew:
                // 🟢 perfect zone
                if (temp >= 60f && temp <= 70f)
                {
                    targetLiquidColor = greenZoneColor;
                    targetBoarderColor = greenZoneBoarderColor;
                }
                // 🟡 warning zone
                else if ((temp > 50f && temp < 60f) || (temp > 70f && temp < 80f))
                {
                    targetLiquidColor = yellowZoneColor;
                    targetBoarderColor = yellowZoneBoarderColor;
                }
                // 🔴 overheated
                else if (temp >= 80f)
                {
                    targetLiquidColor = redZoneColor;
                    targetBoarderColor = redZoneBoarderColor;
                }
                else
                {
                    targetLiquidColor = idleColor;
                    targetBoarderColor = idleBoarderColor;
                }
                break;
        }

        // Плавна інтерполяція для обох кольорів одночасно
        currentLiquidColor = Color.Lerp(currentLiquidColor, targetLiquidColor, Time.deltaTime * lerpSpeed);
        currentBoarderColor = Color.Lerp(currentBoarderColor, targetBoarderColor, Time.deltaTime * lerpSpeed);

        // Відправляємо оновлені дані в шейдер
        liquidMat.SetColor(colorProperty, currentLiquidColor);
        liquidMat.SetColor(boarderColorProperty, currentBoarderColor);
    }
}