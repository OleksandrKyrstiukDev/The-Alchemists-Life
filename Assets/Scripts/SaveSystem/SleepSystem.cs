using System.Collections;
using TMPro;
using UnityEngine;

public class SleepSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup sleepPanel;
    [SerializeField] private TextMeshProUGUI dayText;

    [Header("Localization")]
    [SerializeField] private string dayKey = "DAY";


    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float sleepDuration = 10f;


    [Header("Audio")]
    [SerializeField] private AudioSource sleepAudio;
    [SerializeField] private float fadeOutAudioDuration = 2f;


    [Header("Player")]
    [SerializeField] private MonoBehaviour playerController;


    private bool sleeping;



    public void StartSleep()
    {
        if (sleeping)
            return;


        StartCoroutine(SleepRoutine());
    }



    private IEnumerator SleepRoutine()
    {
        sleeping = true;


        if (playerController != null)
            playerController.enabled = false;



        if (sleepAudio != null)
        {
            sleepAudio.volume = 1f;
            sleepAudio.Play();
        }



        yield return Fade(0f, 1f);



        Sleep();


        RefreshUI();



        yield return new WaitForSeconds(
            sleepDuration
        );



        yield return FadeOutAudio();



        yield return Fade(1f, 0f);



        if (playerController != null)
            playerController.enabled = true;



        sleeping = false;
    }





    public void Sleep()
    {
        Debug.Log("[SLEEP] Sleep() called");


        DayManager.Instance.BeginNight();

        GameManager.Instance.NextDay();

        DayManager.Instance.StartNewDay();
    }






    private IEnumerator Fade(float from, float to)
    {
        float t = 0f;


        while (t < fadeDuration)
        {
            t += Time.deltaTime;


            sleepPanel.alpha =
                Mathf.Lerp(
                    from,
                    to,
                    t / fadeDuration
                );


            yield return null;
        }


        sleepPanel.alpha = to;
    }






    private IEnumerator FadeOutAudio()
    {
        if (sleepAudio == null)
            yield break;


        float startVolume =
            sleepAudio.volume;


        float t = 0f;


        while (t < fadeOutAudioDuration)
        {
            t += Time.deltaTime;


            sleepAudio.volume =
                Mathf.Lerp(
                    startVolume,
                    0f,
                    t / fadeOutAudioDuration
                );


            yield return null;
        }


        sleepAudio.volume = 0f;

        sleepAudio.Stop();
    }







    public void RefreshUI()
    {
        if (GameManager.Instance == null)
            return;



        string text =
            LocalizationManager.Instance.Get(
                dayKey
            );



        dayText.text =
            $"{text} {GameManager.Instance.CurrentDay}";
    }
}