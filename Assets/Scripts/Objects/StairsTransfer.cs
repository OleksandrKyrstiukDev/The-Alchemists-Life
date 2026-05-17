using UnityEngine;
using UnityEngine.InputSystem;

public class StairsTransfer : MonoBehaviour
{
    [Header("Налаштування переміщення")]
    public Transform targetExit; // Куди веде цей телепорт

    private bool isPlayerInZone = false;
    private GameObject playerObject;

    // Статична затримка, щоб не було "пінг-понгу"
    private static float lastTeleportTime;
    private const float teleportCooldown = 0.5f;

    void Update()
    {
        // 1. Перевіряємо чи гравець фізично в зоні ТЬОГО конкретного тригера
        // 2. Чи натиснута кнопка
        // 3. Чи минуло 0.5 сек з будь-якого останнього переміщення
        if (isPlayerInZone &&
            Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            Time.time > lastTeleportTime + teleportCooldown)
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (targetExit == null || playerObject == null) return;

        lastTeleportTime = Time.time;

        // ВАЖЛИВО: скидаємо стан зони ПЕРЕД телепортацією, 
        // щоб скрипт не думав, що ми все ще тут
        isPlayerInZone = false;

        CharacterController cc = playerObject.GetComponent<CharacterController>();

        if (cc != null)
        {
            cc.enabled = false;
            playerObject.transform.position = targetExit.position;
            playerObject.transform.rotation = targetExit.rotation;
            cc.enabled = true;
        }
        else
        {
            playerObject.transform.position = targetExit.position;
            playerObject.transform.rotation = targetExit.rotation;
        }

        playerObject = null; // Очищуємо посилання
        Debug.Log("Гравець переміщений до: " + targetExit.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = true;
            playerObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            playerObject = null;
        }
    }

    // Додатковий захист: якщо об'єкт вимикається, скидаємо стан
    private void OnDisable()
    {
        isPlayerInZone = false;
        playerObject = null;
    }
}