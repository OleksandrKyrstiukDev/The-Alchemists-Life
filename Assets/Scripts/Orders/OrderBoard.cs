using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderBoard : MonoBehaviour
{
    [Header("All Orders")]
    [SerializeField] private OrderObject[] orders;

    [Header("References")]
    [SerializeField] private OrdersUI ordersUI;

    [SerializeField] private PlayerStats playerStats;

    public void GenerateDailyOrders(int ordersCount)
    {
        Debug.Log("[OrderBoard] GenerateDailyOrders");

        ReputationTier playerTier = playerStats.CurrentTier;

        List<OrderObject> availableOrders = orders
            .Where(order => order.requiredTier <= playerTier)
            .ToList();

        Debug.Log(
            $"[OrderBoard] Available orders for {playerTier}: {availableOrders.Count}"
        );

        if (availableOrders.Count == 0)
        {
            Debug.LogWarning("[OrderBoard] No available orders!");
            return;
        }

        availableOrders = availableOrders
            .OrderBy(x => Random.value)
            .ToList();

        int count = Mathf.Min(
            ordersCount,
            availableOrders.Count
        );

        for (int i = 0; i < count; i++)
        {
            ordersUI.CreateOrderItem(
                availableOrders[i]
            );
        }
    }
}