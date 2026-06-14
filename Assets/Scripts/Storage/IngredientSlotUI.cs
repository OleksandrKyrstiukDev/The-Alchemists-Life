using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class IngredientSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private TextMeshProUGUI Price;

    [SerializeField] private Button buy1Button;
    [SerializeField] private Button buy5Button;

    private InventorySlot slot;
    private StorageUI storageUI;

    public void Setup(InventorySlot slot, StorageUI ui)
    {
        this.slot = slot;
        storageUI = ui;

        icon.sprite = slot.ingredient.icon;
        nameText.text = slot.ingredient.displayName;
        amountText.text = $"x{slot.amount}";
        Price.text = $"{slot.ingredient.buyPrice} зол.";

        buy1Button.onClick.RemoveAllListeners();
        buy5Button.onClick.RemoveAllListeners();

        buy1Button.onClick.AddListener(() => storageUI.Buy(slot.ingredient, 1));
        buy5Button.onClick.AddListener(() => storageUI.Buy(slot.ingredient, 5));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        storageUI.TakeIngredient(slot);
    }
}