using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrdersUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject detailsPanel;
    [SerializeField] private GameObject ordersListPanel;

    private List<OrderItemUI> spawnedItems = new();

    [Header("Details")]
    public Image portrait;
    public TextMeshProUGUI clientNameText;
    public TextMeshProUGUI fullDescriptionText;
    public TextMeshProUGUI restrictionsText;
    public TextMeshProUGUI rewardsText;

    [Header("Buttons")]
    public Button submitButton;
    public Button declineButton;
    public Button closeDetailsButton;

    [Header("Systems")]
    [SerializeField] private PlayerStats playerStats;
    private PlayerUsePotion playerUsePotion;

    private OrderObject currentOrder;

    [Header("Order List")]
    [SerializeField] private Transform listContainer;
    [SerializeField] private OrderItemUI orderItemPrefab;

    private void Awake()
    {
        playerUsePotion = FindFirstObjectByType<PlayerUsePotion>();

        detailsPanel.SetActive(false);

        if (closeDetailsButton != null)
            closeDetailsButton.onClick.AddListener(CloseDetails);
    }

    // =========================
    // CREATE LIST ITEM
    // =========================
    public void CreateOrderItem(OrderObject order)
    {
        OrderItemUI item = Instantiate(orderItemPrefab, listContainer);
        item.Setup(order, this);

        spawnedItems.Add(item);
    }

    // =========================
    // OPEN DETAILS
    // =========================
    public void ShowOrder(OrderObject order)
    {
        currentOrder = order;

        ordersListPanel.SetActive(false);
        detailsPanel.SetActive(true);

        portrait.sprite = order.portrait;

        clientNameText.text = order.clientName;
        fullDescriptionText.text = order.fullDescription;
        restrictionsText.text = order.restrictions;

        rewardsText.text =
            $"Gold: {order.goldReward}\nRep: {order.reputationReward}";

        UpdateButtons();
    }

    private void UpdateButtons()
    {
        submitButton.interactable =
            playerUsePotion != null &&
            playerUsePotion.HasPotion;
    }

    // =========================
    // SUBMIT ORDER
    // =========================
    public void SubmitPotion()
    {
        if (currentOrder == null) return;
        if (playerUsePotion == null || !playerUsePotion.HasPotion) return;

        BrewedPotionData? potion = playerUsePotion.CurrentPotionData;

        if (!potion.HasValue)
        {
            Debug.LogWarning("[OrdersUI] Potion data missing");
            return;
        }

        OrderResult result = OrderEvaluator.Evaluate(currentOrder, potion.Value);

        switch (result)
        {
            case OrderResult.Perfect:
                playerStats.AddGold(currentOrder.goldReward);
                playerStats.AddReputation(currentOrder.reputationReward);
                break;

            case OrderResult.Medium:
                playerStats.AddGold(Mathf.RoundToInt(currentOrder.goldReward * 0.5f));
                playerStats.AddReputation(Mathf.RoundToInt(currentOrder.reputationReward * 0.5f));
                break;

            case OrderResult.Fail:
                playerStats.RemoveReputation(1);
                break;
        }

        playerUsePotion.RemovePotion();

        RemoveOrder(currentOrder);
        if (DayManager.Instance != null)
            DayManager.Instance.CompleteOrder();
        else
            Debug.LogError("[OrdersUI] DayManager.Instance is NULL");
        CloseDetails();
    }

    // =========================
    // DECLINE ORDER
    // =========================
    public void DeclineOrder()
    {
        if (currentOrder != null && playerStats != null)
        {
            playerStats.RemoveReputation(currentOrder.declinePenalty);
        }

        RemoveOrder(currentOrder);
        if (DayManager.Instance != null)
            DayManager.Instance.CompleteOrder();
        else
            Debug.LogError("[OrdersUI] DayManager.Instance is NULL");
        CloseDetails();
    }

    // =========================
    // CLOSE DETAILS
    // =========================
    private void CloseDetails()
    {
        currentOrder = null;

        detailsPanel.SetActive(false);
        ordersListPanel.SetActive(true);
    }

    // =========================
    // REMOVE UI ITEM
    // =========================
    public void RemoveOrder(OrderObject order)
    {
        OrderItemUI item = spawnedItems
            .FirstOrDefault(x => x.CurrentOrder == order);

        if (item != null)
        {
            spawnedItems.Remove(item);
            item.RemoveSelf();
        }
    }
}