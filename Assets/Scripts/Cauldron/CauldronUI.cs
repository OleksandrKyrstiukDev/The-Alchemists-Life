using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CauldronUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Cauldron cauldron;

    [Header("Canvas")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject stir;

    [Header("Temperature UI")]
    [SerializeField] private Image temperatureBar;
    [SerializeField] private float minTemp = 0f;
    [SerializeField] private float maxTemp = 100f;

    [Header("Time UI (stacked)")]
    [SerializeField] private Image greenTimeBar;
    [SerializeField] private Image yellowTimeBar;
    [SerializeField] private Image redTimeBar;

    [Header("Stir UI")]
    [SerializeField] private TextMeshProUGUI stirText;

    [Header("Feedback")]
    [SerializeField] private Feedback feedbackSystem;
    [SerializeField] private Image feedBackPanel;
    [SerializeField] private TextMeshProUGUI feedback;

    // ===============================
    // Runtime values
    // ===============================

    public float Temperature { get; private set; } = 20f;
    public int StirCount { get; private set; }

    private bool brewingStarted;

    // --- preparation ---
    public float PrepTime { get; private set; }
    private float prepTempAccum;
    private float prepTempTime;

    public float AveragePrepTemperature => prepTempTime > 0f ? prepTempAccum / prepTempTime : Temperature;


    // --- brewing zones ---
    private float greenTime;
    private float yellowTime;
    private float redTime;

    // ===============================
    // Unity
    // ===============================

    private void Awake()
    {
        if (cauldron != null)
            cauldron.RegisterUI(this);

        if (panel != null)
            panel.SetActive(false);

        if (stir != null)
            stir.SetActive(false);
    }

    private void Update()
    {
        if (!brewingStarted) return;

        float dt = Time.deltaTime;

        UpdateTemperature(dt);

        cauldron.UpdatePhase(Temperature);

        AccumulatePrep(dt);
        AccumulateBrewZones(dt);

        UpdateVisuals();
    }


    // ===============================
    // Core logic
    // ===============================

    private void UpdateTemperature(float dt)
    {
        Temperature -= dt; // охолодження
        Temperature = Mathf.Max(0f, Temperature);
    }


    private void AccumulatePrep(float dt)
    {
        if (cauldron.CurrentPhase != CauldronPhase.Prep)
            return;

        PrepTime += dt;
        prepTempAccum += Temperature * dt;
        prepTempTime += dt;
    }


    private void AccumulateBrewZones(float dt)
    {
        // ПОКИ ХАРДКОД (наступний крок — винести в рецепт)
        if (Temperature >= 60f && Temperature <= 70f)
            greenTime += dt;
        else if ((Temperature > 50f && Temperature < 60f) ||
                 (Temperature > 70f && Temperature < 80f))
            yellowTime += dt;
        else if (Temperature >= 80f)
            redTime += dt;
    }

    // ===============================
    // UI
    // ===============================

    private void UpdateVisuals()
    {
        float t = Mathf.InverseLerp(minTemp, maxTemp, Temperature);
        temperatureBar.fillAmount = t;

        switch (cauldron.CurrentPhase)
        {
            case CauldronPhase.Idle:
                temperatureBar.color = Color.gray;
                break;

            case CauldronPhase.Prep:
                temperatureBar.color = Color.blue;
                break;

            case CauldronPhase.Brew:
                temperatureBar.color = Temperature >= 80f ? Color.red : Color.yellow;
                break;
        }

        greenTimeBar.fillAmount = Mathf.Clamp01(greenTime / 10f);
        yellowTimeBar.fillAmount = Mathf.Clamp01(yellowTime / 10f);
        redTimeBar.fillAmount = Mathf.Clamp01(redTime / 10f);

        stirText.text = StirCount.ToString();
    }

    // ===============================
    // Player actions
    // ===============================

    public void Heat()
    {
        if (!brewingStarted)
        {
            brewingStarted = true;

            if (panel != null)
                panel.SetActive(true);

            if (stir != null)
                stir.SetActive(true);

            cauldron.OnBrewingStarted();
        }

        Temperature += 5f;
    }
    public void Stir()
    {
        if (!brewingStarted)
            brewingStarted = true;

        StirCount++;
    }

    public void Finish()
    {
        Debug.Log(
            $"[UI] Finish Brew | Temp={Temperature:0} | " +
            $"Green={greenTime:0.0} | Yellow={yellowTime:0.0} | " +
            $"Red={redTime:0.0} | Stirs={StirCount}"
        );

        cauldron.Brew(
            Temperature,
            StirCount,
            greenTime,
            yellowTime,
            redTime
        );
        ResetUI();
    }

    // ===============================
    // Reset
    // ===============================

    private void ResetUI()
    {
        brewingStarted = false;
        Temperature = 20f;
        StirCount = 0;
       

        PrepTime = 0f;
        prepTempAccum = 0f;
        prepTempTime = 0f;

        greenTime = 0f;
        yellowTime = 0f;
        redTime = 0f;

        temperatureBar.fillAmount = 0f;
        temperatureBar.color = Color.blue;

        greenTimeBar.fillAmount = 0f;
        yellowTimeBar.fillAmount = 0f;
        redTimeBar.fillAmount = 0f;

        stirText.text = "0";
        cauldron.OnBrewingFinished();
        
    }

    public void ShowFeedback(BrewResultData resultData)
    {
        if (feedbackSystem == null || resultData.feedback == null)
            return;

        foreach (var f in resultData.feedback
            .OrderByDescending(f => f.severity)
            .Take(3))
        {
            string text = feedbackSystem.GetText(f);
            feedBackPanel.gameObject.SetActive(true);
            feedback.text = text;
            Debug.Log($"[FEEDBACK] {text}");
        }

   
    }

}
