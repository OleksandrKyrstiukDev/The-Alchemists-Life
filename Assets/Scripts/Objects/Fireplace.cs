using UnityEngine;

public class Fireplace : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private HeatingReaction heatingReaction;
    [SerializeField] private QuestInteractable questInteractable;

    [Header("Visuals")]
    [SerializeField] private Renderer fireRenderer;
    [SerializeField] private Light fireLight;

    [Header("Colors")]
    [SerializeField] private Color perfectFireColor = new Color(1f, 0.55f, 0.2f);
    [SerializeField] private Color goodFireColor = new Color(1f, 0.35f, 0.15f);
    [SerializeField] private Color failFireColor = new Color(0.9f, 0.1f, 0.05f);
    [SerializeField] private Color offColor = Color.black;

    [Header("Intensity")]
    [SerializeField] private float perfectIntensity = 2.5f;
    [SerializeField] private float goodIntensity = 1.8f;
    [SerializeField] private float failIntensity = 1.0f;
    [SerializeField] private float offIntensity = 0f;

    public HouseState house;

    void Awake()
    {
        Debug.Log($"[Fireplace] HeatingReaction: {heatingReaction}");
        if (heatingReaction == null)
            heatingReaction = GetComponent<HeatingReaction>();
    }

    void OnEnable()
    {
        Debug.Log("[Fireplace] Subscribed to HeatingReaction");
        if (heatingReaction != null)
            heatingReaction.OnStateChanged += OnHeatingChanged;
    }

    void OnDisable()
    {
        if (heatingReaction != null)
            heatingReaction.OnStateChanged -= OnHeatingChanged;
    }

    void OnHeatingChanged(HeatingState state)
    {
        Debug.Log($"[Fireplace] State: {state}");

        switch (state)
        {
            case HeatingState.Perfect:
                ApplyFire(perfectFireColor, perfectIntensity);
                house.warmth += 0.5f;
                questInteractable.Interact();
                break;

            case HeatingState.Stable:
                ApplyFire(goodFireColor, goodIntensity);
                house.warmth += 0.3f;
                questInteractable.Interact();
                break;

            case HeatingState.Unstable:
                ApplyFire(failFireColor, failIntensity);
                house.warmth += 0.1f;
                break;

            case HeatingState.Off:
                ApplyFire(offColor, offIntensity);
                break;
        }
    }

    void ApplyFire(Color color, float intensity)
    {
        if (fireRenderer != null)
        {
            var mat = fireRenderer.material;

            Debug.Log($"Shader: {mat.shader.name}");

            mat.color = color;

            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", color * 1.2f);
            }
        }

        if (fireLight != null)
        {
            fireLight.color = color;
            fireLight.intensity = intensity;
        }
    }
}