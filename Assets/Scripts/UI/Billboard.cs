using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera cam;

    [Header("Position")]
    public float forwardOffset = 0.5f;
    public float heightOffset = 0.2f;


    private Vector3 startPosition;


    void Start()
    {
        cam = Camera.main;

        startPosition = transform.position;
    }


    void LateUpdate()
    {
        if (cam == null)
            return;


        // Повертаємо текст до гравця/камери
        transform.rotation =
    Quaternion.LookRotation(
        transform.position - cam.transform.position
    );

        transform.Rotate(
            0,
            90,
            0
        );


        // Трохи піднімаємо і зміщуємо в сторону погляду камери
        Vector3 targetPos =
            startPosition +
            cam.transform.forward * forwardOffset +
            Vector3.up * heightOffset;


        transform.position =
            Vector3.Lerp(
                transform.position,
                targetPos,
                Time.deltaTime * 10f
            );
    }
}