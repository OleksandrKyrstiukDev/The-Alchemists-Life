using UnityEngine;

public class IngredientSource : MonoBehaviour
{
    [Header("Ingredient")]
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private int amount = 5;

    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    public GameObject Take(Transform handPoint)
    {
        if (amount <= 0)
        {
            Debug.Log("[IngredientSource] Empty");
            return null;
        }

        if (ingredientPrefab == null)
        {
            Debug.LogError("[IngredientSource] Prefab is NULL!");
            return null;
        }

        GameObject obj = Instantiate(
            ingredientPrefab,
            handPoint.position,
            handPoint.rotation,
            handPoint
        );

        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;

      
        if (obj.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        amount--;

        Debug.Log($"[IngredientSource] Left: {amount}");

        if (amount <= 0)
        {
            Destroy(gameObject);
        }

        return obj;
    }
}