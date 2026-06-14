using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrderItemUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI clientNameText;
    [SerializeField] private TextMeshProUGUI shortDescriptionText;

    private OrderObject order;
    private OrdersUI ordersUI;

    public OrderObject CurrentOrder => order;

    public void Setup(OrderObject data, OrdersUI ui)
    {
        order = data;
        ordersUI = ui;

        clientNameText.text = order.clientName;
        shortDescriptionText.text = order.shortDescription;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[OrderItemUI] CLICK: {order?.clientName}");

        if (ordersUI == null)
        {
            Debug.LogError("[OrderItemUI] OrdersUI is NULL");
            return;
        }

        if (order == null)
        {
            Debug.LogError("[OrderItemUI] Order is NULL");
            return;
        }

        ordersUI.ShowOrder(order);
    }

    public void RemoveSelf()
    {
        Destroy(gameObject);
    }
}