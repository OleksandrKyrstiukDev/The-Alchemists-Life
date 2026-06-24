using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door")]
    public Transform door;
    public float openAngle = 90f;
    public float speed = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;

    private Quaternion closedRotation;
    private Quaternion openRotation;

    private bool isPlayerInside;
    private bool wasPlayerInside;

    private Transform player;

    private void Start()
    {
        closedRotation = door.localRotation;
    }

    private void Update()
    {
        RotateDoor();
        HandleSound();
    }

    private void RotateDoor()
    {
        Quaternion target = isPlayerInside ? openRotation : closedRotation;

        door.localRotation = Quaternion.Slerp(
            door.localRotation,
            target,
            Time.deltaTime * speed
        );
    }

    private void HandleSound()
    {
        if (isPlayerInside && !wasPlayerInside)
        {
            if (audioSource != null && openSound != null)
                audioSource.PlayOneShot(openSound);
        }

        wasPlayerInside = isPlayerInside;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        player = other.transform;
        isPlayerInside = true;

        Vector3 toPlayer = player.position - door.position;

        // 🔥 КЛЮЧОВИЙ МОМЕНТ: перпендикуляр до дверей
        float side = Vector3.Dot(door.up, Vector3.Cross(door.forward, toPlayer));

        float direction = (side > 0f) ? 1f : -1f;

        openRotation = closedRotation * Quaternion.Euler(0f, 0f, openAngle * direction);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        isPlayerInside = false;
        player = null;
    }
}