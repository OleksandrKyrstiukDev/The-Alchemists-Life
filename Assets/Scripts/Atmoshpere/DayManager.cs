using System;
using UnityEngine;

public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public static event Action OnNewDay;

    [Header("References")]
    [SerializeField] private DayTime dayTime;
    [SerializeField] private OrderBoard orderBoard;

    [Header("Orders")]
    [SerializeField] private int ordersPerDay = 5;

    [Header("Time Progress")]
    [SerializeField]
    [Range(0.01f, 1f)]
    private float timePerOrder = 0.15f;

    public int ActiveOrders { get; private set; }

    public DayPhase CurrentPhase { get; private set; }

    private float dayProgress;

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

        dayProgress = 0.15f;

        if (dayTime != null)
            dayTime.SetTime(dayProgress);

        GenerateOrders();

        OnNewDay?.Invoke();

        Debug.Log(
            $"[DAY] Start day {GameManager.Instance.CurrentDay}"
        );
    }

    private void GenerateOrders()
    {
        ActiveOrders = ordersPerDay;

        if (orderBoard != null)
            orderBoard.GenerateDailyOrders(ordersPerDay);

        Debug.Log(
            $"[DAY] Generated {ActiveOrders} orders"
        );
    }

    public void CompleteOrder()
    {
        if (ActiveOrders <= 0)
            return;

        ActiveOrders--;

        Debug.Log(
            $"[DAY] Order completed. Left: {ActiveOrders}"
        );

        AdvanceTime();

        CheckDayProgress();
    }

    public void DeclineOrder()
    {
        if (ActiveOrders <= 0)
            return;

        ActiveOrders--;

        Debug.Log(
            $"[DAY] Order declined. Left: {ActiveOrders}"
        );

        AdvanceTime();

        CheckDayProgress();
    }

    private void AdvanceTime()
    {
        dayProgress += timePerOrder;

        dayProgress = Mathf.Clamp01(dayProgress);

        if (dayTime != null)
            dayTime.SetTime(dayProgress);

        UpdatePhase();

        Debug.Log(
            $"[DAY] Time advanced -> {dayProgress:0.00}"
        );
    }

    private void UpdatePhase()
    {
        DayPhase newPhase;

        if (dayProgress >= 1f)
        {
            newPhase = DayPhase.Night;
        }
        else if (dayProgress >= 0.75f)
        {
            newPhase = DayPhase.Evening;
        }
        else if (dayProgress >= 0.30f)
        {
            newPhase = DayPhase.Work;
        }
        else
        {
            newPhase = DayPhase.Morning;
        }

        if (newPhase != CurrentPhase)
        {
            CurrentPhase = newPhase;

            Debug.Log(
                $"[DAY] Phase changed -> {CurrentPhase}"
            );
        }
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

        dayProgress = Mathf.Max(
            dayProgress,
            0.75f
        );

        if (dayTime != null)
            dayTime.SetTime(dayProgress);

        Debug.Log(
            "[DAY] Evening started"
        );
    }

    public void BeginNight()
    {
        CurrentPhase = DayPhase.Night;

        dayProgress = 1f;

        if (dayTime != null)
            dayTime.SetTime(dayProgress);

        Debug.Log(
            "[DAY] Night started"
        );
    }

    public float GetDayProgress()
    {
        return dayProgress;
    }
}