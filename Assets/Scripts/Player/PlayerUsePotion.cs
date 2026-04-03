using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUsePotion : MonoBehaviour
{
    [Header("Potion")]
    [SerializeField] private Transform handTransform;

    private BrewedPotion currentPotion;
    private bool hasPotion;
    private PotionObject potionInHand;

    // Input System
    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;
        UsePotion();
    }

    private PotionZone currentZone;

    void OnTriggerEnter(Collider other)
    {
        var zone = other.GetComponent<PotionZone>();
        if (zone != null)
            currentZone = zone;
    }

    void OnTriggerExit(Collider other)
    {
        var zone = other.GetComponent<PotionZone>();
        if (zone == currentZone)
            currentZone = null;
    }

    void UsePotion()
    {
        if (potionInHand == null)
        {
            Debug.Log("[PlayerUsePotion] No potion in hand");
            return;
        }

        if (currentZone == null)
        {
            Debug.Log("[PlayerUsePotion] No potion zone");
            return;
        }

        if (!currentZone.CanApply(potionInHand))
        {
            Debug.Log("[PlayerUsePotion] Potion not in zone"); 
            return;
        }

        currentZone.Apply(potionInHand);
        Destroy(potionInHand.gameObject);

        potionInHand = null;

        Debug.Log("[PlayerUsePotion] Potion used");
    }



    // Викликається Cauldron'ом
    public void GivePotion(BrewedPotion potion)
    {
        if (potionInHand != null)
            Destroy(potionInHand.gameObject);

        currentPotion = potion;

        GameObject potionGO = Instantiate(
            potion.prefab,
            handTransform.position,
            handTransform.rotation,
            handTransform
        );

        potionGO.transform.localPosition = Vector3.zero;
        potionGO.transform.localRotation = Quaternion.identity;
        potionGO.transform.localScale = Vector3.one;

        // ВИМИКАЄМО фізику
        if (potionGO.TryGetComponent(out Rigidbody rb))
            rb.isKinematic = true;

        foreach (var c in potionGO.GetComponentsInChildren<Collider>())
            c.isTrigger = true;

        potionInHand = potionGO.GetComponent<PotionObject>();
        potionInHand.Init(potion.data);

        Debug.Log($"[PlayerUsePotion] Potion in hand: {potion.data.name}");
    }

}
