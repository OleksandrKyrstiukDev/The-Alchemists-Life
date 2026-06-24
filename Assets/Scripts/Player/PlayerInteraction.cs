using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private float interactRadius = 1.5f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private Transform origin;

    [Header("Systems")]
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] private SleepSystem sleepSystem;
    [SerializeField] private CameraFocusController cameraFocus;
    private IIngredientReceiver currentReceiver;
    private OrdersZone currentOrdersZone;
    private StorageZone currentStorageZone;
    private BedTrigger currentBed;

    public Cauldron currentCauldron;

    public SpoonStirController spoonStir;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private void Update()
    {
        DetectReceiver();
    }

    // =========================
    // DETECT CAULDRON / RECEIVER
    // =========================
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

    // =========================
    // TRIGGERS
    // =========================
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out OrdersZone zone))
            currentOrdersZone = zone;

        if (other.TryGetComponent(out StorageZone storage))
            currentStorageZone = storage;

        if (other.TryGetComponent(out BedTrigger bed))
        {
            currentBed = bed;
            Debug.Log("[PLAYER] Enter bed zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out OrdersZone zone) && currentOrdersZone == zone)
            currentOrdersZone = null;

        if (other.TryGetComponent(out StorageZone storage) && currentStorageZone == storage)
            currentStorageZone = null;

        if (other.TryGetComponent(out BedTrigger bed) && currentBed == bed)
        {
            currentBed = null;
            Debug.Log("[PLAYER] Exit bed zone");
        }
    }

    // =========================
    // UI PANEL INTERACTION
    // =========================
    public void OnPanel(InputValue value)
    {
        if (!value.isPressed) return;

        if (currentOrdersZone != null)
        {
            currentOrdersZone.Open();
            return;
        }

        if (currentStorageZone != null)
        {
            currentStorageZone.Open();
            return;
        }

        Debug.Log("[PLAYER] Nothing to open");
    }

    // =========================
    // FINISH BREW
    // =========================
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
        cameraFocus.ReturnToPlayer();

    }

    // =========================
    // CAULDRON ACTIONS
    // =========================
    public void OnHeat(InputValue value)
    {
        cameraFocus.FocusCauldron();
        if (!value.isPressed) return;

        if (currentReceiver is Cauldron cauldron && cauldron.UI != null)
        {
            cauldron.UI.Heat();
        }
    }

    public void OnStir(InputValue value)
    {
        Debug.Log("[OnStir] Called");

        if (value == null)
        {
            Debug.LogError("[OnStir] InputValue is NULL");
            return;
        }

        Debug.Log($"[OnStir] isPressed = {value.isPressed}");

        if (!value.isPressed)
        {
            Debug.Log("[OnStir] Ignored (not pressed)");
            return;
        }

        if (spoonStir == null)
        {
            Debug.LogError("[OnStir] spoonStir is NULL (not assigned in inspector)");
            return;
        }

        Debug.Log("[OnStir] spoonStir OK");

        if (!spoonStir.CanStir)
        {
            Debug.Log("[OnStir] BLOCKED - still stirring");
            return;
        }

        Debug.Log("[OnStir] CanStir = TRUE");

        if (currentReceiver == null)
        {
            Debug.LogError("[OnStir] currentReceiver is NULL");
            return;
        }

        if (currentReceiver is Cauldron cauldron)
        {
            Debug.Log("[OnStir] Receiver is Cauldron");

            if (cauldron.UI == null)
            {
                Debug.LogError("[OnStir] Cauldron.UI is NULL");
                return;
            }

            Debug.Log("[OnStir] Calling cauldron.UI.Stir()");

            cauldron.UI.Stir();

            Debug.Log("[OnStir] Calling spoonStir.Stir()");

            spoonStir.Stir();
        }
        else
        {
            Debug.Log("[OnStir] Receiver is NOT Cauldron");
        }
    }

    // =========================
    // INTERACT (INGREDIENTS)
    // =========================

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;


        // 1. SLEEP
        if (currentBed != null)
        {
            currentBed.TrySleep();
            return;
        }

        // 2. CAULDRON
        if (currentReceiver is Cauldron cauldron)
        {
            var inventory = playerInventory.Items;

            foreach (var slot in inventory)
            {
                if (slot.amount <= 0)
                    continue;

                bool success = cauldron.TryAddFromInventory(
                    playerInventory,
                    slot.ingredient
                );

                if (success)
                {
                    if (animator != null)
                        animator.SetTrigger("PickUp");
                    else Debug.LogError("Animation 0");

                        Debug.Log("[PLAYER] Added ingredient to cauldron");
                    return;
                }

            }

            Debug.Log("[PLAYER] No ingredients to add");

            return;
        }


        // PLANT
        if (currentReceiver is Plant plant)
        {
            plant.Interact();
            return;
        }
        return;
        
    }
}