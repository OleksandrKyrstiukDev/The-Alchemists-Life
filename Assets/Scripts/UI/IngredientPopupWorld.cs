using TMPro;
using UnityEngine;


public class IngredientPopupWorld : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private SpriteRenderer icon;


    [Header("Animation")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float lifeTime = 1.5f;


    private float timer;

    public void Setup(
        IngredientData ingredient,
        int amount)
    {

        text.text =
            $"Додано: {ingredient.displayName} x{amount}";


        if (icon != null)
            icon.sprite = ingredient.icon;


    }

    private void Update()
    {

        transform.position +=
            Vector3.up *
            moveSpeed *
            Time.deltaTime;



        timer += Time.deltaTime;


        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }

    }
}