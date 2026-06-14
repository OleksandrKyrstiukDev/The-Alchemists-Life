using UnityEngine;

[ExecuteAlways]
public class DayTime : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Gradient directionalLightGradient;
    [SerializeField] private Gradient ambientLightGradient;

    [Header("Intensity")]
    [SerializeField] private AnimationCurve directionalIntensityCurve;
    [SerializeField] private AnimationCurve ambientIntensityCurve;

    [Header("References")]
    [SerializeField] private Light dirLight;

    [Header("Night Settings")]
    [SerializeField] private bool disableShadowsAtNight = true;
    [SerializeField] private float nightThreshold = 0.95f;

    [Header("Debug")]
    [SerializeField, Range(0f, 1f)]
    private float timeProgress = 0.15f;

    private Vector3 defaultAngles;

    private void Start()
    {
        if (dirLight != null)
            defaultAngles = dirLight.transform.localEulerAngles;

        ApplyLighting();
    }

    private void OnValidate()
    {
        ApplyLighting();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            ApplyLighting();
#endif
    }

    public void SetPhase(DayPhase phase)
    {
        switch (phase)
        {
            case DayPhase.Morning:
                SetTime(0.15f);
                break;

            case DayPhase.Work:
                SetTime(0.50f);
                break;

            case DayPhase.Evening:
                SetTime(0.75f);
                break;

            case DayPhase.Night:
                SetTime(1.00f);
                break;
        }

        Debug.Log($"[DayTime] Phase -> {phase}");
    }

    public void SetTime(float progress)
    {
        timeProgress = Mathf.Clamp01(progress);

        ApplyLighting();

        Debug.Log($"[DayTime] TimeProgress -> {timeProgress:0.00}");
    }

    public float GetTimeProgress()
    {
        return timeProgress;
    }

    private void ApplyLighting()
    {
        if (dirLight == null)
            return;

        //  ол≥р сонц€
        dirLight.color =
            directionalLightGradient.Evaluate(timeProgress);

        // яскрав≥сть сонц€
        dirLight.intensity =
            directionalIntensityCurve.Evaluate(timeProgress);

        // Ambient кол≥р
        RenderSettings.ambientLight =
            ambientLightGradient.Evaluate(timeProgress);

        // Ambient €скрав≥сть
        RenderSettings.ambientIntensity =
            ambientIntensityCurve.Evaluate(timeProgress);

        // ѕоворот сонц€
        dirLight.transform.localEulerAngles =
            new Vector3(
                360f * timeProgress - 90f,
                defaultAngles.y,
                defaultAngles.z
            );

        // “≥н≥ вноч≥
        if (disableShadowsAtNight)
        {
            dirLight.shadows =
                timeProgress >= nightThreshold
                    ? LightShadows.None
                    : LightShadows.Soft;
        }
    }
}

public enum DayPhase
{
    Morning,
    Work,
    Evening,
    Night
}