using UnityEngine;

public class GameController : MonoBehaviour
{
    public HouseState house;
    public Light sceneLight;
    public UIController ui;

    private bool finished = false;

    // щоб не оновлювати UI без змін
    private float lastProgress = -1f;

    void Update()
    {
        float progress = house.GetProgress();

        // оновлюємо UI тільки якщо є зміна
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

        // фінальний буст світла
        sceneLight.intensity = 2f;
        sceneLight.color = Color.yellow;
    }

    void UpdateWorldVisuals(float progress)
    {
        // плавна зміна світла залежно від стану будинку
        sceneLight.intensity = Mathf.Lerp(0.5f, 2f, progress);

        // холод → теплий
        sceneLight.color = Color.Lerp(Color.blue, Color.yellow, progress);
    }
}