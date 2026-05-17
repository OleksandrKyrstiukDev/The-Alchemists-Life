using UnityEngine;

public partial class DoorController : MonoBehaviour
{
    public GameObject door; // Перетягни сюди об'єкт дверей
    public float openAngle = 90f; // Кут повороту
    public float speed = 2f; // Швидкість відкриття

    private Quaternion closedRotation;
    private Quaternion openRotation;
    private bool isPlayerInside = false;

    void Start()
    {
        // Запам'ятовуємо початковий стан
        closedRotation = door.transform.localRotation;

        // ЗМІНА ТУТ: міняємо (0, openAngle, 0) на (0, 0, openAngle)
        openRotation = closedRotation * Quaternion.Euler(0, 0, openAngle);
    }

    void Update()
    {
        // Плавна анімація повороту
        if (isPlayerInside)
        {
            door.transform.localRotation = Quaternion.Slerp(door.transform.localRotation, openRotation, Time.deltaTime * speed);
        }
        else
        {
            door.transform.localRotation = Quaternion.Slerp(door.transform.localRotation, closedRotation, Time.deltaTime * speed);
        }
    }

    // Спрацьовує, коли гравець входить у зону
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = true;
        }
    }

    // Спрацьовує, коли гравець виходить із зони
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInside = false;
        }
    }
}