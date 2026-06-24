using UnityEngine;

public class PlantSway : MonoBehaviour
{
    [SerializeField] private float swayAmount = 5f;
    [SerializeField] private float swaySpeed = 1f;

    private Quaternion startRotation;
    private float randomOffset;

    private void Start()
    {
        startRotation = transform.localRotation;
        randomOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float angle =
            Mathf.Sin(Time.time * swaySpeed + randomOffset)
            * swayAmount;

        transform.localRotation =
            startRotation *
            Quaternion.Euler(0f, 0f, angle);
    }
}