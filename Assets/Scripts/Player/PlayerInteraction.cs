using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform origin;

    private IIngredientReceiver currentReceiver;
    private OrdersZone currentOrdersZone;

    private void Update()
    {
        DetectReceiver();
    }

    private void DetectReceiver()
    {
        currentReceiver = null;

        Collider[] hits = Physics.OverlapSphere(
            origin.position,
            interactRadius,
            interactLayer
        );

        float closest = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out IIngredientReceiver receiver))
            {
                float dist = Vector3.Distance(origin.position, hit.transform.position);

                if (dist < closest)
                {
                    closest = dist;
                    currentReceiver = receiver;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out OrdersZone zone))
        {
            currentOrdersZone = zone;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out OrdersZone zone))
        {
            if (currentOrdersZone == zone)
                currentOrdersZone = null;
        }
    }

    public void OnPanel(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentOrdersZone != null)
        {
            currentOrdersZone.Open();
            return;
        }

        Debug.Log("[PLAYER] Nothing to open");
    }

    public void OnFinish(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentReceiver == null)
        {
            Debug.Log("[PLAYER] No receiver to finish");
            return;
        }

        if (currentReceiver is IBrewFinishReceiver finisher)
        {
            finisher.FinishBrew();
        }
        else
        {
            Debug.Log("[PLAYER] Current receiver cannot finish brewing");
        }
    }

    public void OnHeat(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentReceiver is Cauldron cauldron && cauldron.UI != null)
        {
            cauldron.UI.Heat();
        }
    }

    public void OnStir(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentReceiver is Cauldron cauldron && cauldron.UI != null)
        {
            cauldron.UI.Stir();
        }
    }
}