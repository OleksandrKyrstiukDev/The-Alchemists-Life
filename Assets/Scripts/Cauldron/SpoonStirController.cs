using System.Collections;
using UnityEngine;

public class SpoonStirController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float stirDuration = 1f;

    private bool isStirring;

    public bool CanStir => !isStirring;

    public void Stir()
    {
        Debug.Log("[SpoonStir] Stir() ENTERED");

        if (animator == null)
        {
            Debug.LogError("[SpoonStir] Animator is NULL");
            return;
        }

        Debug.Log("[SpoonStir] Animator OK");

        if (isStirring)
        {
            Debug.Log("[SpoonStir] BLOCKED (already stirring)");
            return;
        }

        Debug.Log("[SpoonStir] Starting coroutine");

        StartCoroutine(StirRoutine());
    }

    private IEnumerator StirRoutine()
    {
        Debug.Log("[SpoonStir] Coroutine START");

        isStirring = true;

        Debug.Log("[SpoonStir] Setting Trigger: Stir");

        animator.SetTrigger("StirTrigger");

        Debug.Log("[SpoonStir] Trigger SENT");

        yield return new WaitForSeconds(stirDuration);

        isStirring = false;

        Debug.Log("[SpoonStir] Coroutine END");
    }
}