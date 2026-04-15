using System.Collections.Generic;
using UnityEngine;

public static class CauldronProcess
{
    public static BrewResult Evaluate(
    List<IngredientData> ingredients,
    List<BrewFeedback> feedback,
    BrewingPhase prepPhase,
    BrewingPhase brewPhase,
    float temperature,
    int stirCount,
    float greenTime,
    float yellowTime,
    float redTime,
    PrepResult prep,
    float extraPenalty,
    float recipeConfidence
)
    {

        if (ingredients == null || ingredients.Count == 0)
            return BrewResult.Fail;

        if (feedback == null)
            return BrewResult.Fail;

        feedback.Clear(); 

        float stability = 0f;
        float toxicity = 0f;

        foreach (var ing in ingredients)
        {
            stability += ing.stability;
            toxicity += ing.toxicity;
        }

        stability /= ingredients.Count;
        toxicity /= ingredients.Count;

        if (brewPhase == null)
        {
            Debug.LogWarning("[CauldronProcess] brewPhase == null, використання стандартних значень");
            brewPhase = new BrewingPhase
            {
                optimalTemperature = 50f,
                temperatureTolerance = 10f,
                optimalStirCount = 3,
                stirTolerance = 1
            };
        }

        float tempTarget = brewPhase.optimalTemperature + prep.temperatureBias;
        float tempDev =
            Mathf.Abs(temperature - tempTarget) /
            brewPhase.temperatureTolerance;

        float stirTarget = brewPhase.optimalStirCount + prep.stirBias;
        float stirDev =
            Mathf.Abs(stirCount - stirTarget) /
            brewPhase.stirTolerance;
        Debug.Log(
    $"[RISK] Base deviations | " +
    $"TempDev={tempDev:0.00} (T={temperature:0.0}, Target={tempTarget:0.0}) | " +
    $"StirDev={stirDev:0.00} (Count={stirCount}, Target={stirTarget})"
);

        if (temperature < tempTarget - brewPhase.temperatureTolerance)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.Underheated,
                severity = Mathf.Clamp01(tempDev)
            });
        }
        else if (temperature > tempTarget + brewPhase.temperatureTolerance)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.Overheated,
                severity = Mathf.Clamp01(tempDev)
            });
        }

        if (stirCount < stirTarget)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.Understirred,
                severity = Mathf.Clamp01(stirDev)
            });
        }
        else if (stirCount > stirTarget)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.Overstirred,
                severity = Mathf.Clamp01(stirDev)
            });
        }

        float effectiveStability =
    (stability + prep.stabilityBonus) * recipeConfidence;

        if (effectiveStability < 1.0f && recipeConfidence < 0.99f)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.UnstableIngredients,
                severity = Mathf.Clamp01(1f - effectiveStability)
            });
        }


        if (toxicity > 1f)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.ToxicMix,
                severity = Mathf.Clamp01(toxicity - 0.7f)
            });
        }

        float prepDelta = Mathf.Abs(prep.prepTime - prepPhase.optimalTime);
        float prepMax = prepPhase.timeTolerance * 2f;
        float prepSeverity = Mathf.Clamp01(prepDelta / prepMax);

        if (prep.prepTime < prepPhase.optimalTime - prepPhase.timeTolerance)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.RushedPrep,
                severity = prepSeverity
            });
        }
        else if (prep.prepTime > prepPhase.optimalTime + prepPhase.timeTolerance)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.OvercookedPrep,
                severity = prepSeverity
            });
        }

        float risk = tempDev * 1.2f + stirDev * 0.5f;
        Debug.Log(
    $"[RISK] Initial risk = {risk:0.00} " +
    $"(temp * 1.2 + stir * 0.5)"
);
        float prepMultiplier = Mathf.Lerp(0.7f, 1.5f, prepSeverity);

        Debug.Log(
            $"[RISK] Prep multiplier = {prepMultiplier:0.00} " +
            $"(prepTime={prep.prepTime:0.0}, optimal={prepPhase.optimalTime:0.0}, " +
            $"severity={prepSeverity:0.00})"
        );

        risk *= prepMultiplier;

        Debug.Log($"[RISK] After prep = {risk:0.00}");


        float stabilityMultiplier = Mathf.Lerp(1.3f, 0.7f, stability);

        Debug.Log(
            $"[RISK] Stability multiplier = {stabilityMultiplier:0.00} " +
            $"(stability={stability:0.00})"
        );

        risk *= stabilityMultiplier;

        Debug.Log($"[RISK] After stability = {risk:0.00}");

        bool extremeHeat =
     temperature > brewPhase.optimalTemperature +
     brewPhase.temperatureTolerance * 2.5f;

        bool extremeToxic =
            toxicity > 1.5f;

        if (extremeHeat || extremeToxic)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.Overheated,
                severity = 1f
            });
            return BrewResult.Explode;
        }

        if (extraPenalty >= 0.2f)
        {
            feedback.Add(new BrewFeedback
            {
                type = BrewMistakeType.ToxicMix,
                severity = Mathf.Clamp01(extraPenalty)
            });

            if (extraPenalty < 0.8f)
            {
                if (risk < 0.5f) risk = 1.0f;
                else risk += 0.5f;
            }
        }

        BrewResult result;
        if (risk < 0.5f) result = BrewResult.Perfect;
        else if (risk < 1.2f) result = BrewResult.Good;
        else if (risk < 2.0f) result = BrewResult.Fail;
        else result = BrewResult.Explode;
        Debug.Log(
    $"[RISK FINAL] risk={risk:0.00} → result={result} | " +
    $"feedbackCount={feedback.Count}"
);

        switch (result)
        {
            case BrewResult.Perfect:
                feedback.Clear();
                break;

            case BrewResult.Good:
                feedback.RemoveAll(f => f.severity > 0.5f);
                Debug.Log(
    $"[STABILITY DEBUG] avg={stability:0.00} bonus={prep.stabilityBonus:0.00} " +
    $"confidence={recipeConfidence:0.00} effective={(stability + prep.stabilityBonus) * recipeConfidence:0.00}"
);
                break;

            case BrewResult.Fail:
                feedback.RemoveAll(f => f.severity < 0.3f);
                break;

            case BrewResult.Explode:
                feedback.RemoveAll(f => f.severity < 0.7f);
                break;
        }

        return result;
    }
}