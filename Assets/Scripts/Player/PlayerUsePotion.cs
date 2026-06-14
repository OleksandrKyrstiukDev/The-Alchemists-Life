using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUsePotion : MonoBehaviour
{
    [Header("Potion")]
    [SerializeField] private Transform handTransform;

    private PotionObject potionInHand;
    private PotionZone currentZone;

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        Debug.Log("[PlayerUsePotion] INPUT: Interact");
        UsePotion();
    }

    void OnTriggerEnter(Collider other)
    {
        var zone = other.GetComponent<PotionZone>();
        if (zone != null)
        {
            currentZone = zone;
            Debug.Log($"[PlayerUsePotion] ENTER ZONE: {zone.name}");
        }
    }

    void OnTriggerExit(Collider other)
    {
        var zone = other.GetComponent<PotionZone>();
        if (zone == currentZone)
        {
            currentZone = null;
            Debug.Log($"[PlayerUsePotion] EXIT ZONE");
        }
    }

    void UsePotion()
    {
        if (potionInHand == null)
        {
            Debug.LogWarning("[PlayerUsePotion] No potion in hand");
            return;
        }

        if (currentZone == null)
        {
            Debug.LogWarning("[PlayerUsePotion] No zone");
            return;
        }

        if (!currentZone.CanApply(potionInHand))
        {
            Debug.LogWarning("[PlayerUsePotion] Zone rejected potion");
            return;
        }

        currentZone.Apply(potionInHand);

        Destroy(potionInHand.gameObject);
        potionInHand = null;

        Debug.Log("[PlayerUsePotion] Potion used SUCCESS");
    }

    public void GivePotion(BrewedPotion potion)
    {
        if (potion.prefab == null)
        {
            Debug.LogError("[PlayerUsePotion] prefab is NULL");
            return;
        }

        if (potionInHand != null)
            Destroy(potionInHand.gameObject);

        GameObject go = Instantiate(
      potion.prefab,
      handTransform.position,
      handTransform.rotation,
      handTransform
  );

        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;

        if (go.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        foreach (var c in go.GetComponentsInChildren<Collider>())
            c.isTrigger = true;

        potionInHand = go.GetComponent<PotionObject>();

        if (potionInHand == null)
        {
            Debug.LogError("[PlayerUsePotion] Missing PotionObject on prefab");
            return;
        }

        potionInHand.Init(potion.data);

        Debug.Log($"[PlayerUsePotion] Got potion: {potion.data.name}");
    }

    public void RemovePotion()
    {
        if (potionInHand != null)
            Destroy(potionInHand.gameObject);

        potionInHand = null;

        Debug.Log("[PlayerUsePotion] Removed potion");
    }

    public bool HasPotion => potionInHand != null;

    public BrewedPotionData? CurrentPotionData =>
        potionInHand != null ? potionInHand.Data : null;
}