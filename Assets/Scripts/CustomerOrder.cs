using System;
using UnityEngine;
using UnityEngine.UI;

public class CustomerOrder : MonoBehaviour
{
    [Serializable]
    public class PossibleOrder
    {
        public string recipeName;

        public bool cheese;
        public bool pepperoni;
        public bool jalapeno;

        [Tooltip("Image displayed above the customer's head.")]
        public Sprite pizzaIcon;
    }

    [Header("Customer")]
    [SerializeField] private string customerName = "Customer";

    [Header("Requested Pizza")]
    [SerializeField] private PizzaRecipe order = new PizzaRecipe();

    [Header("Reward")]
    [SerializeField] private int payment = 10;

    [Header("Serving")]
    [SerializeField] private float removePizzaDelay = 0.4f;
    [SerializeField] private float removeCustomerDelay = 1.5f;

    [Header("Random Customer Setup")]
    [SerializeField]
    private string[] possibleCustomerNames =
    {
        "John",
        "Sara",
        "Mariia",
        "Sean",
        "Sam",
        "Dasha",
        "Holly"
    };

    [SerializeField] private PossibleOrder[] possibleOrders;

    [Header("Order Display")]
    [SerializeField] private Image orderIcon;

    private bool orderCompleted;

    private void Start()
    {
        Debug.Log(
            $"{customerName} ordered: {order.recipeName}"
        );
    }

    public void HandlePizzaEntered(Collider other)
    {
        if (orderCompleted)
            return;

        PizzaAssembly pizza =
            other.GetComponentInParent<PizzaAssembly>();

        if (pizza == null || pizza.IsServed)
            return;

        TryServePizza(pizza);
    }

    private void TryServePizza(PizzaAssembly pizza)
    {
        if (!pizza.MatchesRecipe(order))
        {
            Debug.Log(
                $"Wrong pizza for {customerName}! " +
                $"Received: {pizza.GetIngredientDescription()}"
            );

            return;
        }

        orderCompleted = true;
        pizza.MarkAsServed();

        Debug.Log(
            $"Correct order served to {customerName}! " +
            $"+${payment}"
        );

        if (PizzaShiftManager.Instance != null)
        {
            PizzaShiftManager.Instance.RegisterPizzaSale(
                pizza.gameObject
            );
        }

        Destroy(pizza.gameObject, removePizzaDelay);

        // Destroying the customer also causes CustomerSpawnLocation
        // to release this serving position.
        Destroy(gameObject, removeCustomerDelay);
    }

    public void RandomizeCustomer()
    {
        RandomizeName();
        RandomizeOrder();
    }

    private void RandomizeName()
    {
        if (possibleCustomerNames == null ||
            possibleCustomerNames.Length == 0)
        {
            return;
        }

        int randomIndex = UnityEngine.Random.Range(
            0,
            possibleCustomerNames.Length
        );

        customerName = possibleCustomerNames[randomIndex];
    }

    private void RandomizeOrder()
    {
        if (possibleOrders == null ||
            possibleOrders.Length == 0)
        {
            Debug.LogWarning(
                $"No possible orders assigned to {gameObject.name}."
            );

            return;
        }

        int randomIndex = UnityEngine.Random.Range(
            0,
            possibleOrders.Length
        );

        PossibleOrder selectedOrder =
            possibleOrders[randomIndex];

        // These values belong inside the PizzaRecipe called "order".
        order.recipeName = selectedOrder.recipeName;
        order.cheese = selectedOrder.cheese;
        order.pepperoni = selectedOrder.pepperoni;
        order.jalapeno = selectedOrder.jalapeno;

        if (orderIcon != null)
        {
            orderIcon.sprite = selectedOrder.pizzaIcon;
            orderIcon.enabled =
                selectedOrder.pizzaIcon != null;
        }
    }
}