using UnityEngine;

public class BedTrigger : MonoBehaviour
{
    [SerializeField] private SleepSystem sleepSystem;

    private bool playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = true;

        Debug.Log("[BED] Player entered sleep zone");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInside = false;

        Debug.Log("[BED] Player left sleep zone");
    }

    public void TrySleep()
    {
        if (!playerInside)
            return;

        sleepSystem.StartSleep();
    }
}