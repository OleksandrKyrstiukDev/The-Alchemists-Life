using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject settingsPanel;


    [Header("Scenes")]
    [SerializeField] private string introSceneName = "IntroScene";
    [SerializeField] private string gameSceneName = "Game";


    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SaveSystem.DeleteSave();


        GameSaveData newSave = new GameSaveData
        {
            day = 1,
            gold = 0,
            reputation = 0,
            progress = 0,

            introPlayed = false,

            houseState = new HouseStateData()
        };


        SaveSystem.Save(newSave);

        SceneManager.LoadScene(introSceneName);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }


    public void OpenSettings()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);


        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }


    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainPanel != null)
            mainPanel.SetActive(true);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

#else

        Application.Quit();

#endif
    }

    // =========================
    // LANGUAGE
    // =========================

    public void SetUkrainian()
    {
        if (LocalizationManager.Instance == null)
            return;

        LocalizationManager.Instance.ChangeLanguage(
            Language.Ukrainian
        );

        ReloadScene();
    }

    public void SetEnglish()
    {
        if (LocalizationManager.Instance == null)
            return;

        LocalizationManager.Instance.ChangeLanguage(
            Language.English
        );

        ReloadScene();
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}