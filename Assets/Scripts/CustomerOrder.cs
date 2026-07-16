using UnityEngine;

public class CustomerOrder : MonoBehaviour
{
    [Header("Customer")]
    [SerializeField] private string customerName = "Customer";

    [Header("Requested Pizza")]
    [SerializeField] private PizzaRecipe order = new PizzaRecipe();

    [Header("Reward")]
    [SerializeField] private int payment = 10;

    [Header("Serving")]
    [SerializeField] private float removePizzaDelay = 0.4f;

    private bool orderCompleted;

    private void Start()
    {
        Debug.Log(
            $"{customerName} ordered: {order.recipeName}"
        );
    }

    private void OnTriggerEnter(Collider other)
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

        // Later:
        // DayManager.Instance.AddMoney(payment);
        // CustomerManager.Instance.SpawnNextCustomer();
        
        PizzaShiftManager.Instance.RegisterPizzaSale(pizza.gameObject);

        Destroy(pizza.gameObject, removePizzaDelay);
    }
}