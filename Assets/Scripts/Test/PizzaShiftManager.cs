using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PizzaShiftManager : MonoBehaviour
{
    public static PizzaShiftManager Instance { get; private set; }

    private enum ShiftState
    {
        ServingCustomers,
        FinalDelivery,
        Won,
        Lost
    }

    [Header("Quota")]
    [Min(1f)] public float shiftLengthSeconds = 600f;
    [Min(1)] public int moneyQuota = 50;
    [Min(1)] public int moneyPerPizza = 10;

    [Header("Normal Customer Phase")]
    [Tooltip("The customer spawner or parent containing the normal customer system.")]
    public GameObject regularCustomerSystem;

    [Header("Final Mountain Delivery")]
    [Tooltip("Parent containing the mountain, climbing objects and final delivery zone.")]
    public GameObject mountainDeliverySection;

    [Tooltip("A finished pizza prefab ready to be carried.")]
    public GameObject finalPizzaPrefab;

    public Transform finalPizzaSpawnPoint;

    [Header("UI")]
    public TMP_Text timerText;
    public TMP_Text quotaText;
    public TMP_Text objectiveText;
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";
    public float returnToMenuDelay = 3f;
    public bool returnToMenuAfterWin = true;

    private float timeRemaining;
    private int moneyEarned;
    private ShiftState state;

    // Stops the same pizza being counted twice.
    private readonly HashSet<int> soldPizzaIDs = new();

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
        Time.timeScale = 1f;

        timeRemaining = shiftLengthSeconds;
        moneyEarned = 0;
        state = ShiftState.ServingCustomers;

        if (mountainDeliverySection != null)
            mountainDeliverySection.SetActive(false);

        if (regularCustomerSystem != null)
            regularCustomerSystem.SetActive(true);

        if (losePanel != null)
            losePanel.SetActive(false);

        if (winPanel != null)
            winPanel.SetActive(false);

        int pizzasRequired =
            Mathf.CeilToInt((float)moneyQuota / moneyPerPizza);

        if (objectiveText != null)
        {
            objectiveText.text =
                $"Sell {pizzasRequired} pizzas before time runs out!";
        }

        UpdateUI();
    }

    private void Update()
    {
        if (state != ShiftState.ServingCustomers)
            return;

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            UpdateUI();
            LoseShift();
            return;
        }

        UpdateUI();
    }

    /// <summary>
    /// Call this when a customer successfully accepts a pizza.
    /// Pass the root GameObject of the pizza.
    /// </summary>
    public void RegisterPizzaSale(GameObject pizza)
    {
        if (state != ShiftState.ServingCustomers || pizza == null)
            return;

        int pizzaID = pizza.GetInstanceID();

        // Stops multiple colliders from selling one pizza repeatedly.
        if (!soldPizzaIDs.Add(pizzaID))
            return;

        moneyEarned += moneyPerPizza;

        Debug.Log($"Pizza sold! Money: ${moneyEarned}");

        UpdateUI();

        if (moneyEarned >= moneyQuota)
            BeginFinalDelivery();
    }

    private void BeginFinalDelivery()
    {
        state = ShiftState.FinalDelivery;

        if (regularCustomerSystem != null)
            regularCustomerSystem.SetActive(false);

        if (mountainDeliverySection != null)
            mountainDeliverySection.SetActive(true);

        if (timerText != null)
            timerText.text = "QUOTA MET!";

        if (objectiveText != null)
        {
            objectiveText.text =
                "One more pizza must be delivered!\nCarry it to the top of the mountain.";
        }

        SpawnFinalPizza();

        Debug.Log("Quota complete. Final mountain delivery started!");
    }

    private void SpawnFinalPizza()
    {
        if (finalPizzaPrefab == null || finalPizzaSpawnPoint == null)
        {
            Debug.LogWarning(
                "Final pizza prefab or spawn point has not been assigned."
            );

            return;
        }

        GameObject finalPizza = Instantiate(
            finalPizzaPrefab,
            finalPizzaSpawnPoint.position,
            finalPizzaSpawnPoint.rotation
        );

        // Marks this as the special mountain pizza.
        if (finalPizza.GetComponent<FinalDeliveryPizza>() == null)
            finalPizza.AddComponent<FinalDeliveryPizza>();
    }

    public void CompleteFinalDelivery()
    {
        if (state != ShiftState.FinalDelivery)
            return;

        state = ShiftState.Won;

        if (objectiveText != null)
            objectiveText.text = "FINAL DELIVERY COMPLETE!";

        if (winPanel != null)
            winPanel.SetActive(true);

        Debug.Log("Final delivery complete. You win!");

        if (returnToMenuAfterWin)
            StartCoroutine(ReturnToMenu());
        else
            Time.timeScale = 0f;
    }

    private void LoseShift()
    {
        if (state != ShiftState.ServingCustomers)
            return;

        state = ShiftState.Lost;

        if (regularCustomerSystem != null)
            regularCustomerSystem.SetActive(false);

        if (objectiveText != null)
            objectiveText.text = "You failed to meet the quota!";

        if (losePanel != null)
            losePanel.SetActive(true);

        StartCoroutine(ReturnToMenu());
    }

    private IEnumerator ReturnToMenu()
    {
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(returnToMenuDelay);

        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void UpdateUI()
    {
        if (quotaText != null)
            quotaText.text = $"${moneyEarned} / ${moneyQuota}";

        if (timerText == null || state != ShiftState.ServingCustomers)
            return;

        int totalSeconds = Mathf.CeilToInt(timeRemaining);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}