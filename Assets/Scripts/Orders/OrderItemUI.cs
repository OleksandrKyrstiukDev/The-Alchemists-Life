using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class OrderItemUI : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI clientNameText;
    public TextMeshProUGUI shortDescriptionText;

    private OrderObject order;
    private OrdersUI ordersUI;

    public void Setup(OrderObject data, OrdersUI ui)
    {
        order = data;
        ordersUI = ui;

        clientNameText.text = order.clientName;
        shortDescriptionText.text = order.shortDescription;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK");
        OpenDetails();
    }

    public void OpenDetails()
    {
        
        ordersUI.ShowOrder(order);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("HOVER");
        transform.localScale = Vector3.one * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }
}