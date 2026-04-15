using UnityEngine;

public class GameController : MonoBehaviour
{
    public HouseState house;
    public Light sceneLight;
    public UIController ui;

    private bool finished = false;

    private float lastProgress = -1f;

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
        sceneLight.color = Color.yellow;
    }

    void UpdateWorldVisuals(float progress)
    {

        sceneLight.intensity = Mathf.Lerp(0.5f, 2f, progress);
        sceneLight.color = Color.Lerp(Color.blue, Color.yellow, progress);
    }
}