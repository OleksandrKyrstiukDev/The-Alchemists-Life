using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;


    [Header("Graphics")]
    [SerializeField] private TMP_Dropdown qualityDropdown;


    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;


    [Header("Gameplay")]
    [SerializeField] private Slider sensitivitySlider;


    [Header("FPS")]
    [SerializeField] private Toggle fpsToggle;

    [SerializeField] private Image fpsToggleImage;

    [SerializeField] private Color fpsOnColor = Color.green;
    [SerializeField] private Color fpsOffColor = Color.red;



    [Header("Panels")]
    [SerializeField] private GameObject instructionsPanel;



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }


        Instance = this;

        DontDestroyOnLoad(gameObject);
    }



    private void Start()
    {
        LoadSettings();


        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(SetQuality);


        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(SetMaster);


        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusic);


        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFX);


        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);


        if (fpsToggle != null)
            fpsToggle.onValueChanged.AddListener(SetFPS);


        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }



    // =========================
    // GRAPHICS
    // =========================

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(
            index,
            true
        );


        PlayerPrefs.SetInt(
            "Quality",
            index
        );


        PlayerPrefs.Save();
    }



    // =========================
    // AUDIO
    // =========================


    public void SetMaster(float value)
    {
        value = Mathf.Max(value, 0.0001f);


        audioMixer.SetFloat(
            "MasterVolume",
            Mathf.Log10(value) * 20f
        );


        PlayerPrefs.SetFloat(
            "Master",
            value
        );
    }



    public void SetMusic(float value)
    {
        value = Mathf.Max(value, 0.0001f);


        audioMixer.SetFloat(
            "MusicVolume",
            Mathf.Log10(value) * 20f
        );


        PlayerPrefs.SetFloat(
            "Music",
            value
        );
    }



    public void SetSFX(float value)
    {
        value = Mathf.Max(value, 0.0001f);


        audioMixer.SetFloat(
            "SFXVolume",
            Mathf.Log10(value) * 20f
        );


        PlayerPrefs.SetFloat(
            "SFX",
            value
        );
    }



    // =========================
    // GAMEPLAY
    // =========================


    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat(
            "Sensitivity",
            value
        );


        PlayerPrefs.Save();
    }



    // =========================
    // FPS
    // =========================


    public void SetFPS(bool enabled)
    {

        Application.targetFrameRate =
            enabled ? 60 : -1;



        if (fpsToggleImage != null)
        {
            fpsToggleImage.color =
                enabled
                ? fpsOnColor
                : fpsOffColor;
        }

        PlayerPrefs.SetInt(
            "FPSLimit",
            enabled ? 1 : 0
        );


        PlayerPrefs.Save();
    }



    // =========================
    // INSTRUCTIONS
    // =========================


    public void OpenInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);
    }



    public void CloseInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);
    }



    // =========================
    // LOAD
    // =========================


    private void LoadSettings()
    {

        int quality =
            PlayerPrefs.GetInt(
                "Quality",
                QualitySettings.GetQualityLevel()
            );


        QualitySettings.SetQualityLevel(
            quality
        );


        if (qualityDropdown != null)
            qualityDropdown.value = quality;



        float master =
            PlayerPrefs.GetFloat(
                "Master",
                1f
            );


        float music =
            PlayerPrefs.GetFloat(
                "Music",
                1f
            );


        float sfx =
            PlayerPrefs.GetFloat(
                "SFX",
                1f
            );



        if (masterSlider != null)
            masterSlider.value = master;


        if (musicSlider != null)
            musicSlider.value = music;


        if (sfxSlider != null)
            sfxSlider.value = sfx;



        SetMaster(master);
        SetMusic(music);
        SetSFX(sfx);




        float sensitivity =
            PlayerPrefs.GetFloat(
                "Sensitivity",
                1f
            );


        if (sensitivitySlider != null)
            sensitivitySlider.value = sensitivity;



        bool fpsLimit =
            PlayerPrefs.GetInt(
                "FPSLimit",
                1
            ) == 1;



        if (fpsToggle != null)
            fpsToggle.isOn = fpsLimit;



        SetFPS(fpsLimit);
    }
}