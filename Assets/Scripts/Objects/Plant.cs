using UnityEngine;

public class Plant : MonoBehaviour
{
    [Header("Links")]
    [SerializeField] private PlantCareReaction plantcareReaction;

    [Header("Potion Effects")]
    [SerializeField] private float perfectScaleMultiplier = 2f;
    [SerializeField] private float goodScaleMultiplier = 1.3f;

    [Header("Growth")]
    [SerializeField] private int daysToGrow = 4;
    [SerializeField] private float seedScale = 0.2f;

    [Header("Harvest")]
    [SerializeField] private IngredientData ingredient;
    [SerializeField] private int harvestAmount = 3;
    private Vector3 matureScale;
    [SerializeField] private StorageInventory storageInventory;
    private int growthDays;

    private bool mature;
    public bool CanHarvest => mature;
    private void Awake()
    {
        if (plantcareReaction == null)
            plantcareReaction =
                GetComponent<PlantCareReaction>();

        matureScale = transform.localScale;

        transform.localScale =
            matureScale * seedScale;
    }

    private void OnEnable()
    {
        if (plantcareReaction != null)
            plantcareReaction.OnStateChanged +=
                OnPlantStateChanged;

        DayManager.OnNewDay += GrowOneDay;
    }

    private void OnDisable()
    {
        if (plantcareReaction != null)
            plantcareReaction.OnStateChanged -=
                OnPlantStateChanged;

        DayManager.OnNewDay -= GrowOneDay;
    }

    private void GrowOneDay()
    {
        if (mature)
            return;

        growthDays++;

        float progress =
            growthDays / (float)daysToGrow;

        progress = Mathf.Clamp01(progress);

        transform.localScale =
            Vector3.Lerp(
                matureScale * seedScale,
                matureScale,
                progress
            );

        if (progress >= 1f)
        {
            MakeMature();
        }
    }
    private void MakeMature()
    {
        mature = true;

        growthDays = daysToGrow;

        transform.localScale = matureScale;

        Debug.Log("[Plant] Mature");
    }


    public void Harvest()
    {
        if (!mature)
        {
            Debug.Log("[Plant] Not ready for harvest");
            return;
        }

        if (ingredient == null)
        {
            Debug.LogWarning("[Plant] Ingredient is NULL");
            return;
        }

        if (storageInventory == null)
        {
            Debug.LogWarning("[Plant] StorageInventory is NULL");
            return;
        }

        storageInventory.AddIngredient(
            ingredient,
            harvestAmount
        );

        Debug.Log(
            $"[Plant] Harvested {harvestAmount}x {ingredient.displayName}"
        );

        growthDays = 0;
        mature = false;

        transform.localScale =
            matureScale * seedScale;
    }

    public void Interact()
    {
        if (CanHarvest)
        {
            Harvest();
        }
        else
        {
            Debug.Log("[Plant] Not ready");
        }
    }

    private void OnPlantStateChanged(
     PlantState state
 )
    {
        switch (state)
        {
            case PlantState.Grown:

                MakeMature();

                break;

            case PlantState.Overgrown:

                MakeMature();

                break;

            case PlantState.Dead:

                DestroyPlant();

                break;
        }
    }

    private void DestroyPlant()
    {
        Debug.Log(
            "[Plant] Destroyed"
        );

        Destroy(gameObject);
    }
}