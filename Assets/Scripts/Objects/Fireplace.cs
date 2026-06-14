using System.Collections;
using UnityEngine;

public class Fireplace : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private HeatingReaction heatingReaction;
    [SerializeField] private QuestInteractable questInteractable;

    [Header("VFX Prefab")]
    [SerializeField] private GameObject fireVfxPrefab;

    [Header("Colors")]
    [SerializeField] private Color perfectFireColor = new Color(1f, 0.55f, 0.2f);
    [SerializeField] private Color goodFireColor = new Color(1f, 0.35f, 0.15f);
    [SerializeField] private Color failFireColor = new Color(0.9f, 0.1f, 0.05f);

    [Header("Intensity")]
    [SerializeField] private float perfectIntensity = 2.5f;
    [SerializeField] private float goodIntensity = 1.8f;
    [SerializeField] private float failIntensity = 1.0f;

    [Header("Ignition")]
    [SerializeField] private float igniteDuration = 2f;
    [SerializeField] private float maxEmissionRate = 50f;

    public HouseState house;

    private GameObject fireInstance;
    private ParticleSystem[] fireParticles;
    private Light fireLight;

    private bool fireStarted;

    private void Awake()
    {
        if (heatingReaction == null)
            heatingReaction = GetComponent<HeatingReaction>();

        if (fireVfxPrefab != null)
        {
            fireInstance = Instantiate(fireVfxPrefab, transform);

            // 🔥 ГАРАНТІЯ АКТИВНОСТІ
            fireInstance.SetActive(true);

            fireParticles = fireInstance.GetComponentsInChildren<ParticleSystem>();
            fireLight = fireInstance.GetComponentInChildren<Light>();

            if (fireLight != null)
                fireLight.intensity = 0f;

            foreach (var ps in fireParticles)
            {
                var emission = ps.emission;
                emission.rateOverTime = 0f;
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }

    private void OnEnable()
    {
        if (heatingReaction != null)
            heatingReaction.OnStateChanged += OnHeatingChanged;
    }

    private void OnDisable()
    {
        if (heatingReaction != null)
            heatingReaction.OnStateChanged -= OnHeatingChanged;
    }

    private void OnHeatingChanged(HeatingState state)
    {
        switch (state)
        {
            case HeatingState.Perfect:
                ApplyFire(perfectFireColor, perfectIntensity);

                if (house != null)
                    house.warmth += 0.5f;

                questInteractable?.Interact();
                break;

            case HeatingState.Stable:
                ApplyFire(goodFireColor, goodIntensity);

                if (house != null)
                    house.warmth += 0.3f;

                questInteractable?.Interact();
                break;

            case HeatingState.Unstable:
                ApplyFire(failFireColor, failIntensity);

                if (house != null)
                    house.warmth += 0.1f;
                break;

            case HeatingState.Off:
                // камін не гасне
                break;
        }
    }

    private void ApplyFire(Color color, float intensity)
    {
        if (fireParticles != null)
        {
            foreach (var ps in fireParticles)
            {
                var main = ps.main;
                main.startColor = color;
            }
        }

        if (!fireStarted)
        {
            fireStarted = true;
            StartCoroutine(IgniteFire(color, intensity));
        }
        else
        {
            UpdateFireState(color, intensity);
        }
    }

    private IEnumerator IgniteFire(Color color, float targetIntensity)
    {
        float timer = 0f;

        if (fireParticles != null)
        {
            foreach (var ps in fireParticles)
                ps.Play();
        }

        while (timer < igniteDuration)
        {
            timer += Time.deltaTime;
            float t = timer / igniteDuration;

            float rate = Mathf.Lerp(0f, maxEmissionRate, t);

            for (int i = 0; i < fireParticles.Length; i++)
            {
                var emission = fireParticles[i].emission;
                emission.rateOverTime = rate;
            }

            if (fireLight != null)
            {
                fireLight.color = color;
                fireLight.intensity = Mathf.Lerp(0f, targetIntensity, t);
            }

            yield return null;
        }

        for (int i = 0; i < fireParticles.Length; i++)
        {
            var emission = fireParticles[i].emission;
            emission.rateOverTime = maxEmissionRate;
        }

        if (fireLight != null)
        {
            fireLight.color = color;
            fireLight.intensity = targetIntensity;
        }
    }

    private void UpdateFireState(Color color, float intensity)
    {
        if (fireParticles != null)
        {
            foreach (var ps in fireParticles)
            {
                var main = ps.main;
                main.startColor = color;
            }
        }

        if (fireLight != null)
        {
            fireLight.color = color;
            fireLight.intensity = intensity;
        }
    }
}