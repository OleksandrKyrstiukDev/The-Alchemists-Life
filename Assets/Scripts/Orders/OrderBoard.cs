using UnityEngine;

public class OrderBoard : MonoBehaviour
{
    public OrderObject[] orders;
    public OrdersUI ordersUI;

    private void Start()
    {
        SpawnOrders();
    }

    void SpawnOrders()
    {
        Debug.Log("SpawnOrders called");

        foreach (var order in orders)
        {
            Debug.Log("Spawning: " + order.name);
            ordersUI.CreateOrderItem(order);
        }
    }
}