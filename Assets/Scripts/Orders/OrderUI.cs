using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrdersUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject detailsPanel;

    [Header("Details")]
    public Image portrait;
    public TextMeshProUGUI clientNameText;
    public TextMeshProUGUI fullDescriptionText;
    public TextMeshProUGUI restrictionsText;
    public TextMeshProUGUI rewardsText;

    [Header("Buttons")]
    public Button submitButton;
    public Button declineButton;

    private OrderObject currentOrder;
    private PlayerUsePotion playerUsePotion;

    [Header("Order List")]
    [SerializeField] private Transform listContainer;
    [SerializeField] private OrderItemUI orderItemPrefab;
    private void Awake()
    {
        if (playerUsePotion == null)
            playerUsePotion = FindFirstObjectByType<PlayerUsePotion>();

        if (detailsPanel != null)
            detailsPanel.SetActive(false);
    }

    public void ShowOrder(OrderObject order)
    {
        currentOrder = order;

        if (detailsPanel != null)
            detailsPanel.SetActive(true);

        portrait.sprite = order.portrait;

        clientNameText.text = order.clientName;
        fullDescriptionText.text = order.fullDescription;
        restrictionsText.text = order.restrictions;

        rewardsText.text =
            $"Gold: {order.goldReward}\nRep: {order.reputationReward}";

        UpdateButtons();

        Debug.Log($"[OrdersUI] Opened order: {order.clientName}");
    }

    private void UpdateButtons()
    {
        submitButton.interactable =
            playerUsePotion != null &&
            playerUsePotion.HasPotion;
    }

    public void SubmitPotion()
    {
        if (currentOrder == null)
            return;

        PotionData potion = playerUsePotion.CurrentPotionData;

        OrderResult result =
            OrderEvaluator.Evaluate(currentOrder, potion);

        switch (result)
        {
            case OrderResult.Perfect:
                Debug.Log("[ORDER] PERFECT");
                break;

            case OrderResult.Medium:
                Debug.Log("[ORDER] MEDIUM");
                break;

            case OrderResult.Fail:
                Debug.Log("[ORDER] FAIL");
                break;
        }

        playerUsePotion.RemovePotion();

        CloseDetails();
    }

    public void DeclineOrder()
    {
        Debug.Log("[ORDER] Declined");

        CloseDetails();
    }

    private void CloseDetails()
    {
        currentOrder = null;

        if (detailsPanel != null)
            detailsPanel.SetActive(false);
    }

    public void CreateOrderItem(OrderObject order)
    {
        if (orderItemPrefab == null)
        {
            Debug.LogError("[OrdersUI] OrderItemPrefab is NULL");
            return;
        }

        if (listContainer == null)
        {
            Debug.LogError("[OrdersUI] ListContainer is NULL");
            return;
        }

        OrderItemUI item =
            Instantiate(orderItemPrefab, listContainer);

        item.Setup(order, this);

        Debug.Log($"[OrdersUI] Created item: {order.clientName}");
    }
}