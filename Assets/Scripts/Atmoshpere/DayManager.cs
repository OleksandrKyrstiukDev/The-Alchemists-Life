using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private DayTime dayTime;
    [SerializeField] private OrderBoard orderBoard;

    [Header("Orders")]
    [SerializeField] private int ordersPerDay = 5;

    public int ActiveOrders { get; private set; }

    public DayPhase CurrentPhase { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        StartNewDay();
    }

    public void StartNewDay()
    {
        CurrentPhase = DayPhase.Morning;

        if (dayTime != null)
            dayTime.SetPhase(DayPhase.Morning);

        GenerateOrders();

        SetWorkPhase();

        Debug.Log($"[DAY] Start day {GameManager.Instance.CurrentDay}");
    }

    private void GenerateOrders()
    {
        ActiveOrders = ordersPerDay;

        if (orderBoard != null)
        {
            orderBoard.GenerateDailyOrders(ordersPerDay);
        }

        Debug.Log($"[DAY] Generated {ActiveOrders} orders");
    }

    private void SetWorkPhase()
    {
        CurrentPhase = DayPhase.Work;

        if (dayTime != null)
            dayTime.SetPhase(DayPhase.Work);
    }

    public void CompleteOrder()
    {
        if (ActiveOrders <= 0)
            return;

        ActiveOrders--;

        Debug.Log($"[DAY] Order completed. Left: {ActiveOrders}");

        CheckDayProgress();
    }

    public void DeclineOrder()
    {
        if (ActiveOrders <= 0)
            return;

        ActiveOrders--;

        Debug.Log($"[DAY] Order declined. Left: {ActiveOrders}");

        CheckDayProgress();
    }

    private void CheckDayProgress()
    {
        if (ActiveOrders <= 0)
        {
            BeginEvening();
        }
    }

    private void BeginEvening()
    {
        CurrentPhase = DayPhase.Evening;

        if (dayTime != null)
            dayTime.SetPhase(DayPhase.Evening);

        Debug.Log("[DAY] Evening started");
    }

    public void BeginNight()
    {
        CurrentPhase = DayPhase.Night;

        if (dayTime != null)
            dayTime.SetPhase(DayPhase.Night);

        Debug.Log("[DAY] Night started");
    }
}