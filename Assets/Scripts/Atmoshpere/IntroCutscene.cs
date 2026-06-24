using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class IntroCutscene : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI storyText;


    [Header("Story Localization")]
    [SerializeField] private string storyKey;


    [Header("Typing")]
    [SerializeField] private float charactersPerSecond = 30f;


    [Header("Movement")]
    [SerializeField] private float startMoveHeight = 400f;
    [SerializeField] private float moveSpeed = 40f;


    [Header("Next Scene")]
    [SerializeField] private string nextScene = "GameScene";


    private RectTransform rect;

    private string currentStory;

    private bool finished;



    private void Awake()
    {
        rect = storyText.rectTransform;
    }



    private void Start()
    {
        LoadStory();

        storyText.maxVisibleCharacters = 0;

        StartCoroutine(TypeRoutine());
    }



    private void LoadStory()
    {
        if (LocalizationManager.Instance == null)
        {
            Debug.LogError(
                "[INTRO] LocalizationManager missing"
            );

            return;
        }


        currentStory =
            LocalizationManager.Instance.Get(
                storyKey
            );


        storyText.text = currentStory;
    }



    private IEnumerator TypeRoutine()
    {
        for (int i = 0; i <= currentStory.Length; i++)
        {
            storyText.maxVisibleCharacters = i;


            MoveText();


            yield return new WaitForSeconds(
                1f / charactersPerSecond
            );
        }


        finished = true;
    }



    private void Update()
    {
        if (!finished)
            return;


        if (Keyboard.current != null &&
            Keyboard.current.anyKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(nextScene);
        }
    }



    private void MoveText()
    {
        storyText.ForceMeshUpdate();


        float height =
            storyText.preferredHeight;


        if (height > startMoveHeight)
        {
            rect.anchoredPosition +=
                Vector2.up *
                moveSpeed *
                Time.deltaTime;
        }
    }
}