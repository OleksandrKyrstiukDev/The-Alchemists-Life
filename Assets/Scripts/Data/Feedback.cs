using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FeedbackTextSet
{
    public BrewMistakeType type;

    [Header("Low severity (0–0.4)")]
    [TextArea] public List<string> mild;

    [Header("Medium severity (0.4–0.7)")]
    [TextArea] public List<string> medium;

    [Header("High severity (0.7–1.0)")]
    [TextArea] public List<string> severe;
}

public enum BrewMistakeType
{
    Underheated,
    Overheated,
    Understirred,
    Overstirred,
    UnstableIngredients,
    ToxicMix,
    RushedPrep,
    OvercookedPrep
}

public struct BrewFeedback
{
    public BrewMistakeType type;
    public float severity; 
}

public struct BrewResultData
{
    public BrewResult result;
    public List<BrewFeedback> feedback;
}

public class Feedback : MonoBehaviour
{
    [Header("Feedback Texts")]
    [SerializeField] private List<FeedbackTextSet> feedbackTexts;

    private Dictionary<BrewMistakeType, FeedbackTextSet> lookup;

    private void Awake()
    {
        lookup = new Dictionary<BrewMistakeType, FeedbackTextSet>();
        foreach (var set in feedbackTexts)
        {
            if (!lookup.ContainsKey(set.type))
                lookup.Add(set.type, set);
        }
    }

    // ===============================
    // PUBLIC API
    // ===============================

    public string GetText(BrewFeedback feedback)
    {
        if (!lookup.TryGetValue(feedback.type, out var set))
            return GetFallback(feedback);

        List<string> pool =
            feedback.severity < 0.4f ? set.mild : feedback.severity < 0.7f ? set.medium : set.severe;

        if (pool == null || pool.Count == 0)
            return GetFallback(feedback);

        return pool[Random.Range(0, pool.Count)];
    }

    // ===============================
    // FALLBACK (якщо в інспекторі порожньо)
    // ===============================

    private string GetFallback(BrewFeedback f)
    {
        return f.type switch
        {
            BrewMistakeType.Underheated => "Зілля не встигло достатньо прогрітися",
            BrewMistakeType.Overheated => "Зілля перегрілося",
            BrewMistakeType.Understirred => "Суміш була замало перемішана",
            BrewMistakeType.Overstirred => "Надмірне помішування зруйнувало структуру",
            BrewMistakeType.UnstableIngredients => "Інгредієнти взаємодіяли нестабільно",
            BrewMistakeType.ToxicMix => "Суміш вийшла небезпечною",
            BrewMistakeType.RushedPrep => "Підготовка була надто поспішною",
            BrewMistakeType.OvercookedPrep => "Інгредієнти були перетримані",
            _ => "Щось пішло не так"
        };
    }
}