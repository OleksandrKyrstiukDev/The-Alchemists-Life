using UnityEngine;

public class GameController : MonoBehaviour
{
    public HouseState house;
    public Light sceneLight;
    public UIController ui;

    private bool finished = false;
    private float lastProgress = -1f;

    private Color warmLightColor = new Color(1f, 0.85f, 0.65f); // теплий світ
    private Color finalLightColor = new Color(1f, 0.95f, 0.8f); // майже білий теплий

    void Update()
    {
        float progress = house.GetProgress();

        if (Mathf.Abs(progress - lastProgress) > 0.001f)
        {
            ui.UpdateProgress(progress);
            lastProgress = progress;
        }

        if (!finished && house.IsComplete)
        {
            CompleteGame();
        }

        UpdateWorldVisuals(progress);
    }

    void CompleteGame()
    {
        finished = true;

        sceneLight.intensity = 2f;
        sceneLight.color = finalLightColor;
    }

    void UpdateWorldVisuals(float progress)
    {
        // тільки теплий діапазон
        sceneLight.intensity = Mathf.Lerp(0.5f, 2f, progress);

        sceneLight.color = Color.Lerp(
            warmLightColor,
            finalLightColor,
            progress
        );
    }
}