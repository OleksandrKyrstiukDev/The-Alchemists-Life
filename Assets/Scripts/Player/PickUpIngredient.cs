using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpIngredient : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CharacterController playerController;
    [SerializeField] private Transform handPoint;

    [Header("Pickup Settings")]
    [SerializeField] private float pickupRadius = 1.5f;
    [SerializeField] private LayerMask pickupLayer;
    [SerializeField] private float pickupHeightOffset = 0.3f;
    [Header("Cauldron Interaction")]
    [SerializeField] private float cauldronRadius = 2f;

    private GameObject currentIngredient;
    private bool isHolding;

    public bool IsHoldingIngredient => isHolding;
    public IngredientObject CurrentIngredient =>
        currentIngredient?.GetComponent<IngredientObject>();

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        if (isHolding && TryGiveIngredient())
            return;

        if (isHolding)
            Drop();
        else
            TryPickup();
    }

    private bool TryGiveIngredient()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position + Vector3.up * 2,
            cauldronRadius
            
        );

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out IIngredientReceiver receiver))
                continue;

            IngredientData data = CurrentIngredient.data;

            receiver.Receive(data);
            ConsumeHeldIngredient();

            Debug.Log("[INTERACT] Ingredient given");
            return true;
        }

        return false;
    }

    private void TryPickup()
    {
        if (isHolding)
        {
            Debug.Log("[Pickup] Already holding something");
            return;
        }

        Vector3 center =
     playerController.transform.position +
     Vector3.up * (playerController.height * 0.5f + pickupHeightOffset);

        Collider[] hits =
            Physics.OverlapSphere(center, pickupRadius, pickupLayer);

        if (hits.Length == 0) return;

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Collider col in hits)
        {
            float d = Vector3.Distance(center, col.transform.position);
            if (d < minDist)
            {
                minDist = d;
                closest = col.transform;
            }
        }

        if (closest == null) return;

        var source = closest.GetComponent<IngredientSource>();

        if (source != null)
        {
            GameObject obj = source.Take(handPoint);

            if (obj != null)
            {
                currentIngredient = obj;
                isHolding = true;

                Debug.Log("[Pickup] Took from source");
            }

            return;
        }

        Pickup(closest.gameObject);
    }

    private void Pickup(GameObject obj)
    {
        currentIngredient = obj;

        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        obj.transform.SetParent(handPoint);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

        isHolding = true;
        Debug.Log($"[Pickup] Picked {obj.name}");
    }

    private void Drop()
    {
        if (!currentIngredient) return;

        currentIngredient.transform.SetParent(null);

        if (currentIngredient.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        Debug.Log($"[Pickup] Dropped {currentIngredient.name}");

        currentIngredient = null;
        isHolding = false;
    }

    public void ConsumeHeldIngredient()
    {
        Destroy(currentIngredient);
        currentIngredient = null;
        isHolding = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2, cauldronRadius);
    }
}
